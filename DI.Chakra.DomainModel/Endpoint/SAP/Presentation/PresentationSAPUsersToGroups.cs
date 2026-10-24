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
     public class PresentationSAPHANAUsersToGroups
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string IsActive { get; set; }
        public string GroupCode { get; set; }
        public string GroupDescription { get; set; }

        public static PresentationSAPHANAUsersToGroups ORMap(EntityModel.Presentation.PresentationSAPHANAUsersToGroups presentationADSOUsReportDataEntityModel)
        {
            if (presentationADSOUsReportDataEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPHANAUsersToGroups, PresentationSAPHANAUsersToGroups>(presentationADSOUsReportDataEntityModel);
        }

        public static List<PresentationSAPHANAUsersToGroups> ORMapList(IList<EntityModel.Presentation.PresentationSAPHANAUsersToGroups> presentationADSOUsReportDataEntityModels)
        {
            return presentationADSOUsReportDataEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationSAPHANAUsersToGroups> GetSAPUserToGroupsReportData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPHANAUserInfoRepository(dataAccessTransaction).GetSAPUserToGroupsReportData());
            }
        }
    }
}
