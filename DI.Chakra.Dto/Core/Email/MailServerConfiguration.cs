using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class MailServerConfiguration : DtoBase
    {
        public string EmailID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string HostName { get; set; }

        public string PortNumber { get; set; }

        public bool EnableSSL { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
