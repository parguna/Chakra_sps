using DeepIdentity.DataAccess;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class ProvisioningRoleFacade
    {
        ProvisioningRoleDA provisioningRoleDA = null;

        public ProvisioningRoleFacade()
        {
            provisioningRoleDA = new ProvisioningRoleDA();
        }
        public void AddProvisioningRole(ProvisioningRole _provisioningRole)
        {
            provisioningRoleDA.AddProvisioningRole(_provisioningRole);
        }
        public void UpdateProvisioningRole(ProvisioningRole _provisioningRole)
        {
            provisioningRoleDA.UpdateProvisioningRole(_provisioningRole);
        }
        public List<ProvisioningRoleWithDetails> GetAllProvisioningRolesWithDetails()
        {
            return provisioningRoleDA.GetAllProvisioningRolesWithDetails();
        }
        public void AddProvisioningRole_Test()
        {
            provisioningRoleDA.AddProvisioningRoleDetail_Test();
        }
        public List<AccessRequestItem> GetAllEntitlementForProvisioningRole(AccessRequestFilterCriteria criteria)
        {
            return provisioningRoleDA.GetAllEntitlementForProvisioningRole(criteria.SearchKey, criteria.EntitlementEndpointSystems);
        }
        public ProvisioningRole GetProvisioningRole(int ProvRoleDetailId)
        {
            return provisioningRoleDA.GetProvisioningRole(ProvRoleDetailId);
        }
    }
}
