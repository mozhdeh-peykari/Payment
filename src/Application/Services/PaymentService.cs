using Application.Dtos;
using Application.Extensions;
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

    public async Task<GetTokenResponse> GetTokenAsync(GetTokenRequest model)
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

        var paymentDetail = new PaymentDetail
        {
            CreatedAt = DateTime.UtcNow,
            PaymentId = payment.Id,
            Request = JsonSerializer.Serialize(tokenRequest),
            State = PaymentDetailState.TokenGeneration,
            IsSuccessful = false,
        };
        
        await _unitOfWork.PaymentDetails.AddAsync(paymentDetail);
        await _unitOfWork.SaveAsync();

        //call api
        var response = await _client.GetTokenAsync(tokenRequest);

        paymentDetail.IsSuccessful = response.isSuccessful;
        paymentDetail.Response = JsonSerializer.Serialize(response);
        _unitOfWork.PaymentDetails.Update(paymentDetail);
        await _unitOfWork.SaveAsync();

        //await _unitOfWork.PaymentDetails.AddAsync(new PaymentDetail
        //{
        //    CreatedAt = DateTime.UtcNow,
        //    PaymentId = payment.Id,
        //    Request = JsonSerializer.Serialize(tokenRequest),
        //    State = PaymentDetailState.TokenGeneration,
        //    IsSuccessful = false,
        //});

        if (response == null || response.result == null || !response.status)
        {
            //payment.TokenGenerationFailed(JsonSerializer.Serialize(tokenRequest));
            //await _unitOfWork.SaveAsync();

            _logger.Error($"[PaymentService].[GetTokenAsync], Token generation failed");
            return new GetTokenResponse
            {
                ErrorCode = "",
                ErrorMessage = "",
                IsSuccessful = false,
            };
            //throw new Exception($"Token generation failed");
        }

        //var token = response.result?.ToString();
        //payment.Tokenized(token, JsonSerializer.Serialize(tokenRequest));
        //await _unitOfWork.Transactions.SaveAsync();

        return new GetTokenResponse
        {
            IsSuccessful = true,
            Result = response.result,
        };
    }

    public async Task<VerifyResponse> Verify(VerifyRequest model)
    {
        var payment = await _unitOfWork.Payments.GetFirstOrDefaultAsync(x => x.Token == model.Token && x.RequestId == model.RequestId);

        //validations
        if (payment == null)
        {
            _logger.Error($"Transaction not found. VerifyRequest: {JsonSerializer.Serialize(model)}");
            return new VerifyResponse
            {
                ErrorCode = "",
                ErrorMessage = "",
                IsSuccessful = false,
            };
        }

        var paymentDetail = new PaymentDetail
        {
            CreatedAt = DateTime.UtcNow,
            IsSuccessful = false,
            State = PaymentDetailState.ReturnedFromGateway,
            Request = JsonSerializer.Serialize(model),
            PaymentId = payment.Id
        };
        await _unitOfWork.PaymentDetails.AddAsync(paymentDetail);
        await _unitOfWork.SaveAsync();

        if (payment.PaymentState != PaymentState.Pending)
        {
            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = (int)GlobalExceptions.InvalidStatus;
            await _unitOfWork.SaveAsync();

            _logger.Error($"Invalid payment state. PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = (int)GlobalExceptions.InvalidStatus,
                ErrorMessage = GlobalExceptions.InvalidStatus.GetDescription(),
                IsSuccessful = false,
            };
        }
        if (payment.Amount != payment.Amount)
        {
            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = "";
            await _unitOfWork.SaveAsync();

            _logger.Error($"Invalid amount. PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = "",
                ErrorMessage = "",
                IsSuccessful = false,
            };
        }

        if (!_client.IsSuccessful(model.PayResponseCode))
        {
            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = "";
            await _unitOfWork.SaveAsync();

            _logger.Error($"Payment failed. PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = "",
                ErrorMessage = "",
                IsSuccessful = false,
            };
        }



        ///////////////////////



        //confirm

        var request = new ConfirmRequest
        {
            terminalId = _settings.TerminalId,
            tokenIdentity = model.Token,
            retrievalReferenceNumber = model.RetrievalReferenceNumber,
            systemTraceAuditNumber = model.SystemTraceAuditNumber
        };

        var confirmDetail = new PaymentDetail
        {
            CreatedAt = DateTime.UtcNow,
            IsSuccessful = false,
            PaymentId = payment.Id,
            Request = JsonSerializer.Serialize(request),
            State = PaymentDetailState.Verification,
        };
        await _unitOfWork.SaveAsync();

        var response = await _client.ConfirmAsync(request);

        if (!response.isSuccessful)
        {
            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = "";
            confirmDetail.Response = JsonSerializer.Serialize(response);
            await _unitOfWork.SaveAsync();

            _logger.Error($"Payment failed. PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = "",
                ErrorMessage = "",
                IsSuccessful = false,
            };
        }

        payment.PaymentState = PaymentState.Paid;
        confirmDetail.Response = JsonSerializer.Serialize(response);
        await _unitOfWork.SaveAsync();

        var res = new VerifyResponse
        {
            IsSuccessful = true,
            Result = new VerifyResult
            {
                PaymentState = PaymentState.Paid
            }
        };

        return res;
    }
}
