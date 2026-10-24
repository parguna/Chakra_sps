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
    public class PresentationSAPEPUserToRoles
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string InternetAddress { get; set; }
        public string Department { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }

        public static PresentationSAPEPUserToRoles ORMap(EntityModel.Presentation.PresentationSAPEPUserToRoles presentationSAPEPGroupToUsersReport)
        {
            if (presentationSAPEPGroupToUsersReport == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPEPUserToRoles, PresentationSAPEPUserToRoles>(presentationSAPEPGroupToUsersReport);
        }

        public static List<PresentationSAPEPUserToRoles> ORMapList(IList<EntityModel.Presentation.PresentationSAPEPUserToRoles> presentationSAPEPGroupToUsersReports)
        {
            return presentationSAPEPGroupToUsersReports.Select(ORMap).ToList();
        }

        public static List<PresentationSAPEPUserToRoles> GetSAPEPUserToRolesReportData(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPEPUserInfoRepository(dataAccessTransaction).GetSAPEPUserToRolesReportData(endpointSystemId).ToList());
            }
        }
    }
}
