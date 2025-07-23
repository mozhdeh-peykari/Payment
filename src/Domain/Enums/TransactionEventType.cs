namespace Domain.Enums;

public enum TransactionEventType
{
    Initiated,

    TokenGenerationFailed,

    TokenGenerated,

    ReturnedFromGateway,

    VerificationFailed,

    Verified,

    Paid
}
