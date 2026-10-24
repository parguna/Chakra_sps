using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class LotusNotesUsersReportData
    {
        public long UserId { get; set; }
        public string ShortName { get; set; } // UserName
        public string Name { get; set; } // FirstName
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string InternetAddress { get; set; }
        public string Department { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string HostName { get; set; }
        public string Institute { get; set; }
        public string IsMapped { get; set; }
        public string Owner { get; set; }
        //public string DisplayName { get; set; }
        //public string HTTPPasswordChangeDate { get; set; }
        
    }
}
