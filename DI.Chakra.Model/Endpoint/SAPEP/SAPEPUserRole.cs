using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model
{
    [TableInfo("DI_SAPEP_UserRole", "DI_SAPEP_UserRole")]
    public class SAPEPUserRole : IAggregateRoot
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int? Version { get; set; }

    }
}
