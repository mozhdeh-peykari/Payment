using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish.Dtos
{
    public class PayResponse
    {
        public string token { get; set; }

        public int acceptorId { get; set; }

        public string responseCode { get; set; }

        public int requestId { get; set; }

        public int amount { get; set; }
    }
}
