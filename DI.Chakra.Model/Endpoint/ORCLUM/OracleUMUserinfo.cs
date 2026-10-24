#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DI.Chakra.Infrastructure.Enums;

#endregion

namespace DI.Chakra.Model
{
    [Table("DI_Oracle11gUM_UserInfo")]
    public class OracleUMUserInfo : IEndpointUserInfo,IAggregateRoot
    {
        public OracleUMUserInfo()
        {
            this.Roles = new List<string>();
            this.OracleRoles = new List<OracleUMRoleInfo>();
            OracleTables = new List<OracleUMTableInfo>();
        }

        [Key]
        public long Id { get; set; }

        public string TenantId { get; set; }

        public long EndPointSystemId { get; set; }

        public string UserName { get; set; }

        public int OraUserId { get; set; }

        public string AccountStatus { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LockAt { get; set; }

        public DateTime? ExpiryAt { get; set; }

        public string DefaultTableSpace { get; set; }

        public string Profile { get; set; }

        public string InitialRSRCConsumerGroup { get; set; }

        public string AuthenticationType { get; set; }

        public long? DIUserId { get; set; }

        public bool SpecialAccount{ get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int Version { get; set; }

        public string AccountName { get; set; }

        public bool IsLocked { get; set; }

        public LockScenario LockScenario { get; set; }

        public bool IsUserAccount { get; set; }

        public string AccountPassword { get; set; }

        public AccountType AccountType { get; set; }

        public List<string> Roles;
      
        public virtual IList<OracleUMRoleInfo> OracleRoles { get; set; }

        public virtual EndPoint EndPointSystem { get; set; }

        public List<OracleUMUserRole> OracleUserRoles { get; set; }

        public virtual IList<OracleUMTableInfo> OracleTables { get; set; }

        public override string ToString()
        {
            return this.UserName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is OracleUMUserInfo)
            {
                OracleUMUserInfo other = (OracleUMUserInfo)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.UserName.GetHashCode();
        }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                return (List<IEndpointEntitlementInfo>)OracleRoles;
            }
        }
        public string DisplayName
        {
            get
            {
                return this.AccountName;
            }
        }

        public string MappingType { get; set; }
    }

}
