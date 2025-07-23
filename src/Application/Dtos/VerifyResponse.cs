using Domain.Enums;

namespace Application.Dtos;

public class VerifyResponse
{
    public PaymentStatus PaymentStatus { get; set; }

    public DateTime TransactionDate { get; set; }

    public int Amount { get; set; }

    public bool IsSuccessful()
    {
        return true;
    }
}
