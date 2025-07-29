namespace Application.IPGServices.Dtos;

public class GeneralResponseDto<T>
{
    public string ResponseCode { get; set; }

    public string Description { get; set; }

    public bool Status { get; set; }

    public T? Result { get; set; }
}
