using Infrastructure.ExternalServices.IranKish.Dtos;

namespace Infrastructure.ExternalServices.IranKish;

public interface IIranKishClient
{
    Task<TokenResponse> GetTokenAsync(TokenRequest req);

    Task<ConfirmResponse> ConfirmAsync(ConfirmRequest req);

    bool IsSuccessful(string responseCode);
}
