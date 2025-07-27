namespace Application.Dtos;

public class VerifyRequest
{
    public string Token { get; set; }

    public string RequestId { get; set; }

    public string AcceptorId { get; set; }

    public string ResponseCode { get; set; }

    public string Amount { get; set; }

    public string RetrievalReferenceNumber { get; set; }

    public string SystemTraceAuditNumber { get; set; }
}
