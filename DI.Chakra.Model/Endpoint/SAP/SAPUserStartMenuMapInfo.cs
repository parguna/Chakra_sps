using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model.SAP
{
    public class SAPUserStartMenuMapInfo : IAggregateRoot
    {
        public string START_MENU { get; set; }
        public string START_MENU_DESC { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string SAP_USERID { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LASTMODIFIEDDATE { get; set; }
        public int? Version { get; set; }

    }
}
