using Domain.Enums;

namespace Application.Dtos;

public class VerifyResponse
{
    public PaymentState PaymentState { get; set; }

    public DateTime TransactionDate { get; set; }

    public decimal Amount { get; set; }
}
