using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Settings;
using Infrastructure.ExternalServices.IranKish;
using Infrastructure.ExternalServices.IranKish.Dtos;
using Infrastructure.ExternalServices.IranKish.Helpers;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace Application.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentServiceSettings _settings;
    private readonly IIranKishClient _client;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IOptions<PaymentServiceSettings> settings,
        IIranKishClient client,
        IUnitOfWork unitOfWork)
    {
        _settings = settings.Value;
        _client = client;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GetTokenAsync(GetTokenRequest model)
    {
        //generate auth envelope
        var envelope = CryptoHelper.GenerateAuthenticationEnvelope(_settings.TerminalId, model.Amount, _settings.Password, _settings.PublicKey);

        //init transaction into db
        var transaction = new PaymentTransaction(amount: model.Amount,
            terminalId: _settings.TerminalId,
            acceptorId: _settings.AcceptorId,
            type: TransactionType.Purchase);

        await _unitOfWork.Transactions.AddAsync(transaction);
        await _unitOfWork.SaveAsync();

        //prepare req
        var tokenRequest = new TokenRequest
        {
            authenticationEnvelope = new AuthenticationEnvelope
            {
                iv = envelope.IV,
                data = envelope.Data
            },
            request = new Request
            {
                transactionType = transaction.Type.ToString(),
                terminalId = _settings.TerminalId,
                acceptorId = _settings.AcceptorId,
                amount = model.Amount,
                revertUri = model.ReturnUrl,
                requestId = transaction.RequestId,
                requestTimestamp = ((DateTimeOffset)transaction.CreatedDate.ToUniversalTime()).ToUnixTimeSeconds()
            }
        };

        //call api
        var response = await _client.GetTokenAsync(tokenRequest);

        //validate token
        //update db
        if (response?.status ?? false)
        {
            var token = response.result?.ToString();
            transaction.Tokenized(token, JsonSerializer.Serialize(tokenRequest));
            await _unitOfWork.Transactions.SaveAsync();

            return token;
        }
        else
        {
            transaction.TokenGenerationFailed(JsonSerializer.Serialize(tokenRequest));
            await _unitOfWork.SaveAsync();

            return null;
        }
    }

    public bool IsTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return true;
    }

    public async Task<VerifyResponse> Verify(VerifyRequest model)
    {
        if (!model.IsValid())
        {

        }

        //get transaction
        var transaction = await _unitOfWork.Transactions.GetFirstOrDefaultAsync(x => x.Token == model.Token && x.RequestId == model.RequestId);
        if (transaction == null)
        {
            //log
            throw new Exception($"Transaction not found. Token: {model.Token}, RequestId: {model.RequestId}");
        }

        //map response code
        if (!model.IsSuccessful())
        {
            transaction.PaymentFailed(parameters: JsonSerializer.Serialize(model));
            //log
            throw new Exception($"Payment failed. TransactionId: {transaction.Id}");
        }

        transaction.Paid(parameters: JsonSerializer.Serialize(model));
        await _unitOfWork.SaveAsync();

        var request = new ConfirmRequest
        {
            terminalId = _settings.TerminalId,
            tokenIdentity = model.Token,
            retrievalReferenceNumber = model.RetrievalReferenceNumber,
            systemTraceAuditNumber = model.SystemTraceAuditNumber
        };

        var response = await _client.ConfirmAsync(request);

        //map response code to status
        var res = new VerifyResponse
        {
            PaymentStatus = transaction.PaymentStatus, //?
            Amount = response.amount,
            TransactionDate = DateTime.ParseExact($"{response.transactionDate:D8}{response.transactionTime:D6}", "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        };

        if (!res.IsSuccessful())
        {
            transaction.VerificationFailed(parameters: JsonSerializer.Serialize(model));
            await _unitOfWork.SaveAsync();
            //log
            throw new Exception($"Verification failed. TransactionId: {transaction.Id}");
        }

        transaction.Verified(parameters: JsonSerializer.Serialize(model));
        await _unitOfWork.SaveAsync();

        return res;
    }
}
