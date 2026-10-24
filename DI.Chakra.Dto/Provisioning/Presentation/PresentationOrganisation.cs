using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationOrganisation
    {
        
        public string OrgUnitPrefix { get; set; }
        public string OrganisationName { get; set; }
        public string ParentOrganisation { get; set; }

        public string Scope { get; set; }

        public string DisplayName { get; set; }
    }
}
