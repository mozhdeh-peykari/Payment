using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application;

public enum GlobalExceptions
{
    [Description("Payment status is invalid")]
    InvalidStatus = 100,

}
