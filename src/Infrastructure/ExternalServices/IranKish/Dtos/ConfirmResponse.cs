using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish.Dtos;

public class ConfirmResponse : GeneralResponse<ConfirmResponseDetails>
{
}

public class ConfirmResponseDetails
{
    public string responseCode { get; set; }

    public DateTime transactionDateTime { get; set; }

    public string amount { get; set; }
}
