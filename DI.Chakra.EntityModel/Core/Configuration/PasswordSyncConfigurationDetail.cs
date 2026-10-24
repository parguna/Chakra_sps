using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_PasswordSyncConfigurationDetail")]
    public class PasswordSyncConfigurationDetail :EntityBase, ICreatable, IUpdatable
    {
        public int ConfigId { get; set; }

        public long EndpointSystemTypeId { get; set; }

        public long EndpointSystemId { get; set; }

        public bool AllowSync { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
