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
    [Table("DI_M_ProvisioningFormSAPEP")]
    public class ProvisioningFormSAPEP : IEquatable<ProvisioningFormSAPEP>, IProvisioningForm
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> ProvisioningFormId { get; set; }
        public string UserLogonName { get; set; }
        public Nullable<bool> PswdChangeOnNextLogIn { get; set; }
        public Nullable<bool> AllowPasswordChange { get; set; }
        public Nullable<bool> IsPasswdNeverExpires { get; set; }
        public Nullable<bool> IsAccountDisabled { get; set; }
        public string roles { get; set; } // Roles
        public string Groups { get; set; }
        public string UserLanguage { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public PasswordSource PasswordSource { get; set; }
        public string PasswordExpression { get; set; }

        public Nullable<long> AccountNameLogicId { get; set; }

        [ForeignKey("AccountNameLogicId")]
        public virtual ProvisioningNameConstructor AccountNameConstructor { get; set; }

        public static ProvisioningFormSAPEP CreateProvisioningFormSAPEP()
        {
            ProvisioningFormSAPEP SAPEPProvisioningFormDetails = new ProvisioningFormSAPEP();
            SAPEPProvisioningFormDetails.CreatedDate = DateTime.Now;
            SAPEPProvisioningFormDetails.LastModifiedDate = DateTime.Now;

            return SAPEPProvisioningFormDetails;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #region IEquatable<SAPEPProvisioningFormDetails> Members

        public bool Equals(ProvisioningFormSAPEP other)
        {
            if (other == null)
                return false;

            return this.Id.Equals(other.Id);
        }

        #endregion

        public virtual ProvisioningForm ProvisioningForm { get; set; }
    }
}
