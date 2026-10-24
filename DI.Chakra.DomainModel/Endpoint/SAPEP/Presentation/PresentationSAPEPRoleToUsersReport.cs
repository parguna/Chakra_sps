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
    public class PresentationSAPEPRoleToUsersReport
    {
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }

        public long UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string InternetAddress { get; set; }
        public string JobTitle { get; set; }
        public Nullable<DateTime> IACMPasswordChangeDate { get; set; }
        public string Department { get; set; }

        public static List<PresentationSAPEPRoleToUsersReport> GetByEndpointSystemId(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPEPRoleInfoRepository(dataAccessTransaction).GetByEndpointSystemId(endpointSystemId).ToList());
            }
        }

        public static PresentationSAPEPRoleToUsersReport ORMap(EntityModel.Presentation.PresentationSAPEPRoleToUsersReport presentationSAPEPRoleTOUserReport)
        {
            if (presentationSAPEPRoleTOUserReport == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPEPRoleToUsersReport, PresentationSAPEPRoleToUsersReport>(presentationSAPEPRoleTOUserReport);
        }

        public static List<PresentationSAPEPRoleToUsersReport> ORMapList(IList<EntityModel.Presentation.PresentationSAPEPRoleToUsersReport> presentationSAPEPRoleTOUserReports)
        {
            return presentationSAPEPRoleTOUserReports.Select(ORMap).ToList();
        }
    }
}
