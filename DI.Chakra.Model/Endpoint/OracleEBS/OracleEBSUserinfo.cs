#region NAMESPACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

#endregion

namespace DI.Chakra.Model
{
    [Table("DI_OEBS_UserInfo")]
    public class OracleEBSUserInfo : IEndpointUserInfo, IAggregateRoot
    {

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public long? DIUserId { get; set; }

        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        //public string ENCRYPTED_FOUNDATION_PASSWORD { get; set; }
        //public string ENCRYPTED_USER_PASSWORD { get; set; }
        public DateTime?LAST_UPDATE_DATE{ get; set; }
        public string LAST_UPDATED_BY{ get; set; }
        public DateTime? CREATION_DATE{ get; set; }
        public string  CREATED_BY{ get; set; }
        public double? SESSION_NUMBER { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime? LAST_LOGON_DATE { get; set; }
        public DateTime? PASSWORD_DATE { get; set; }
        public double? PASSWORD_ACCESSES_LEFT { get; set; }
        public int? PASSWORD_LIFESPAN_ACCESSES { get; set; }
        public double? PASSWORD_LIFESPAN_DAYS { get; set; }
        public double? EMPLOYEE_ID { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string FAX { get; set; }
        public double? CUSTOMER_ID { get; set; }
        public double? SUPPLIER_ID { get; set; }
        //public string WEB_PASSWORD { get; set; }
        public double? SECURITY_GROUP_ID { get; set; }
        //public Guid USER_GUID { get; set; }
        public int? GCN_CODE_COMBINATION_ID { get; set; }
        public int? PERSON_PARTY_ID { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int Version { get; set; }

        public bool IsLocked { get; set; }
        public AccountType AccountType { get; set; }
        public LockScenario LockScenario { get; set; }
        public string AccountName { get { return this.USER_NAME; } set { this.USER_NAME = value; } }

        public string AccountPassword { get; set; }

        public List<string> Roles;
        public List<string> Responsibilities;
        public EndPoint EndPointSystem { get; set; }
        public IList<OracleEBSRoleInfo> RolesInfo { get; set; }
        public IList<OracleEBSResponsibilitiesInfo> ResponsibilitiesInfo { get; set; }

        public OracleEBSUserInfo()
        {
            RolesInfo = new List<OracleEBSRoleInfo>();
            ResponsibilitiesInfo = new List<OracleEBSResponsibilitiesInfo>();
        }
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                return (List<IEndpointEntitlementInfo>)RolesInfo;
            }
        }
        public string MappingType { get; set; }

        public string DisplayName
        {
            get
            {
                return this.AccountName;
            }
        }
    }

}
