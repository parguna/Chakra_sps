using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_ADS_ComputerGroup")]
    public class ADSComputerGroup : EntityBase,ICreatable,IUpdatable
    {
        
        public long EndpointSystemId { get; set; }

        public string ComputerCN { get; set; }

        public string ComputerDistinguishedName { get; set; }

        public string GroupCN { get; set; }

        public string GroupDistinguishedName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int? Version { get; set; }
    }
}
