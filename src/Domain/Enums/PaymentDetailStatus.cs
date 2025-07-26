namespace Domain.Enums;

public enum PaymentDetailStatus
{
    Initiated,

    TokenGenerationFailed,

    TokenGenerated,

    ReturnedFromGateway,

    VerificationFailed,

    Verified,

    Paid
}
