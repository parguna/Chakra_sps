using DI.Chakra.Model.SOD;
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
    [Table("DI_M_SODException")]
    public class SODException
    {
        [Key]
        public long Id { get; set; }

        public string ExceptionCode { get; set; }
        public string ExceptionDesc { get; set; }
        public Nullable<long> RiskOwnerId { get; set; }


        [ForeignKey("RiskOwnerId")]
        public virtual User RiskOwner { get; set; }

        public virtual List<SodExceptionItem> ExceptionItems { get; set; }
        public virtual List<SODPolicyExceutionDefinition> PolicyExecutionDefinitions { get; set; }

    }

    

}
