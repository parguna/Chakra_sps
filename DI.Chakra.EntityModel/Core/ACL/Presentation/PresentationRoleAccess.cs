using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationRoleAccess : EntityBase
    {
        public long AccessId { get; set; }
        public string RoleName { get; set; }
        public string PageName { get; set; }
        public string AccessRights { get; set; }

    }
}