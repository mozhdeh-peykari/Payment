using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Settings;
using Infrastructure.ExternalServices.IranKish;
using Infrastructure.ExternalServices.IranKish.Dtos;
using Infrastructure.ExternalServices.IranKish.Helpers;
using Infrastructure.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;

namespace Application.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentServiceSettings _settings;
    private readonly IIranKishClient _client;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public PaymentService(IOptions<PaymentServiceSettings> settings,
        IIranKishClient client,
        IUnitOfWork unitOfWork,
        ILogger logger)
    {
        _settings = settings.Value;
        _client = client;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(GetTokenRequest model)
    {
        //generate auth envelope
        var envelope = CryptoHelper.GenerateAuthenticationEnvelope(_settings.TerminalId, model.Amount, _settings.Password, _settings.PublicKey);

        //init payment into db
        var payment = new Payment(amount: model.Amount,
            terminalId: _settings.TerminalId,
            acceptorId: _settings.AcceptorId,
            type: PaymentType.Purchase);

        await _unitOfWork.Payments.AddAsync(payment);
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
                transactionType = payment.Type.ToString(),
                terminalId = _settings.TerminalId,
                acceptorId = _settings.AcceptorId,
                amount = model.Amount,
                revertUri = model.ReturnUrl,
                requestId = payment.RequestId,
                requestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };

        _unitOfWork.PaymentDetails.AddAsync(new PaymentDetail
        {
            CreatedAt = DateTime.UtcNow,
            PaymentId = payment.Id,
            Request = JsonSerializer.Serialize(tokenRequest),
            Status =  

        });

        //call api
        var response = await _client.GetTokenAsync(tokenRequest);

        if (response == null || !response.status)
        {
            payment.TokenGenerationFailed(JsonSerializer.Serialize(tokenRequest));
            await _unitOfWork.SaveAsync();

            _logger.Error($"[PaymentService].[GetTokenAsync], Token generation failed");
            throw new Exception($"Token generation failed");
        }

        var token = response.result?.ToString();
        payment.Tokenized(token, JsonSerializer.Serialize(tokenRequest));
        await _unitOfWork.Transactions.SaveAsync();

        return token;
    }

    public async Task<VerifyResponse> Verify(VerifyRequest model)
    {
        //get payment
        //test:
        var payment = await _unitOfWork.Transactions.GetFirstOrDefaultAsync(x => 1 == 1);
        //var resp = new VerifyResponse
        //{
        //    PaymentStatus = payment.PaymentStatus,
        //    Amount = payment.Amount,
        //    TransactionDate = payment.CreatedDate
        //};
        //return resp;
        //var payment = await _unitOfWork.Transactions.GetFirstOrDefaultAsync(x => x.Token == model.Token && x.RequestId == model.RequestId);

        //validations
        if (payment == null)
        {
            _logger.Error($"[PaymentService].[Verify], Transaction not found. Token: {model.Token}, RequestId: {model.RequestId}");
            throw new Exception($"Transaction not found. Token: {model.Token}, RequestId: {model.RequestId}");
        }

        if (payment.PaymentState != PaymentState.Pending)
        {
            _logger.Error($"[PaymentService].[Verify], Invalid payment status. Token: {model.Token}, RequestId: {model.RequestId}, PaymentStatus: {payment.PaymentState}");
            throw new Exception($"Invalid payment status. Token: {model.Token}, RequestId: {model.RequestId}, PaymentStatus: {payment.PaymentState}");
        }

        if (!_client.IsSuccessful(model.PayResponseCode))
        {
            payment.PaymentFailed(parameters: JsonSerializer.Serialize(model));
            await _unitOfWork.SaveAsync();

            _logger.Error($"[PaymentService].[Verify], Payment failed. Token: {model.Token}, RequestId: {model.RequestId}");
            throw new Exception($"Payment failed. TransactionId: {payment.Id}");
        }

        //set as paid
        payment.Paid(parameters: JsonSerializer.Serialize(model));
        await _unitOfWork.SaveAsync();

        //confirm
        var request = new ConfirmRequest
        {
            terminalId = _settings.TerminalId,
            tokenIdentity = model.Token,
            retrievalReferenceNumber = model.RetrievalReferenceNumber,
            systemTraceAuditNumber = model.SystemTraceAuditNumber
        };

        var response = await _client.ConfirmAsync(request);

        if (response == null || response.result == null)
        {
            return null;
        }

        if (!response.status)
        {
            payment.VerificationFailed(parameters: JsonSerializer.Serialize(model));
            await _unitOfWork.SaveAsync();

            _logger.Error($"[PaymentService].[Verify], Verification failed. Token: {model.Token}, RequestId: {model.RequestId}");
            throw new Exception($"Verification failed. TransactionId: {payment.Id}");
        }

        payment.Verified(parameters: JsonSerializer.Serialize(model));
        await _unitOfWork.SaveAsync();

        var res = new VerifyResponse
        {
            PaymentState = payment.PaymentState,
            Amount = response.result.amount,
            TransactionDate = DateTime.ParseExact($"{response.result.paymentDate:D8}{response.result.paymentTime:D6}", "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
        };

        return res;
    }
}
