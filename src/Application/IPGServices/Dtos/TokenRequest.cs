namespace Application.IPGServices.Dtos
{

    public class TokenRequest
    {
        public Request Request { get; set; }

        public AuthenticationEnvelope AuthenticationEnvelope { get; set; }
    }

    public class Request
    {
        public string TransactionType { get; set; }

        public string TerminalId { get; set; }

        public string AcceptorId { get; set; }

        public long Amount { get; set; }

        public string RevertUri { get; set; }

        public string RequestId { get; set; }

        public long RequestTimestamp { get; set; }
    }
}
