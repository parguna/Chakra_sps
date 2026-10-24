using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class AdHocGroupManagementGridData
    {
        public long GroupId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool CanRequest { get; set; }
    }
}
