using System.ComponentModel;

namespace Domain.Enums;

public enum PaymentDetailState
{
    [Description("Token generation")]
    TokenGeneration = 20,

    [Description("Returned from gateway")]
    ReturnedFromGateway = 21,

    [Description("Verification")]
    Verification = 22
}
