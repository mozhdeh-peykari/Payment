using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos;

public class ResponseDto<T>
{
    public bool IsSuccessful { get; set; }

    public int ErrorCode { get; set; }

    public string ErrorMessage { get; set; }

    public T? Result { get; set; }
}
