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
    [Table("DI_M_SODRunPolicySelection")]

    public class SODRunPolicySelection
    {
        [Key]
        public long Id { get; set; }
        public long SODRunPolicyId { get; set; }
        public string EndpointTypeName { get; set; }
        public string LevelName { get; set; }
        public string LevelValue { get; set; }

        
    }
}
