namespace Domain.Entities;

public class Log
{
    public int Id { get; set; }

    public string PaymentTransactionId { get; set; }

    public string Event { get; set; }

    public string Message { get; set; }

    public DateTime CreatedAt { get; set; }
}
