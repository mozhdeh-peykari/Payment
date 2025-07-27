using Domain.Settings;
using Infrastructure.ExternalServices.IranKish.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security;
using System.Text.Json;

namespace Infrastructure.ExternalServices.IranKish;

public class IranKishClient : IIranKishClient
{
    private readonly HttpClient _httpClient;
    private readonly PaymentServiceSettings _settings;
    private readonly ILogger<IranKishClient> _logger;

    public IranKishClient(HttpClient httpClient,
        IOptions<PaymentServiceSettings> settings,
        ILogger<IranKishClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _logger = logger;

        _httpClient = ConfigureHttpClientToIgnoreSsl(_httpClient);
    }

    private static HttpClient ConfigureHttpClientToIgnoreSsl(HttpClient httpClient)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        return new HttpClient(handler)
        {
            BaseAddress = httpClient.BaseAddress
        };
    }

    public async Task<TokenResponse> GetTokenAsync(TokenRequest req)
    {
        TokenResponse tokenResponse = null;
        string? res = null;

        try
        {
            var response = await _httpClient.PostAsJsonAsync(_settings.Tokenization, req);
            res = await response.Content.ReadAsStringAsync();
            tokenResponse = JsonSerializer.Deserialize<TokenResponse>(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: $"[IranKishClient].[GetTokenAsync], Request: {req}, Response: {res}");
        }

        return tokenResponse;
    }

    public async Task<ConfirmResponse> ConfirmAsync(ConfirmRequest req)
    {
        ConfirmResponse confirmResponse = null;
        string? res = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_settings.Verify, req);
            res = await response.Content.ReadAsStringAsync();
            confirmResponse = JsonSerializer.Deserialize<ConfirmResponse>(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(exception: ex, message: $"[IranKishClient].[ConfirmAsync], Request: {req}, Response: {res}");
        }

        return confirmResponse;
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
