using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish.Dtos
{
    public class TokenRequest
    {
        public Request request { get; set; }

        public AuthenticationEnvelope authenticationEnvelope { get; set; }
    }

    public class Request
    {
        public string transactionType { get; set; }

        public string terminalId { get; set; }

        public string acceptorId { get; set; }

        public long amount { get; set; }

        public string revertUri { get; set; }

        public string requestId { get; set; }

        public long requestTimestamp { get; set; }
    }
}
