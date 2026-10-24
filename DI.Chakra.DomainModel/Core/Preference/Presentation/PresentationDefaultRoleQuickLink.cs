using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationDefaultRoleQuickLink : DomainModelBase<PresentationDefaultRoleQuickLink, EntityModel.Presentation.PresentationDefaultRoleQuickLink>
    {
        public string Name { get; set; }
        //public string Title { get; set; }
        public string RoleName { get; set; }
        public string Category { get; set; }
        public int Position { get; set; }
        public static IList<PresentationDefaultRoleQuickLink> GetDefaultRoleQLRights()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationDefaultRoleQuickLinkRepository(dataAccessTransaction).GetDefaultRoleQLRights());
            }
        }
    }
}
