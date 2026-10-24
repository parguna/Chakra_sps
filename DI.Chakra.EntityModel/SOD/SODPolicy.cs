using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODPolicy")]
    public class SODPolicy : EntityBase,ICreatable,IUpdatable
    {
        public string SODPolicyName { get; set; }

        public string SODPolicyDescription { get; set; }

        public long PolicyTypeId { get; set; }

        public int BindType { get; set; }

        public  ComplianceType policytype { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


    }
}
