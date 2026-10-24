using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class UserPasswordCountGridData
    {
        public string UserName { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public string LoginID { get; set; }
        public string EndpointSystem { get; set; }
        public int PasswordCount { get; set; }
    }
}
