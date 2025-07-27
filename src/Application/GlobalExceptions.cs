using System.ComponentModel;

namespace Application;

public enum GlobalExceptions
{
    [Description("Payment state is invalid")]
    InvalidState = 100,

    [Description("Token generation failed")]
    TokenGenerationFailed = 101,

    [Description("Payment is not found")]
    PaymentNotFound = 102,

    [Description("Amount is invalid")]
    InvalidAmount = 103,

    [Description("Payment failed")]
    PaymentFailed = 104,

    [Description("Verification failed")]
    VerificationFailed = 105,

    [Description("Unexpected error")]
    UnexpectedError = 106,

    [Description("External service is not available")]
    ExternalServiceError = 107,
}
