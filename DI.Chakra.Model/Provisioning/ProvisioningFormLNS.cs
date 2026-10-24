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
    [Table("DI_M_ProvisioningFormLNS")]
    public class ProvisioningFormLNS : IEquatable<ProvisioningFormLNS>, IProvisioningForm
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> ProvisioningFormId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string Groups { get; set; }

        //public string CertifierFilePath { get; set; }
        //public string AccountNamepattern { get; set; }
        public string MailTemplateName { get; set; }
        public string MailACLManager { get; set; }
        //public string UserIdFilePath { get; set; }
        public string CertIdFilePassword { get; set; }
        //public Nullable<int> MailQuotaSizeLimit { get; set; }
        //public Nullable<int> MailQuotaWarningThreshold { get; set; }

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

        public Nullable<long> ShortNameLogicId { get; set; }
        public Nullable<long> LongNameLogicId { get; set; }
        public Nullable<long> MailFileLogicId { get; set; }
        public Nullable<long> UserIdFileLogicId { get; set; }
        public Nullable<long> InternetAddressNameLogicId { get; set; }

        [ForeignKey("ShortNameLogicId")]
        public virtual ProvisioningNameConstructor ShortNameConstructor { get; set; }

        [ForeignKey("LongNameLogicId")]
        public virtual ProvisioningNameConstructor LongNameConstructor { get; set; }

        [ForeignKey("MailFileLogicId")]
        public virtual ProvisioningNameConstructor MailFileNameConstructor { get; set; }

        [ForeignKey("UserIdFileLogicId")]
        public virtual ProvisioningNameConstructor UserIdFileNameConstructor { get; set; }

        [ForeignKey("InternetAddressNameLogicId")]
        public virtual ProvisioningNameConstructor InternetAdderssNameConstructor { get; set; }


        public static ProvisioningFormLNS CreateProvisioningFormLNS()
        {
            ProvisioningFormLNS LNSProvisioningFormDetails = new ProvisioningFormLNS();
            LNSProvisioningFormDetails.CreatedDate = DateTime.Now;
            LNSProvisioningFormDetails.LastModifiedDate = DateTime.Now;

            return LNSProvisioningFormDetails;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #region IEquatable<ADProvisioningFormDetails> Members

        public bool Equals(ProvisioningFormLNS other)
        {
            if (other == null)
                return false;

            return this.Id.Equals(other.Id);
        }

        #endregion



        [ForeignKey("ProvisioningFormId")]
        public virtual ProvisioningForm ProvisioningForm { get; set; }
    }
}
