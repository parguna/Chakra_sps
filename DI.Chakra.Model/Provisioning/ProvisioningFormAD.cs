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
    [Table("DI_M_ProvisioningFormAD")]
    public class ProvisioningFormAD : IEquatable<ProvisioningFormAD>, Model.IProvisioningForm
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> ProvisioningFormId { get; set; }
        public string UserLogonName { get; set; }
        public Nullable<bool> PswdChangeOnNextLogIn { get; set; }
        public Nullable<bool> PasswordCannotChange { get; set; }
        public Nullable<bool> IsPasswdNeverExpires { get; set; }
        public Nullable<bool> IsAccountDisabled { get; set; }
        public Nullable<bool> CreateHomeFolder { get; set; }
        public string AccountContainerRules { get; set; }
        public string UserProfilePath { get; set; }
        public string HomeFolderPath { get; set; }
        public string HomeDrive { get; set; }
        public string Groups { get; set; }

        public PasswordSource PasswordSource { get; set; }
        public string PasswordExpression { get; set; }

        public Nullable<long> AccountNameLogicId { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("AccountNameLogicId")]
        public virtual ProvisioningNameConstructor AccountNameConstructor { get; set; }

        public static ProvisioningFormAD CreateProvisioningRole()
        {
            ProvisioningFormAD adProvisioningFormDetails = new ProvisioningFormAD();
            adProvisioningFormDetails.CreatedDate = DateTime.Now;
            adProvisioningFormDetails.LastModifiedDate = DateTime.Now;

            return adProvisioningFormDetails;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #region IEquatable<ADProvisioningFormDetails> Members

        public bool Equals(ProvisioningFormAD other)
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
