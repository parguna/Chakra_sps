using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationUserAccessDetail
    {
        public long Id { get; set; }

        public string userName { get; set; }

        public string EndpointSystemName { get; set; }

        public string EndpointHostName { get; set; }

        public long EndpointSystemID { get; set; }

        public string Name { get; set; } 
        
        public string FullName { get; set; }

        public string AccountType { get; set; }

        public string Type { get; set; } 

        public long ParentId { get; set; }

        public Nullable<DateTime> LastLogonDate { get; set; }

        public string Institution { get; set; }

        public string Description { get; set; }      
    }
}
