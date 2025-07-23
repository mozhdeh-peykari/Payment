using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Domain.Entities
{
    public class PaymentTransaction
    {
        public PaymentTransaction(decimal amount,
            string terminalId,
            string acceptorId,
            TransactionType type)
        {
            Amount = amount;
            TerminalId = terminalId;
            AcceptorId = acceptorId;
            Type = type;
            PaymentStatus = PaymentStatus.Init;
            RequestId = Guid.NewGuid().ToString("N").Substring(0, 20);
            CreatedDate = DateTime.UtcNow;
            Token = string.Empty;
        }

        public int Id { get; set; }

        public decimal  Amount { get; private set; }

        public PaymentStatus PaymentStatus { get; private set; } //

        public string RequestId { get; private set; }

        public string Token { get; private set; } //

        public DateTime CreatedDate { get; private set; }

        public DateTime? ConfirmedDate { get; private set; } //

        public string TerminalId { get; private set; }

        public string AcceptorId { get; private set; }

        public TransactionType Type { get; set; }

        public List<TransactionEvent> Events { get; private set; } = [];

        public void Tokenized(string token, string parameters)
        {
            Token = token;
            PaymentStatus = PaymentStatus.Pending;
            Events.Add(new TransactionEvent
            {
                EventType = TransactionEventType.TokenGenerated,
                Message = "Token is generated",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void TokenGenerationFailed(string parameters)
        {
            Events.Add(new TransactionEvent
            {
                EventType = TransactionEventType.TokenGenerationFailed,
                Message = "Token generation failed",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void Paid(string parameters)
        {
            PaymentStatus = PaymentStatus.Paid;
            Events.Add(new TransactionEvent
            {
                EventType = TransactionEventType.ReturnedFromGateway,
                Message = "Returned from gateway",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void PaymentFailed(string parameters)
        {
            PaymentStatus = PaymentStatus.Failed;
            Events.Add(new TransactionEvent
            {
                EventType = TransactionEventType.ReturnedFromGateway,
                Message = "Returned from gateway",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void AddEvent(TransactionEventType eventType,
            string message,
            string parameters)
        {
            Events.Add(new TransactionEvent
            {
                EventType = eventType,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                Params = parameters,
            });
        }

        public void VerificationFailed(string parameters)
        {
            PaymentStatus = PaymentStatus.Failed;
            Events.Add(new TransactionEvent
            {
                EventType = TransactionEventType.VerificationFailed,
                Message = "Verification failed",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void Verified(string parameters)
        {
            PaymentStatus = PaymentStatus.Verified;
            Events.Add(new TransactionEvent
            {
                EventType = TransactionEventType.Verified,
                Message = "Verified successfully",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }
    }
}
