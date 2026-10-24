using DI.Chakra.DataAccess;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Facade
{
    public class ProvisioningRoleFacade
    {
        ProvisioningRoleDA provisioningRoleDA = null;

        public ProvisioningRoleFacade()
        {
            provisioningRoleDA = new ProvisioningRoleDA();
        }
        //public void AddProvisioningRole(ProvisioningRole _provisioningRole)
        //{
        //    provisioningRoleDA.AddProvisioningRole(_provisioningRole);
        //}
        //public void UpdateProvisioningRole(ProvisioningRole _provisioningRole)
        //{
        //    provisioningRoleDA.UpdateProvisioningRole(_provisioningRole);
        //}
        //public List<ProvisioningRole> GetAllProvisioningRolesWithDetails()
        //{
        //    return provisioningRoleDA.GetAllProvisioningRolesWithDetails();
        //}
        //public void AddProvisioningRole_Test()
        //{
        //    provisioningRoleDA.AddProvisioningRoleDetail_Test();
        //}
        public List<AccessRequestItem> GetAllEntitlementForProvisioningRole(AccessRequestFilterCriteria criteria)
        {
            return provisioningRoleDA.GetAllEntitlementForProvisioningRole(criteria.SearchKey, criteria.EntitlementEndpointSystems);
        }
        //public ProvisioningRole GetProvisioningRole(int ProvRoleId)
        //{
        //    return provisioningRoleDA.GetProvisioningRole(ProvRoleId);
        //}
       
        //public void DeleteProvisioningRole(int ProvRoleId)
        //{
        //     provisioningRoleDA.DeleteProvisioningRole(ProvRoleId);
        //}
    }
}
