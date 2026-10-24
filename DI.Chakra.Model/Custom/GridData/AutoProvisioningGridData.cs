using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class AutoProvisioningGridData
    {
        public int UserId { get; set; }
        public bool CreateAcnt { get; set; }
        public bool IsNewUser { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public string JoiningDate { get; set; }
    }
}
