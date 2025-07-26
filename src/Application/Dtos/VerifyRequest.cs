using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class VerifyRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string RequestId { get; set; }

    [Required]
    public string AcceptorId { get; set; }

    [Required]
    public string PayResponseCode { get; set; }

    [Required]
    public string Amount { get; set; }

    [Required]
    public string RetrievalReferenceNumber { get; set; }

    [Required]
    public string SystemTraceAuditNumber { get; set; }
}
