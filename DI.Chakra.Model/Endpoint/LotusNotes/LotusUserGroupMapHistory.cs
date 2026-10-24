using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model.LotusNotes
{
    [TableInfo("DI_LNS_UserGroupMapHistory", "DI_LNS_UserGroupMapHistory")]
    public class LotusUserGroupMapHistory : IAggregateRoot
    {
        public string LNS_Group { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string LNS_USERID { get; set; }
        public string LNS_EmailId { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LASTMODIFIEDDATE { get; set; }
    }
}
