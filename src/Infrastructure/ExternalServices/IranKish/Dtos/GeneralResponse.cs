namespace Infrastructure.ExternalServices.IranKish.Dtos;

public class GeneralResponse<T>
{
    public string responseCode { get; set; }

    public string description { get; set; }

    public bool status { get; set; }

    public T? result { get; set; }

    public bool isSuccessful => responseCode == "00" ? true : false;
}
