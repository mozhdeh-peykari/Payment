using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish.Dtos
{
    public class ConfirmRequest
    {
        public int terminalId { get; set; }

        public int retrievalReferenceNumber { get; set; }

        public int systemTraceAuditNumber { get; set; }

        public string tokenIdentity { get; set; }
    }
}
