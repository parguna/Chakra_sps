using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_PasswordSyncConfiguration")]
    public class PasswordSyncConfiguration
    {

        public PasswordSyncConfiguration()
        {
            this.PasswordSyncConfigurationDetails = new List<PasswordSyncConfigurationDetail>();
        }

        [Key]
        public long Id { get; set; }
        public bool AllowPasswordSyncThruFilter { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public virtual List<PasswordSyncConfigurationDetail> PasswordSyncConfigurationDetails { get; set; }

    }
}
