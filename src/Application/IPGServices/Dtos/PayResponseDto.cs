namespace Application.IPGServices.Dtos;

public class PayResponseDto
{
    public string Token { get; set; }

    public int AacceptorId { get; set; }

    public string ResponseCode { get; set; }

    public int RequestId { get; set; }

    public int Amount { get; set; }
}
