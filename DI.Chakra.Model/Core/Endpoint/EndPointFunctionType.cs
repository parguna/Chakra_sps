using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.Core.Endpoint
{
    [Serializable]
    [Table("DI_M_EndpointFunctionType")]
    public class EndPointFunctionType
    {
        [Key]
        public long Id { get; set; }
        public string Description { get; set; }
        public Nullable<long> EndpointSystemTypeId { get; set; }

        [ForeignKey("EndpointSystemTypeId")]
        public virtual EndPointSystemType EndPointSystemType { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
