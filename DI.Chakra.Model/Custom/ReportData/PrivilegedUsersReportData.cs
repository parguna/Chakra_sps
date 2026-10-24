using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class PrivilegedUsersReportData
    {
        public string UserName { get; set; }
        public Nullable<DateTime> LastLogonDate { get; set; }
        public string LastLogon { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
