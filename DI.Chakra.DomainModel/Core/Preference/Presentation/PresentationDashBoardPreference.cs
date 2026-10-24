using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationDashBoardPreference :DomainModelBase<PresentationDashBoardPreference,EntityModel.Presentation.PresentationDashBoardPreference>
    {
        public long DashboardId { get; set; }
        //public string Title { get; set; }
        public string DashboardName { get; set; }
        public string DashboardCategory { get; set; }
        public int Position { get; set; }
        public string TargetUrl { get; set; }
        public string NavigateUrl { get; set; }
        public DashboardType DashBoardType { get; set; }

        public static List<PresentationDashBoardPreference> GetUserDashboards(long userId, long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationDashBoardPreferenceRepository(dataAccessTransaction).GetUserDashboards(userId, roleId));
            }
        }

        public static List<PresentationDashBoardPreference> GetDefaultDashboards(long roleId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationDashBoardPreferenceRepository(dataAccessTransaction).GetDefaultDashboards(roleId));
            }
        }

        public static PresentationDashBoardPreference ORMap(EntityModel.Presentation.PresentationDashBoardPreference presentationDashBoardPreference)
        {
            if (presentationDashBoardPreference == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationDashBoardPreference, PresentationDashBoardPreference>(presentationDashBoardPreference);
        }

        public static List<PresentationDashBoardPreference> ORMapList(IList<EntityModel.Presentation.PresentationDashBoardPreference> presentationDashBoardPreference)
        {
            return presentationDashBoardPreference.Select(ORMap).ToList();
        }
    }
}
