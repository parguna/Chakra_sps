using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
namespace DI.Chakra.EntityModel
{
    [Table("DI_VPN_ApplicationEnviornment")]
    public class VPNApplicationEnvironment : EntityBase, ICreatable, IUpdatable
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public long MatrixId { get; set; }
        [Required]
        public VPNMatrixType MatrixType { get; set; }
        [Required]
        public VPNMatrixEnviornmentType EnviornmentType { get; set; }
        public string ADGroups { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
