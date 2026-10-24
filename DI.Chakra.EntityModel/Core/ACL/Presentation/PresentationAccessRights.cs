using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class PresentationAccessRights : EntityBase
    {
        public string RoleName { get; set; }
        public string MenuPath { get; set; }
        public string Permissions { get; set; }
    }
}
