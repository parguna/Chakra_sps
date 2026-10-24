#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace DI.Chakra.Model
{
    [TableInfo("DI_Oracle11gUM_RoleInfoHistory", "Oracle11gUMRoleInfoHistory")]
    [Table("DI_Oracle11gUM_RoleInfoHistory")]
    public class OracleUMRoleInfoHistory:IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public int RoleId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string RoleName { get; set; }
        public string PasswordRequired { get; set; }
        public string AuthenticationType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
