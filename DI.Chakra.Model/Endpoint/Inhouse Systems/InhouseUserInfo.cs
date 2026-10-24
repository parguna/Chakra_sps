using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_INH_UserInfo")]
    public class InhouseUserInfo : IXLData, IEndpointUserInfo
    {

        public InhouseUserInfo()
        {
            this.Permissions = new List<InhousePermission>();
        }

        [Key]
        public long Id
        {
            get;
            set;
        }
        [NotMapped]
        public int UserId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string InhouseSystemCode { get; set; }
        public string NRIC { get; set; }
        public string EmployeeId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? AccountCreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedDate { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime? DisabledDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public long? DIUserId { get; set; }

        public AccountType AccountType { get; set; }
        public LockScenario LockScenario { get; set; }
        
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }
        public string Remarks4 { get; set; }
        public string Remarks5 { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int Version { get; set; }
        public string AccountName
        {
            get { return UserName; }
            //set
            //{
            //    UserName = value;
            //}
        }
        public string MappingType { get; set; }
        public string Permission { get; set; }
        public virtual List<InhousePermission> Permissions { get; set; }

        [NotMapped]
        public DateTime? ValidFrom { get; set; }
        [NotMapped]
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

        [ForeignKey("EndPointSystemId")]
        public virtual EndPoint EndPoint { get; set; }

        public string AccountPassword
        {
            get
            {
                return string.Empty;
            }
        }
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();

                foreach (var item in Permissions)
                    li.Add(item);

                return li;
            }
        }

    }
}
