using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationOrganisation
    {

        
        public string OrgUnitPrefix { get; set; }
        public string OrganisationName { get; set; }
        public string ParentOrganisation { get; set; }

        public string Scope { get; set; }

        public string DisplayName { get; set; }

        public static List<PresentationOrganisation> GetAllOrganizationInfo()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationOrganisationRepository(dataAccessTransaction).GetAllOrganizationInfo());
            }
        }
        public static PresentationOrganisation ORMap(EntityModel.Presentation.PresentationOrganisation presentationOrganisation)
        {
            if (presentationOrganisation == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationOrganisation, PresentationOrganisation>(presentationOrganisation);
        }

        public static List<PresentationOrganisation> ORMapList(IList<EntityModel.Presentation.PresentationOrganisation> presentationOrganisation)
        {
            return presentationOrganisation.Select(ORMap).ToList();
        }
    }
}
