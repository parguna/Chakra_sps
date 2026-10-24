using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.Repository
{
    public class PresentationQuickLinkAccessRightsRepository : RepositoryBase<PresentationQuickLinkAccessRights>
    {
        public PresentationQuickLinkAccessRightsRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }
        public IList<PresentationQuickLinkAccessRights> GetQuickLinkAccessRights()
        {
            IList<PresentationQuickLinkAccessRights> dashboardList = new List<PresentationQuickLinkAccessRights>();
            var query = from QR in this.CurrentContext.QuickLinkAccessRights.Include("QuickLink").Include("Role")
                        select new PresentationQuickLinkAccessRights
                        {
                            Id = QR.Id,
                            Name = QR.QuickLink.Name,
                            RoleName = QR.Role.RoleName,
                            Category = QR.QuickLink.Category

                        };
            dashboardList = query.ToList();
            return dashboardList;
        }
    }
}
