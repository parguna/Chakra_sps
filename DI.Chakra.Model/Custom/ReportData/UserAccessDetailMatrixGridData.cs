using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    public class UserAccessDetailMatrixGridData
    {
        [Key]
        public int Id { get; set; }
        public string userName { get; set; }

        public string EndpointSystemName { get; set; }
        public string EndpointHostName { get; set; }
        public long EndpointSystemID { get; set; }
        public string Name { get; set; } // EndPointUserName
        public string FullName { get; set; } // DisplayName
        public string AccountType { get; set; }
        public string Type { get; set; } // User, Group, Role, TCode
        public int ParentId { get; set; }

        public Nullable<DateTime> LastLogonDate { get; set; }
        public string Institution { get; set; }
        public string Description { get; set; }
    }
}
