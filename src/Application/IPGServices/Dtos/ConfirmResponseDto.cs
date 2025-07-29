namespace Application.IPGServices.Dtos;

public class ConfirmResponseDto : GeneralResponseDto<ConfirmResponseDetailsDto>
{
    public bool IsSuccessful { get; set; }
    public string Amount { get; set; }
}

public class ConfirmResponseDetailsDto
{
    public string ResponseCode { get; set; }

    public DateTime TransactionDateTime { get; set; }

    public string Amount { get; set; }
}
