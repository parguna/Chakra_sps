using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class UsersByEndPointGridData
    {
        public string name { get; set; }
        public long UserId { get; set; }
        public Nullable<bool> SpecialAccount { get; set; }
        public Nullable<DateTime> lastLogon { get; set; }
        public string mailId { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public Nullable<long> DIUserId { get; set; }
        public string currentuser { get; set; }
        public string FullName { get; set; }
        public string HostName { get; set; }
    }
}
