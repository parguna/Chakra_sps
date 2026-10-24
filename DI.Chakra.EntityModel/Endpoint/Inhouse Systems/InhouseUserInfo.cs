using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_INH_UserInfo")]
    public class InhouseUserInfo : EntityBase,ICreatable,IUpdatable
    {
        public long EndPointSystemId { get; set; }

        public string InhouseSystemCode { get; set; }

        public string IdentificationType { get; set; }
        public string IdentificationCountry { get; set; }
        public string EmployeeId { get; set; }
        public string NRIC { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? AccountCreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LockedDate { get; set; }
        public string LockScenario { get; set; }
        public AccountType AccountType { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDisabled { get; set; }

        public DateTime? DisabledDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string Remarks1 { get; set; }

        public string Remarks2 { get; set; }

        public string Remarks3 { get; set; }

        public string Remarks4 { get; set; }

        public string Remarks5 { get; set; }
        [NotMapped]
        public string IsMapped { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public virtual List<InhousePermission> Permissions { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }

        [NotMapped]
        public string Permission { get; set; }
        [NotMapped]
        public string AccountName
        {
            get { return UserName; }
            set { UserName = value; }
        }
    }
    
}
