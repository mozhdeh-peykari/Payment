using Domain.Enums;

namespace Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    public decimal  Amount { get; set; }

    public PaymentState PaymentState { get; set; }

    public string RequestId { get; set; }

    public string? Token { get; set; }

    public DateTime CreatedDate { get; set; }

    public string TerminalId { get; set; }

    public string AcceptorId { get; set; }

    public TransactionType Type { get; set; }

    public int? ErrorCode { get; set; }
}
