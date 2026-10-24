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
    public class PresentationSAPEPUserToGroups
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string InternetAddress { get; set; }
        public string Department { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }

        public static PresentationSAPEPUserToGroups ORMap(EntityModel.Presentation.PresentationSAPEPUserToGroups presentationSAPProfileReport)
        {
            if (presentationSAPProfileReport == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPEPUserToGroups, PresentationSAPEPUserToGroups>(presentationSAPProfileReport);
        }

        public static List<PresentationSAPEPUserToGroups> ORMapList(IList<EntityModel.Presentation.PresentationSAPEPUserToGroups> presentationSAPProfileReports)
        {
            return presentationSAPProfileReports.Select(ORMap).ToList();
        }

        public static IList<PresentationSAPEPUserToGroups> GetSAPEPUserToGroupsReportData(long EndpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPEPUserInfoRepository(dataAccessTransaction).GetSAPEPUserToGroupsReportData(EndpointSystemId));
            }
        }
    }
}
