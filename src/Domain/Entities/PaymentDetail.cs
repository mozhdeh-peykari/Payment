using Domain.Enums;

namespace Domain.Entities;

public class PaymentDetail : BaseEntity<int>
{
    public int PaymentId { get; set; }

    public PaymentDetailState State { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsSuccessful { get; set; }

    public string Request { get; set; }

    public string? Response { get; set; }
}
