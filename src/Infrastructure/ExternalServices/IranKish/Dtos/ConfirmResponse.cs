using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish.Dtos
{
    public class ConfirmResponse
    {
        public string responseCode { get; set; }

        public int transactionDate { get; set; }

        public int transactionTime { get; set; }

        public int amount { get; set; }
    }
}
