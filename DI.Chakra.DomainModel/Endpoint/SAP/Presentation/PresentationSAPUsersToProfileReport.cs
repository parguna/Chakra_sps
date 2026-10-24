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
    public class PresentationSAPUsersToProfileReport
    {
        public string EndpointSystemType { get; set; }

        public string EndpointSystemName { get; set; }

        public string AccountName { get; set; }

        public string DisplayName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string ExceptionType { get; set; }

        public long? UserId { get; set; }

        public int? Version { get; set; }

        public long EndpointSystemId { get; set; }

        public long EndpointUserId { get; set; }

        public static PresentationSAPUsersToProfileReport ORMap(EntityModel.Presentation.PresentationSAPUsersToProfileReport presentationSAPUsersToProfileReport)
        {
            if (presentationSAPUsersToProfileReport == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPUsersToProfileReport, PresentationSAPUsersToProfileReport>(presentationSAPUsersToProfileReport);
        }

        public static List<PresentationSAPUsersToProfileReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPUsersToProfileReport> presentationSAPUsersToProfileReports)
        {
            return presentationSAPUsersToProfileReports.Select(ORMap).ToList();
        }

        public List<PresentationSAPUsersToProfileReport> GetSAPUserToProfilesReportData(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPUserInfoRepository(dataAccessTransaction).GetSAPUserToProfilesReportData(endpointSystemId));
            }
        }
    }
}
