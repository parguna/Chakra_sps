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
    [Table("DI_M_SODPolicy")]
    public class SODPolicy
    {
        [Key]
        public long Id { get; set; }
        public string SODPolicyName { get; set; }
        public string SODPolicyDescription { get; set; }
        public long PolicyTypeId { get; set; }
        public int BindType { get; set; }

        public virtual  ComplianceType policytype { get; set; }

    }

}
