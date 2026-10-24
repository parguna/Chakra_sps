using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationUserDetails
    {
        public long Id { get; set; }
        public bool FullTimeEmployee { get; set; }
        public string EmployeeId { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public DateTime? JoinDate { get; set; }
        public string TelephoneNumber { get; set; }
    }
}
