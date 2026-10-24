using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationOrganisation : EntityBase
    {
        
        public string OrgUnitPrefix { get; set; }
        public string OrganisationName { get; set; }
        public string ParentOrganisation { get; set; }

        public string Scope { get; set; }

        public string DisplayName { get; set; }
    }
}
