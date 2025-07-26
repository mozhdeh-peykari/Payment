using Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Domain.Entities
{
    public class Payment
    {
        public Payment(decimal amount,
            string terminalId,
            string acceptorId,
            PaymentType type)
        {
            Amount = amount;
            TerminalId = terminalId;
            AcceptorId = acceptorId;
            Type = type;
            PaymentState = PaymentState;
            RequestId = Guid.NewGuid().ToString("N").Substring(0, 20);
            CreatedDate = DateTime.UtcNow;
            Token = string.Empty;
        }

        public int Id { get; set; }

        public decimal  Amount { get; private set; }

        public PaymentState PaymentState { get; set; }

        public string RequestId { get; private set; }

        public string Token { get; private set; } //

        public DateTime CreatedDate { get; private set; }

        public DateTime? ConfirmedDate { get; private set; } //

        public string TerminalId { get; private set; }

        public string AcceptorId { get; private set; }

        public PaymentType Type { get; set; }

        public int ErrorCode { get; set; }

        //public List<PaymentDetail> Events { get; private set; } = [];
        /*
        public void Tokenized(string token, string parameters)
        {
            Token = token;
            PaymentState = PaymentState.Pending;
            Events.Add(new PaymentDetail
            {
                EventType = PaymentDetailStatus.TokenGenerated,
                Message = "Token is generated",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void TokenGenerationFailed(string parameters)
        {
            Events.Add(new PaymentDetail
            {
                EventType = PaymentDetailStatus.TokenGenerationFailed,
                Message = "Token generation failed",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void Paid(string parameters)
        {
            PaymentState = PaymentState.Paid;
            Events.Add(new PaymentDetail
            {
                EventType = PaymentDetailStatus.ReturnedFromGateway,
                Message = "Returned from gateway",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void PaymentFailed(string parameters)
        {
            PaymentState = PaymentState.Failed;
            Events.Add(new PaymentDetail
            {
                EventType = PaymentDetailStatus.ReturnedFromGateway,
                Message = "Returned from gateway",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void AddEvent(PaymentDetailStatus eventType,
            string message,
            string parameters)
        {
            Events.Add(new PaymentDetail
            {
                EventType = eventType,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                Params = parameters,
            });
        }

        public void VerificationFailed(string parameters)
        {
            PaymentState = PaymentState.Failed;
            Events.Add(new PaymentDetail
            {
                EventType = PaymentDetailStatus.VerificationFailed,
                Message = "Verification failed",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }

        public void Verified(string parameters)
        {
            //PaymentState = PaymentState.Verified;
            Events.Add(new PaymentDetail
            {
                EventType = PaymentDetailStatus.Verified,
                Message = "Verified successfully",
                CreatedAt = DateTime.UtcNow,
                Params = parameters
            });
        }
        */
    }
}
