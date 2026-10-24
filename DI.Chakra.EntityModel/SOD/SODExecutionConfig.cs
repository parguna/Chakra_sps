using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    
    [Table("DI_M_SODExecutionConfig")]
    public class SODExecutionConfig : EntityBase,ICreatable,IUpdatable
    {

        public Nullable<long> EndpointSystemTypeId { get; set; }

        public string Level1 { get; set; }

        public string Level2 { get; set; }

        public string Level3 { get; set; }

        public string Level4 { get; set; }

        public Nullable<int> SODRunPolicyId { get; set; }

        public  EndPointSystemType EndpointSystemType { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


    }
}
