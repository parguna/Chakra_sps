using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PasswordPolicyEndpoint : DtoBase 
    {
        public long PolicyId { get; set; }

        public long EndpointSystemId { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public  EndPoint Endpoint { get; set; }

        public  PasswordPolicy PasswordPolicy { get; set; }
    }
}
