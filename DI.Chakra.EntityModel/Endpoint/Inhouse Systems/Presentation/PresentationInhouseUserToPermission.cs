using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationInhouseUserToPermission
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescription { get; set; }
    }
}
