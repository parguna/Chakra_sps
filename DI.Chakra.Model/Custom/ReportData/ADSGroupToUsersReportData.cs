using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
     public class ADSGroupToUsersReportData
    {
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsAccountDisabled { get; set; }
        public bool IsAccountLocked { get; set; }

    }
}
