using Application.Dtos;
using Application.Interfaces;
using Domain.Settings;
using Infrastructure.ExternalServices.IranKish;
using Infrastructure.ExternalServices.IranKish.Dtos;
using Infrastructure.ExternalServices.IranKish.Helpers;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentServiceSettings _settings;
        private readonly IIranKishClient _client;

        public PaymentService(IOptions<PaymentServiceSettings> settings,
            IIranKishClient client)
        {
            _settings = settings.Value;
            _client = client;
        }

        public async Task<string> GetTokenAsync(GetTokenRequest model)
        {
            var envelope = CryptoHelper.GenerateAuthenticationEnvelope(_settings.TerminalId, model.Amount, _settings.Password, _settings.PublicKey);

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

            var token = await _client.GetTokenAsync(tokenRequest);

            return token;
        }
    }
}
