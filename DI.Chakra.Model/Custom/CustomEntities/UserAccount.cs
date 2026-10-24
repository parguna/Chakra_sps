using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
   
        [Serializable]
        public class UserAccount
        {
            public long AcountReconcilationId { get; set; }
            public long EndpointUserId { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public Nullable<DateTime> LastLogonDate { get; set; }
            [XmlIgnore]
            public AccountType AccountType { get; set; }
            [XmlIgnore]
            public EndpointType EndPointSystemType { get; set; }
            public string EndpointSystemName { get; set; }
            public long EndpointSystemId { get; set; }
            public string Reserved1 { get; set; }
            public string Reserved2 { get; set; }
            public string Reserved3 { get; set; }
        }
    }

