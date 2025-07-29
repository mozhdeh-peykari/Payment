namespace Application.IPGServices.Dtos;

public class TokenResponseDto : GeneralResponseDto<TokenResultDto>
{
    public bool IsSuccessful { get; set; }
    public string Token { get; set; }
}

public class TokenResultDto
{
    public string Token { get; set; }

    public int InitiateTimeStamp { get; set; }

    public int ExpiryTimeStamp { get; set; }

    public string TransactionType { get; set; }

    public BillInfoDto BillInfo { get; set; }
}

public class BillInfoDto
{
    public int? BillId { get; set; }

    public int? BillPaymentId { get; set; }
}
