namespace Infrastructure.ExternalServices.IranKish.Dtos;

public class TokenResponse
{
    public string responseCode { get; set; }

    public string description { get; set; }

    public bool status { get; set; }

    public object result { get; set; }
}
