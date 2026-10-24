using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationQuickLinkAccessRights : DomainModelBase<PresentationQuickLinkAccessRights, EntityModel.Presentation.PresentationQuickLinkAccessRights>
    {
        public string Name { get; set; }
        //public string Title { get; set; }
        public string RoleName { get; set; }
        public string Category { get; set; }

        public static IList<PresentationQuickLinkAccessRights> GetQuickLinkAccessRights()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationQuickLinkAccessRightsRepository(dataAccessTransaction).GetQuickLinkAccessRights());
            }
        }
    }
}
