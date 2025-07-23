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
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.Services
{
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
                    transactionType = "Purchase",
                    terminalId = _settings.TerminalId,
                    acceptorId = _settings.AcceptorId,
                    amount = model.Amount,
                    revertUri = model.ReturnUrl,
                    requestId = Guid.NewGuid().ToString("N").Substring(0, 20),
                    requestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                }
            };

            //init transaction into db
            var transaction = new PaymentTransaction
            {
                Amount = model.Amount,
                PaymentStatus = PaymentStatus.Init,
                Token = "",
                ConfirmedTime = default,
                RequestId = default,
                RequestTime = DateTime.UtcNow,
                TerminalId = _settings.TerminalId,
            };
            transaction.AddEvent(eventType: TransactionEventType.Initiated, message: "", parameters: JsonSerializer.Serialize(tokenRequest));
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.SaveAsync();

            //call api
            var token = await _client.GetTokenAsync(tokenRequest);

            //validate token
            //update db
            if (IsTokenValid(token))
            {
                transaction.PaymentStatus = PaymentStatus.Pending;

                transaction.Events.Add(new TransactionEvent
                {
                    EventType = TransactionEventType.TokenGenerated,
                    Message = "Token generation failed",
                    CreatedAt = DateTime.UtcNow,
                });

                await _unitOfWork.Transactions.SaveAsync();

                return token;
            }
            else
            {
                transaction.Events.Add(new TransactionEvent
                {
                    EventType = TransactionEventType.TokenGenerationFailed,
                    Message = "Token generation failed",
                    CreatedAt = DateTime.UtcNow,
                });
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
            if (model.IsValid())
            {

            }
            else
            {

            }

            //get transaction
            var transaction = await _unitOfWork.Transactions.GetFirstOrDefaultAsync(x => x.Token == model.Token && x.RequestId == model.RequestId);
            if (transaction == null)
            {
                throw new Exception($"Transaction not found. Token: {model.Token}, RequestId: {model.RequestId}");
            }

            var request = new ConfirmRequest
            {
                terminalId = _settings.TerminalId,
                tokenIdentity = model.Token,
                retrievalReferenceNumber = 0, //?
                systemTraceAuditNumber = 0 //?
            };

            transaction.AddEvent(eventType: TransactionEventType.Paid, message: "Returned from gateway", parameters: JsonSerializer.Serialize(request));

            var response = await _client.ConfirmAsync(request);

            //map response code to status

            //update
            transaction.PaymentStatus = PaymentStatus.Paid;

            var res = new VerifyResponse
            {
                PaymentStatus = transaction.PaymentStatus,
            };

            return res;
        }
    }
}
