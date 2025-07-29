using Application.IPGServices.Dtos;

namespace Application.IPGServices.Interfaces;

public interface IIPGService
{
    Task<TokenResponseDto> GetTokenAsync(TokenRequestDto req);

    Task<ConfirmResponseDto> ConfirmAsync(ConfirmRequestDto req);
}
