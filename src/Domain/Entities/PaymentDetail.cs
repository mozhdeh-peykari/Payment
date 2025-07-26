using Domain.Enums;

namespace Domain.Entities;

public class PaymentDetail
{
    public int Id { get; set; }

    public int PaymentId { get; set; }

    public PaymentDetailStatus Status { get; set; }

    public string Message { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsSuccessful { get; set; }

    public string Request { get; set; }

    public string Response { get; set; }
}
