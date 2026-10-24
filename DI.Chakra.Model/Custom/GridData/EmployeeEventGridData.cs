using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class EmployeeEventGridData
    {
        public long TriggerId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public EmployeeEvents EventType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsRequestCreated { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        public bool IsRejoiningUser { get; set; }
        public string UserDetails { get; set; }
        public string OldUserDetails { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string EmployeeEvent
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(EventType);
            }
        }

        public string RequestCreated
        {
            get
            {
                return IsRequestCreated ? "Yes" : "No";
            }
        }

        public string RejoiningUser
        {
            get
            {
                return IsRejoiningUser ? "Yes" : "No";
            }
        }

        public string AdminCheckRequired
        {
            get
            {
                return IsAdminCheckRequired ? "Yes" : "No";
            }
        }

        public string Active
        {
            get
            {
                return IsActive ? "Yes" : "No";
            }
        }
        
    }
}
