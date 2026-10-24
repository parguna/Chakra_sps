using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class AllUsersReportData
    {
        public long UserId { get; set; }
        public string UserName{ get; set; }
        public string DisplayName { get; set; }
        public string OrganisationName { get; set; }
        public string EmailId { get; set; }
        public string BusinessRole { get; set; }
        public string Position { get; set; }
        public Nullable<DateTime> JoinDate { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string tenantId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<DateTime> TeminaionDate { get; set; }

        public string IsUseractive
        {
            get
            {
                if (IsActive == true)
                    return "Yes";
                else
                    return "No";
            }
        }
    }
}
