using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PasswordSyncConfiguration : DtoBase
    {
        public bool AllowPasswordSyncThruFilter { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public List<PasswordSyncConfigurationDetail> PasswordSyncConfigurationDetails { get; set; }
    }
}
