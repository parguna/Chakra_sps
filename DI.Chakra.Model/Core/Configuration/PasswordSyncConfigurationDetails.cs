using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_PasswordSyncConfigurationDetail")]
    public class PasswordSyncConfigurationDetail
    {
        [Key]
        public long Id { get; set; }
        public long ConfigId { get; set; }
        public long EndpointSystemTypeId { get; set; }
        public long EndpointSystemId { get; set; }
        public bool AllowSync { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("ConfigId")]
        public PasswordSyncConfiguration PasswordSyncConfiguration { get; set; } 
    }
}
