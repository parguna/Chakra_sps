using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class DeptChangedUserGridData
    {
        //public int TrustedSourceChangeTrackerId { get; set; }
        public long UserId { get; set; }
        //public bool CreateAcnt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organisation { get; set; }
        public string DepartmentName { get; set; }
        //public string NewOrganisation { get; set; }
        public string JobTitle { get; set; }
        public string RequestSource { get; set; }
        public DateTime? JoiningDate { get; set; }
        //spublic string TerminationDate { get; set; }
    }
}
