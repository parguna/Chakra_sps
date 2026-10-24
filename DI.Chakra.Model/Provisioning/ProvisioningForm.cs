using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_ProvisioningForm")]
    public class ProvisioningForm : IEquatable<ProvisioningForm>
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public Nullable<bool> IsCreateAccountForm { get; set; }
        public string FormName { get; set; }
        public string FormDescritpion { get; set; }
        public long EndpointSystemId { get; set; }
        public AccountType AccountType { get; set; }
        public Nullable<int> EndpointSystem { get; set; }
        public string FormExecutorTypeName { get; set; }
        public object FormDetails;
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public List<ProvisioningFormGeneralData> FormGeneralData { get; set; }
        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint EndPoint { get; set; }

        public static ProvisioningForm CreateProvisioningForm()
        {
            ProvisioningForm provisioningForm = new ProvisioningForm();
            provisioningForm.CreatedDate = DateTime.Now;
            provisioningForm.LastModifiedDate = DateTime.Now;

            return provisioningForm;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.TenantId.GetHashCode() ^ this.FormName.GetHashCode();
        }

        #region IEquatable<ProvisioningForm> Members

        public bool Equals(ProvisioningForm other)
        {
            if (other == null)
                return false;

            return this.Id.Equals(other.Id);
        }

        #endregion

    }
}
