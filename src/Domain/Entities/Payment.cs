using Domain.Enums;

namespace Domain.Entities;

public class Payment : BaseEntity<int>
{
    public decimal  Amount { get; set; }

    public PaymentState PaymentState { get; set; }

    public string RequestId { get; set; }

    public string? Token { get; set; }

    public DateTime CreatedDate { get; set; }

    public string TerminalId { get; set; }

    public int? ErrorCode { get; set; }
}
