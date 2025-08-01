﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.IranKish.Dtos
{
    public class ConfirmRequest
    {
        public string terminalId { get; set; }

        public string retrievalReferenceNumber { get; set; }

        public string systemTraceAuditNumber { get; set; }

        public string tokenIdentity { get; set; }
    }
}
