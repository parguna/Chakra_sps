using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class LNSGroupToUsersReportData
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string Category { get; set; }

        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string InternetAddress { get; set; }
        public string JobTitle { get; set; }
        public string HTTPPasswordChangeDate { get; set; }
        public string Department { get; set; }
    }
}
