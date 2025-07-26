using Domain.Enums;

namespace Application.Dtos;

public class VerifyResponse : ResponseDto<VerifyResult>
{
}

public class VerifyResult
{
    public PaymentState PaymentState { get; set; }
}
