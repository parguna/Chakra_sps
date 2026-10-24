using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_NPAC_DesignationInfo")]
    public class NPACDesignationInfo : EntityBase, ICreatable, IUpdatable
    {
        public NPACDesignationInfo()
        {
        }                         
        public long EndPointSystemId { get; set; }
        public string NPACDesignation { get; set; }
        public string HRDesignationName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }       
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }       
    }
}
