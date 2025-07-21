using Domain.Settings;
using Infrastructure.ExternalServices.IranKish.Dtos;
using System.Net.Http.Json;

namespace Infrastructure.ExternalServices.IranKish
{
    public class IranKishClient : IIranKishClient
    {
        private readonly HttpClient _httpClient;
        private readonly PaymentServiceSettings _settings;

        public IranKishClient(HttpClient httpClient,
            PaymentServiceSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        }

        public async Task<string> GetTokenAsync(TokenRequest req)
        {
            var response = await _httpClient.PostAsJsonAsync(_settings.Tokenization, req);
            var token = await response.Content.ReadAsStringAsync();

            return token;
        }

        public async Task<ConfirmResponse> ConfirmAsync(ConfirmRequest req)
        {
            var response = await _httpClient.PostAsJsonAsync(_settings.Verify, req);

            return new ConfirmResponse();
        }
    }
}
