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
    public class PresentationSAPRoleWithNoUsers
    {
        public int SAP_ROLEID { get; set; }

        public string ROLE_CODE { get; set; }

        public string ROLE_DESC { get; set; }

        public string ROLETYPE { get; set; }

        public string Institution { get; set; }

        public string HostName { get; set; }


        public static PresentationSAPRoleWithNoUsers ORMap(EntityModel.Presentation.PresentationSAPRoleWithNoUsers presentationSAPRoleWithNoUsers)
        {
            if (presentationSAPRoleWithNoUsers == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPRoleWithNoUsers, PresentationSAPRoleWithNoUsers>(presentationSAPRoleWithNoUsers);
        }

        public static List<PresentationSAPRoleWithNoUsers> ORMapList(IList<EntityModel.Presentation.PresentationSAPRoleWithNoUsers> presentationSAPRoleWithNoUsersReports)
        {
            return presentationSAPRoleWithNoUsersReports.Select(ORMap).ToList();
        }

        public static List<PresentationSAPRoleWithNoUsers> GetSAPRolesWithNoUsers(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPRoleInfoRepository(dataAccessTransaction).GetSAPRolesWithNoUsers(endpointSystemId));
            }
        }

    }
}
