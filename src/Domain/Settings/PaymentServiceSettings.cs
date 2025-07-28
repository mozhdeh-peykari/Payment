namespace Domain.Settings
{
    public class PaymentServiceSettings
    {
        public string BaseUrl { get; set; }

        public string RedirectToGateway { get; set; }

        public string Tokenization { get; set; }

        public string Verify { get; set; }

        public string Password { get; set; }

        public string TerminalId { get; set; }

        public string AcceptorId { get; set; }

        public string PublicKey { get; set; }

        public string CallbackUrl { get; set; }

        public string TransactionType { get; set; }
    }
}
