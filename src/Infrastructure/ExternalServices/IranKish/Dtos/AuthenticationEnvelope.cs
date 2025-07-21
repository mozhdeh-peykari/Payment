using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish.Dtos
{
    public class AuthenticationEnvelope
    {
        public string iv { get; set; }


        public string data { get; set; }
    }
}
