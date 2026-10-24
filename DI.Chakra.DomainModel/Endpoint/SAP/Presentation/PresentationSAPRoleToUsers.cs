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
     public class PresentationSAPRoleToUsers
    {
        public long RoleId { get; set; }

        public string RoleCode { get; set; }

        public string RoleDescription { get; set; }

        public string Role_FromDate { get; set; }

        public string Role_ToDate { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Department { get; set; }
    


        public static PresentationSAPRoleToUsers ORMap(EntityModel.Presentation.PresentationSAPRoleToUsers presentationSAPUsersToRolesReport)
        {
            if (presentationSAPUsersToRolesReport == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPRoleToUsers, PresentationSAPRoleToUsers>(presentationSAPUsersToRolesReport);
        }

        public static List<PresentationSAPRoleToUsers> ORMapList(IList<EntityModel.Presentation.PresentationSAPRoleToUsers> presentationSAPUsersToRolesReports)
        {
            return presentationSAPUsersToRolesReports.Select(ORMap).ToList();
        }

        public static IList<PresentationSAPRoleToUsers> GetSAPRoleToUsersReportData(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPRoleInfoRepository(dataAccessTransaction).GetSAPRolesToUsers(endPointSystemId));
            }

        }


    }
}
