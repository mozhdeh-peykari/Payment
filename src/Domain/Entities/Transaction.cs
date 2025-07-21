namespace Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public decimal  Amount { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string RequestId { get; set; }

        public string Token { get; set; }

        public DateTime RequestTime { get; set; }

        public DateTime? ConfirmedTime { get; set; }
    }
}
