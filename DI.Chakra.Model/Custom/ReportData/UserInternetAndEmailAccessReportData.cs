using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class UserInternetAndEmailAccessReportData
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string DepartmentName { get; set; }
        public string InternetAccess { get; set; }
        public string EmailAccess { get; set; }
        public string IsExceutiveEmployee { get; set; }
    }
}
