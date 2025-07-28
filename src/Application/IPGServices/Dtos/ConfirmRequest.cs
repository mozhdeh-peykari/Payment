namespace Application.IPGServices.Dtos;

public class ConfirmRequest
{
    public string TerminalId { get; set; }

    public string RetrievalReferenceNumber { get; set; }

    public string SystemTraceAuditNumber { get; set; }

    public string TokenIdentity { get; set; }
}

