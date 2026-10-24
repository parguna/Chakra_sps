using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;

namespace DI.Chakra.Repository 
{
    public class SAPGRCRoleRepository : RepositoryBase<SAPGRCRole>
    {
        public SAPGRCRoleRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBusinessRoles(long EndpointUserId, ApplicationType _applicationType, AdHocActionType actionType)
        {
            long key = 0;
            List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                     where user.Id == EndpointUserId
                                     from role in user.SAPRoleInfo
                                     select role.ROLE_CODE).ToList();


            var businessRolesQuery = (from businessRole in CurrentContext.SAPGRCRole 
                    select businessRole);

            if (actionType == AdHocActionType.AmendmentRemove)
            {
                businessRolesQuery = businessRolesQuery.Where(businessRole => SapRoles.Contains(businessRole.TechnicalRole)).Select(businessRole => businessRole);
            }
            else
            {
                businessRolesQuery = businessRolesQuery.Where(businessRole => !SapRoles.Contains(businessRole.TechnicalRole)).Select(businessRole => businessRole);
            }

            

            //var businessRoles = businessRolesQuery.ToList();
            //add dummy key and return

            return businessRolesQuery.Select(b=>b.BusinessRoleName).Distinct().OrderBy(item => item).ToDictionary(item => key++, item => item);

        }
        //Added new method to pass search filter criteria.
        public Dictionary<long, string> GetBusinessRoles(long EndpointUserId, ApplicationType _applicationType, AdHocActionType actionType, string searchString, string subModule, string Module)
        {
            long key = 0;

            string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPUserinfo_Id={0}", EndpointUserId);

            List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();


            var businessRolesQuery = (from businessRole in CurrentContext.SAPGRCRole
                                      select businessRole);

            businessRolesQuery = businessRolesQuery.Where(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.BusinessRoleName.Contains(searchString)) &&
                                    item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now &&
                                     (string.IsNullOrEmpty(Module) ? 1 == 1 : item.Module == Module) &&
                                     (string.IsNullOrEmpty(subModule) ? 1 == 1 : item.SubModule == subModule));

            if (actionType == AdHocActionType.AmendmentRemove)
            {
                businessRolesQuery = businessRolesQuery.Where(businessRole => SapRoles.Contains(businessRole.TechnicalRole)).Select(businessRole => businessRole);
            }
            else
            {
                businessRolesQuery = businessRolesQuery.Where(businessRole => !SapRoles.Contains(businessRole.TechnicalRole)).Select(businessRole => businessRole);
            }



            //var businessRoles = businessRolesQuery.ToList();
            //add dummy key and return

            return businessRolesQuery.Select(b => b.BusinessRoleName).Distinct().OrderBy(item => item).ToDictionary(item => key++, item => item);

        }
        public Dictionary<long, string> GetBusinessRoles(long EndpointUserId)
        {
            long key = 0;
            string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPUserinfo_Id={0}", EndpointUserId);
            List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();
            var businessRolesQuery = (from businessRole in CurrentContext.SAPGRCRole select businessRole);
            businessRolesQuery = businessRolesQuery.Where(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now);
            businessRolesQuery = businessRolesQuery.Where(businessRole => SapRoles.Contains(businessRole.TechnicalRole)).Select(businessRole => businessRole);
            return businessRolesQuery.Select(b => b.BusinessRoleName).Distinct().OrderBy(item => item).ToDictionary(item => key++, item => item);

        }
    }
}
