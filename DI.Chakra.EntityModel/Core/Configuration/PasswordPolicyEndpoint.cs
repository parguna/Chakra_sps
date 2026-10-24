using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_PasswordPolicyEndpoint")]
    public class PasswordPolicyEndpoint : EntityBase , ICreatable, IUpdatable
    {
        public long PolicyId { get; set; }

        public long EndpointSystemId { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey("EndpointSystemId")]
        public  EndPoint Endpoint { get; set; }

        [ForeignKey("PolicyId")]
        public  PasswordPolicy PasswordPolicy { get; set; }
    }
}
