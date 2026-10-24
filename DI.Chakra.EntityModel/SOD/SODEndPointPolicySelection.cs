using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODEndPointPolicySelection")]
    public class SODEndPointPolicySelection : EntityBase,ICreatable,IUpdatable
    {
        public long SODPolicyExceutionId { get; set; }

        public string EndpointTypeName { get; set; }

        public string LevelName { get; set; }

        public string LevelValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("SODPolicyExceutionId")]
        public SODPolicyExecutionDefinition SODPolicyExecutionDefinition { get; set; }
    }
}
