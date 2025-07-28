namespace Application.IPGServices.Dtos;

public class TokenResponse : GeneralResponse<TokenResult>
{
}

public class TokenResult
{
    public string token { get; set; }

    public int initiateTimeStamp { get; set; }

    public int expiryTimeStamp { get; set; }

    public string transactionType { get; set; }

    public BillInfo billInfo { get; set; }
}

public class BillInfo
{
    public int? billId { get; set; }

    public int? billPaymentId { get; set; }
}
