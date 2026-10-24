using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationSAPUserinfo : SAPUserinfo
    {
        public long EndpointUserId { get; set; }

        public string UserName { get; set; }  

        public string FullName { get; set; }

        public DateTime? LastLogonDate { get; set; }

        public long EndpointSystemId { get; set; }

        public string EndPointSystemType { get; set; }

        public string EndpointSystemName { get; set; }

        public string Category { get; set; }

        public string RoleCode { get; set; }

        public string RoleDescription { get; set; }    
    }
}
