using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_ProvisioningFormORCLUM")]
    public class ProvisioningFormORCLUM : IEquatable<ProvisioningFormORCLUM>, IProvisioningForm
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> ProvisioningFormId { get; set; }
        public string UserLogonName { get; set; }
        public Nullable<bool> PswdChangeOnNextLogIn { get; set; }
        public Nullable<bool> AllowPasswordChange { get; set; }
        public Nullable<bool> IsPasswdNeverExpires { get; set; }
        public Nullable<bool> IsAccountDisabled { get; set; }
        public string AccountContainerRules { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string Roles { get; set; }
        public string MailTemplateName { get; set; }
        public string MailACLManager { get; set; }
        public string CertIdFilePassword { get; set; }
        public string AssignedPolicy { get; set; }
        public bool IdmPolicy { get; set; }
        public bool CheckPassword { get; set; }
        public int PasswordChangeInterval { get; set; }
        public int PasswordGracePeriod { get; set; }
        public bool MailChargeable { get; set; }
        public bool AllowForeignSync { get; set; }
        public bool ForceUserToChangePwdOnNextLogin { get; set; }
        public string Comments { get; set; }

        public PasswordSource PasswordSource { get; set; }
        public string PasswordExpression { get; set; }

        public static ProvisioningFormORCLUM CreateProvisioningFormLNS()
        {
            ProvisioningFormORCLUM OracleProvisioningFormDetails = new ProvisioningFormORCLUM();
            OracleProvisioningFormDetails.CreatedDate = DateTime.Now;
            OracleProvisioningFormDetails.LastModifiedDate = DateTime.Now;
            return OracleProvisioningFormDetails;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #region IEquatable<OracleProvisioningFormDetails> Members

        public bool Equals(ProvisioningFormORCLUM other)
        {
            if (other == null)
                return false;

            return this.Id.Equals(other.Id);
        }

        #endregion

        public virtual ProvisioningNameConstructor AccountNameConstructor { get; set; }

        public bool IsPasswordEnabled { get; set; }

        public virtual ProvisioningForm ProvisioningForm { get; set; }
    }
}
