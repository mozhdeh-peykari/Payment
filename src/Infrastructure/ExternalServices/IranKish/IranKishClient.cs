using Domain.Settings;
using Infrastructure.ExternalServices.IranKish.Dtos;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.ExternalServices.IranKish
{
    public class IranKishClient : IIranKishClient
    {
        private readonly HttpClient _httpClient;
        private readonly PaymentServiceSettings _settings;

        public IranKishClient(HttpClient httpClient,
            IOptions<PaymentServiceSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<TokenResponse> GetTokenAsync(TokenRequest req)
        {
            var response = await _httpClient.PostAsJsonAsync(_settings.Tokenization, req);
            var res = await response.Content.ReadAsStringAsync();
            var serialized = JsonSerializer.Deserialize<TokenResponse>(res);

            return serialized;
        }

        public async Task<ConfirmResponse> ConfirmAsync(ConfirmRequest req)
        {
            var response = await _httpClient.PostAsJsonAsync(_settings.Verify, req);

            return new ConfirmResponse();
        }
    }
}
