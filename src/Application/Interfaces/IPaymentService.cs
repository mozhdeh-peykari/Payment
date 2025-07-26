using Application.Dtos;
using Infrastructure.ExternalServices.IranKish.Dtos;

namespace Application.Interfaces;

public interface IPaymentService
{
    Task<string> GetTokenAsync(GetTokenRequest model);

    Task<VerifyResponse> Verify(VerifyRequest model);
}
