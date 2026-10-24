using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationQuickLinkAccessRights : EntityBase
    {
        public string Name { get; set; }
        //public string Title { get; set; }
        public string Category { get; set; }
        public string RoleName { get; set; }
    }
}
