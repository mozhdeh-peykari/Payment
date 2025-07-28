namespace Application.IPGServices.Dtos;

public class ConfirmResponse : GeneralResponse<ConfirmResponseDetails>
{
}

public class ConfirmResponseDetails
{
    public string ResponseCode { get; set; }

    public DateTime TransactionDateTime { get; set; }

    public string Amount { get; set; }
}
