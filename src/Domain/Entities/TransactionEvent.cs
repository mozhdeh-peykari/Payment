using Domain.Enums;

namespace Domain.Entities
{
    public class TransactionEvent
    {
        public int Id { get; set; }

        public TransactionEventType EventType { get; set; }

        public string Message { get; set; }

        public string ExternalServiceResponseCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Params { get; set; }


        public int PaymentTransactionId { get; set; }

        public PaymentTransaction PaymentTransaction { get; set; }
    }
}
