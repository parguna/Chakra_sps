using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class ExUserToDistributionReportData
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string mail { get; set; }
        public string department { get; set; }
        public Nullable<DateTime> AccountCreatedDate { get; set; }

        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
    }
}
