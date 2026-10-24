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
    [Table("DI_M_SODExecutionConfig")]
    public class SODExecutionConfig
    {
        [Key]
        public long Id { get; set; }

        public Nullable<long> EndpointSystemTypeId { get; set; }
        public string Level1 { get; set; }
        public string Level2 { get; set; }
        public string Level3 { get; set; }
        public string Level4 { get; set; }
        public Nullable<int> SODRunPolicyId { get; set; }
        public virtual EndPointSystemType EndpointSystemType { get; set; }
    }
}
