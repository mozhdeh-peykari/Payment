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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Application.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentServiceSettings _settings;
    private readonly IIranKishClient _client;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IOptions<PaymentServiceSettings> settings,
        IIranKishClient client,
        IUnitOfWork unitOfWork,
        ILogger<PaymentService> logger)
    {
        _settings = settings.Value;
        _client = client;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<GetTokenResponse> GetTokenAsync(GetTokenRequest model)
    {
        var envelope = CryptoHelper.GenerateAuthenticationEnvelope(_settings.TerminalId, model.Amount, _settings.Password, _settings.PublicKey);

        //req
        var requestId = Guid.NewGuid().ToString("N").Substring(0, 20);
        var tokenRequest = new TokenRequest
        {
            authenticationEnvelope = new AuthenticationEnvelope
            {
                iv = envelope.IV,
                data = envelope.Data
            },
            request = new Request
            {
                transactionType = TransactionType.Purchase.ToString(),
                terminalId = _settings.TerminalId,
                acceptorId = _settings.AcceptorId,
                amount = model.Amount,
                revertUri = model.ReturnUrl,
                requestId = requestId,
                requestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };

        //db
        var payment = new Payment
        {
            Amount = model.Amount,
            TerminalId = _settings.TerminalId,
            AcceptorId = _settings.AcceptorId,
            Type = TransactionType.Purchase,
            CreatedDate = DateTime.UtcNow,
            RequestId = requestId,
            PaymentState = PaymentState.Pending
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveAsync();

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

        //api
        var response = await _client.GetTokenAsync(tokenRequest);

        if (response == null)
        {
            var errorMessage = GlobalExceptions.ExternalServiceError.GetDescription();
            var errorCode = (int)GlobalExceptions.ExternalServiceError;

            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = errorCode;
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveAsync();

            _logger.LogError($"{errorMessage}, PaymentId: {payment.Id}");
            return new GetTokenResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                IsSuccessful = false,
            };
        }

        //save response
        paymentDetail.IsSuccessful = response.isSuccessful;
        paymentDetail.Response = JsonSerializer.Serialize(response);
        _unitOfWork.PaymentDetails.Update(paymentDetail);
        await _unitOfWork.SaveAsync();

        //check status
        if (response.status)
        {
            payment.Token = response.result.token;
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveAsync();

            return new GetTokenResponse
            {
                IsSuccessful = true,
                Result = response.result.token,
            };
        }
        else
        {
            var errorCode = (int)GlobalExceptions.TokenGenerationFailed;
            var errorMessage = GlobalExceptions.TokenGenerationFailed.GetDescription();

            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = errorCode;
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveAsync();

            _logger.LogError($"{errorMessage}, Request: {tokenRequest}, Response: {response}");
            return new GetTokenResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                IsSuccessful = false,
            };
        }
    }

    public async Task<VerifyResponse> Verify(VerifyRequest model)
    {
        var payment = await _unitOfWork.Payments.GetFirstOrDefaultAsync(x => x.Token == model.Token && x.RequestId == model.RequestId);

        if (payment == null)
        {
            var errorMessage = GlobalExceptions.PaymentNotFound.GetDescription();
            _logger.LogError($"{errorMessage}, VerifyRequest: {JsonSerializer.Serialize(model)}");
            return new VerifyResponse
            {
                ErrorCode = (int)GlobalExceptions.PaymentNotFound,
                ErrorMessage = errorMessage,
                IsSuccessful = false,
            };
        }

        //save request
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

        //validations
        if (payment.PaymentState != PaymentState.Pending)
        {
            var errorMessage = GlobalExceptions.InvalidState.GetDescription();
            var errorCode = (int)GlobalExceptions.InvalidState;
            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = errorCode;
            await _unitOfWork.SaveAsync();

            _logger.LogError($"{errorMessage}, PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                IsSuccessful = false,
            };
        }
        if (payment.Amount.ToString() != model.Amount)
        {
            var errorMessage = GlobalExceptions.InvalidAmount.GetDescription();
            var errorCode = (int)GlobalExceptions.InvalidAmount;

            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = errorCode;
            await _unitOfWork.SaveAsync();

            _logger.LogError($"{errorMessage}, PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                IsSuccessful = false,
            };
        }

        if (!_client.IsSuccessful(model.ResponseCode))
        {
            var errorMessage = GlobalExceptions.PaymentFailed.GetDescription();
            var errorCode = (int)GlobalExceptions.PaymentFailed;
            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = errorCode;
            await _unitOfWork.SaveAsync();

            _logger.LogError($"{errorMessage}, PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
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

        //save
        var confirmDetail = new PaymentDetail
        {
            CreatedAt = DateTime.UtcNow,
            IsSuccessful = false,
            PaymentId = payment.Id,
            Request = JsonSerializer.Serialize(request),
            State = PaymentDetailState.Verification,
        };
        _unitOfWork.PaymentDetails.Update(confirmDetail);
        await _unitOfWork.SaveAsync();

        //api
        var response = await _client.ConfirmAsync(request);

        if (response == null)
        {
            var errorMessage = GlobalExceptions.ExternalServiceError.GetDescription();
            var errorCode = (int)GlobalExceptions.ExternalServiceError;

            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = errorCode;
            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveAsync();

            _logger.LogError($"{errorMessage}, PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                IsSuccessful = false,
            };
        }

        if(response.isSuccessful)
        {
            payment.PaymentState = PaymentState.Paid;
            confirmDetail.Response = JsonSerializer.Serialize(response);
            confirmDetail.IsSuccessful = true;
            paymentDetail.IsSuccessful = true;
            _unitOfWork.PaymentDetails.Update(confirmDetail);
            _unitOfWork.PaymentDetails.Update(paymentDetail);
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
        else
        {
            var errorMessage = GlobalExceptions.VerificationFailed.GetDescription();
            var errorCode = (int)GlobalExceptions.VerificationFailed;

            payment.PaymentState = PaymentState.Failed;
            payment.ErrorCode = errorCode;
            confirmDetail.Response = JsonSerializer.Serialize(response);
            _unitOfWork.Payments.Update(payment);
            _unitOfWork.PaymentDetails.Update(confirmDetail);
            await _unitOfWork.SaveAsync();

            _logger.LogError($"{errorMessage}, PaymentId: {payment.Id}");
            return new VerifyResponse
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                IsSuccessful = false,
            };
        }
    }
}
