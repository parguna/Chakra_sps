using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Infrastructure;

namespace DeepIdentity.Model.SAP
{
    public class SAPCompositeChildRoleMapInfoHistory : IAggregateRoot
    {
        public string COMPOSITE_ROLE_CODE { get; set; }
        public string CHILD_ROLE_CODE { get; set; }
        public string CHILD_ROLE_CODE_DESCRIPTION { get; set; }
        public string TenantId { get; set; }
        public int EndPointSystemId { get; set; }
        public string SAP_USERID { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LASTMODIFIEDDATE { get; set; }
    }
}
