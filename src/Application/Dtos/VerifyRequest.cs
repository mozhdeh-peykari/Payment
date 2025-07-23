using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class VerifyRequest
    {
        public string Token { get; set; }

        public string RequestId { get; set; }

        public string? AcceptorId { get; set; }

        public string? ResponseCode { get; set; }

        public string? Amount { get; set; }

        public bool IsValid() { return true; }
    }
}
