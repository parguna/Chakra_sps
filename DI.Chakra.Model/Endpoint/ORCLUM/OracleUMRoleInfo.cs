using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    [Table("DI_Oracle11gUM_RoleInfo")]
    public class OracleUMRoleInfo : IAggregateRoot, IEndpointEntitlementInfo, IComparer<OracleUMRoleInfo>
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string PasswordRequired { get; set; }
        public string AuthenticationType { get; set; }
        public int? Version { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public virtual IList<OracleUMUserInfo> OracleUsers { get; set; }

        [ForeignKey("EndPointSystemId")]
        public virtual EndPoint EndPoint { get; set; }  

        public int Compare(OracleUMRoleInfo x, OracleUMRoleInfo y)
        {
            int cmprValue = -1;
            if (x != null && y != null)
            {

                cmprValue = string.Compare(y.RoleName, x.RoleName);
            }

            return cmprValue;
        }

        public string EntitlementName
        {
            get { return RoleName; }
        }

        public string EntitlementDesc
        {
            get { return Description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }
    }
    
}
