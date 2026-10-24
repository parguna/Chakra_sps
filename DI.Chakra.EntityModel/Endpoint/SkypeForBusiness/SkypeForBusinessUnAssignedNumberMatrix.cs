using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SFB_UnAssignedNumberMatrix")]
    public class SkypeForBusinessUnAssignedNumberMatrix : EntityBase, ICreatable, IUpdatable
    {
        [Required]
        public int Number { get; set; }
        public int Priority { get; set; }
        [Required]
        public bool Assigned { get; set; }
        public bool Reserved { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
