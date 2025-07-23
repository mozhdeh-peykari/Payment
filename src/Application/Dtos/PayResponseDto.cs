using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class PayResponseDto
    {
        public string Token { get; set; }

        public int AcceptorId { get; set; }

        public string ResponseCode { get; set; }

        public int RequestId { get; set; }

        public int Amount { get; set; }

        public bool IsValid() { return true; }
    }
}
