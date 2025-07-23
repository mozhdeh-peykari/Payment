using Domain.Enums;

namespace Domain.Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public decimal  Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string RequestId { get; set; }

        public string Token { get; set; }

        public DateTime RequestTime { get; set; }

        public DateTime? ConfirmedTime { get; set; }

        public string TerminalId { get; set; }

        public List<TransactionEvent> Events { get; private set; } = [];

        public void AddEvent(TransactionEventType eventType, string message, string parameters)
        {
            Events.Add(new TransactionEvent
            {
                EventType = eventType,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }
    }
}
