using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_ProvisioningFormGeneralData")]
    public class ProvisioningFormGeneralData : IEquatable<ProvisioningFormGeneralData>
    {
        [Key]
        public long Id { get; set; }
        public long ProvisioningFormId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeColumnName { get; set; }
        public Nullable<bool> IsGroup { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static ProvisioningFormGeneralData CreateProvisioningFormGeneralData()
        {
            ProvisioningFormGeneralData provisioningFormGD = new ProvisioningFormGeneralData();
            provisioningFormGD.CreatedDate = DateTime.Now;
            provisioningFormGD.LastModifiedDate = DateTime.Now;

            return provisioningFormGD;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        #region IEquatable<ProvisioningFormGeneralData> Members

        public bool Equals(ProvisioningFormGeneralData other)
        {
            if (other == null)
                return false;

            return this.Id.Equals(other.Id);
        }

        #endregion

    }
}
