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
    [Table("DI_M_ProvisioningFormSAP")]
    public class ProvisioningFormSAP : IEquatable<ProvisioningFormSAP>, IProvisioningForm
    {
        [Key]
        public long Id { get; set; }
        public Nullable<long> ProvisioningFormId { get; set; }
        public string roles { get; set; } // Roles { get; set; }
        public string parameters { get; set; } 
        public string start_menu { get; set; }
        public string OutputDevice { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        
        //public string PrinterLongName { get; set; }
        //public string UserEndDate { get; set; }
        public string TimeZone { get; set; }
        public string DefaultUserGroup { get; set; }
        public string NewUser_DecimalNotation { get; set; }
        public string LogonLanguage { get; set; }
        public string FormatCountry { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public SAPUserType UserType { get; set; }

        public PasswordSource PasswordSource { get; set; }
        public string PasswordExpression { get; set; }

        public Nullable<long> AccountNameLogicId { get; set; }

        [ForeignKey("AccountNameLogicId")]
        public virtual ProvisioningNameConstructor AccountNameConstructor { get; set; }

        public static ProvisioningFormSAP CreateProvisioningRole()
        {
            ProvisioningFormSAP SAPProvisioningFormDetails = new ProvisioningFormSAP();
            SAPProvisioningFormDetails.CreatedDate = DateTime.Now;
            SAPProvisioningFormDetails.LastModifiedDate = DateTime.Now;

            return SAPProvisioningFormDetails;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #region IEquatable<SAPProvisioningFormDetails> Members

        public bool Equals(ProvisioningFormSAP other)
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
