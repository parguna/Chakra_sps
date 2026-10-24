using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationSAPRoleToUsers
    {
        public long RoleId { get; set; }


        public string RoleCode { get; set; }

        public string RoleDescription { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Department { get; set; }


    }
}
