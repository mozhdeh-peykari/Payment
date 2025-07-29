namespace Application.IPGServices.Dtos
{

    public class TokenRequestDto
    {
        public RequestDto Request { get; set; }

        public AuthenticationEnvelopeDto AuthenticationEnvelope { get; set; }
    }

    public class RequestDto
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
