using Application.Dtos;

namespace Application.Interfaces;

public interface IPaymentService
{
    Task<GetTokenResponse> GetTokenAsync(GetTokenRequest model);

    Task<VerifyResponse> Verify(VerifyRequest model);
}
