using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODException")]
    public class SODException : EntityBase,ICreatable,IUpdatable
    {
        public string ExceptionCode { get; set; }

        public string ExceptionDesc { get; set; }

        public Nullable<long> RiskOwnerId { get; set; }
        
        [ForeignKey("RiskOwnerId")]
        public  User RiskOwner { get; set; }

        public  List<SODExceptionItem> ExceptionItems { get; set; }

        public  List<SODPolicyExecutionDefinition> SODPolicyExecutionDefinition { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



    }

}
