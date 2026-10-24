using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationPagePermission : PagePermission
    {
        public string PermissionName { get; set; }

        public string PageUrlName { get; set; }
    }
}
