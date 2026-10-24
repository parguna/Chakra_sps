using System;
using System.Collections.Generic;
using System.Text;

namespace DI.Chakra.Services.EmailingProcessor
{
    public class MailServerConfigInfo 
    {
        public int MailServerId { get; set; }
        public String EmailID { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String HostName { get; set; }
        public String PortNumber { get; set; }
        public bool EnableSSL { get; set; }
        public int Priority { get; set; }
        public String CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public String LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

    }
}
