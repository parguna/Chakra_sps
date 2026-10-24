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
    [Table("DI_M_ProvisioningRoleDetail")]
    public class ProvisioningRoleDetail
    {
        [Key]
        public long Id { get; set; }
        public long ProvisioingRoleId { get; set; }
        public long EndpointSystemId { get; set; }
        public string Entitlements { get; set; }
        [ForeignKey("ProvisioingRoleId")]
        public ProvisioningRole ProvisioningRole { get; set; }
    }
}
