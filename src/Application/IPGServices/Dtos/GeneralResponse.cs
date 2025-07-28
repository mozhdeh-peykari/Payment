namespace Application.IPGServices.Dtos;

public class GeneralResponse<T>
{
    public string ResponseCode { get; set; }

    public string Description { get; set; }

    public bool Status { get; set; }

    public T? Result { get; set; }

    public bool IsSuccessful => ResponseCode == "00" ? true : false;
}
