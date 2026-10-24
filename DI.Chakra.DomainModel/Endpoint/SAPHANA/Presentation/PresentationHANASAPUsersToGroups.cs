using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
     public class PresentationSAPUsersToGroups
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string IsActive { get; set; }
        public string GroupCode { get; set; }
        public string GroupDescription { get; set; }

        public static PresentationSAPUsersToGroups ORMap(EntityModel.Presentation.PresentationSAPUsersToGroups presentationADSOUsReportDataEntityModel)
        {
            if (presentationADSOUsReportDataEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPUsersToGroups, PresentationSAPUsersToGroups>(presentationADSOUsReportDataEntityModel);
        }

        public static List<PresentationSAPUsersToGroups> ORMapList(IList<EntityModel.Presentation.PresentationSAPUsersToGroups> presentationADSOUsReportDataEntityModels)
        {
            return presentationADSOUsReportDataEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationSAPUsersToGroups> GetSAPUserToGroupsReportData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPUserInfoRepository(dataAccessTransaction).GetSAPUserToGroupsReportData());
            }
        }
    }
}
