using Application.IPGServices.Dtos;

namespace Application.IPGServices.Interfaces;

public interface IIPGService
{
    Task<TokenResponse> GetTokenAsync(TokenRequest req);

    Task<ConfirmResponse> ConfirmAsync(ConfirmRequest req);
}
