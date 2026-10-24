using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DeepIdentity.Model.SAP
{
    [Table("DI_SAP_USER_ROLE_MAP")]
    public class SAPUserRoleMapInfo : IAggregateRoot
    {
        [Key]
        public int SAP_USER_ROLEID { get; set; }

        public string SAP_ROLE_CODE { get; set; }
        public string TenantId { get; set; }
        public int EndPointSystemId { get; set; }
        public string SAP_USERID { get; set; }
        public string CREATEDBY { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LASTMODIFIEDDATE { get; set; }
    }
}
