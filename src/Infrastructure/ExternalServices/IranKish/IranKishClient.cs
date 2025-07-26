using Domain.Settings;
using Infrastructure.ExternalServices.IranKish.Dtos;
using Infrastructure.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security;
using System.Text.Json;

namespace Infrastructure.ExternalServices.IranKish
{
    public class IranKishClient : IIranKishClient
    {
        private readonly HttpClient _httpClient;
        private readonly PaymentServiceSettings _settings;
        private readonly ILogger _logger;

        public IranKishClient(HttpClient httpClient,
            IOptions<PaymentServiceSettings> settings,
            ILogger logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _logger = logger;
        }

        public async Task<TokenResponse> GetTokenAsync(TokenRequest req)
        {
            string? res = null;
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_settings.Tokenization, req);
                res = await response.Content.ReadAsStringAsync();

                _logger.Info("[IranKishClient].[GetTokenAsync]", new { Request = req, Response = res });
            }
            catch (Exception ex)
            {
                _logger.Error("[IranKishClient].[GetTokenAsync]", ex, new { Request = req, Response = res });
            }

            if (res == null)
            {
                return null;
            }

            var deserialized = JsonSerializer.Deserialize<TokenResponse>(res);
            return deserialized;
        }

        public async Task<ConfirmResponse> ConfirmAsync(ConfirmRequest req)
        {
            string? res = null;
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_settings.Verify, req);
                res = await response.Content.ReadAsStringAsync();

                _logger.Info("[IranKishClient].[ConfirmAsync]", new { Request = req, Response = res });
            }
            catch (Exception ex)
            {
                _logger.Error("[IranKishClient].[ConfirmAsync]", ex, new { Request = req, Response = res });
            }

            if (res == null)
            {
                return null;
            }

            var deserialized = JsonSerializer.Deserialize<ConfirmResponse>(res);
            return deserialized;
        }

        public bool IsSuccessful(string responseCode)
        {
            if (responseCode == "00")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
