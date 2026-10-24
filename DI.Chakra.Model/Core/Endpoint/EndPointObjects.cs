using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Table("DI_M_EndpointObjects")]
    public class EndpointObjects
    {
        [Key]
        public long Id { get; set; }
        public int EndpointSystemTypeId { get; set; }
        public string ObjectName {get;set;}
        public virtual List<DiscoveryProcess> DiscoveryProcess { get; set; }


    }
}
