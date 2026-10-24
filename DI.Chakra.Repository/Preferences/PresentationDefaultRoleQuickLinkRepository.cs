using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.Repository
{
    public class PresentationDefaultRoleQuickLinkRepository : RepositoryBase<PresentationDefaultRoleQuickLink>
    {

        public PresentationDefaultRoleQuickLinkRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }
        public IList<PresentationDefaultRoleQuickLink> GetDefaultRoleQLRights()
        {
            IList<PresentationDefaultRoleQuickLink> qlRoleList = new List<PresentationDefaultRoleQuickLink>();
            var query = from DR in this.CurrentContext.DefaultRoleQuickLink.Include("QuickLink").Include("Role")
                        select new PresentationDefaultRoleQuickLink
                        {
                            Id = DR.Id,
                            Name = DR.QuickLink.Name,
                            RoleName = DR.Role.RoleName,
                            Category = DR.QuickLink.Category,
                            Position= DR.Position
                        };
            qlRoleList = query.ToList();
            return qlRoleList;
        }
    }
}
