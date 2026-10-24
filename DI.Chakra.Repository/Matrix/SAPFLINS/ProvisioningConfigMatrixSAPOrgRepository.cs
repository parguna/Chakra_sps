using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class ProvisioningConfigMatrixSAPOrgRepository : RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPOrg>
    {
        public ProvisioningConfigMatrixSAPOrgRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }

        public List<ProvisioningConfigMatrixSAPOrg> GetSAPRolesBasedOnUserOrg(string companyCode, string departmentCode, string divisionCode, string costcenter)
        {
            try
            {
                string query = string.Format("SELECT * FROM [dbo].[DI_M_ProvisioningConfigMatrixSAPORG] WHERE CompanyCode = '{0}' AND '{1}' LIKE '' + REPLACE(ISNULL(DepartmentCode,''),'*','') + '%' AND '{2}' LIKE '' + REPLACE(ISNULL(DivisionCode,''),'*','') + '%' AND '{3}' LIKE '' + REPLACE(ISNULL(CostCenterCode,''),'*','') + '%'", companyCode, departmentCode, divisionCode, costcenter);
                return CurrentContext.Database.SqlQuery<ProvisioningConfigMatrixSAPOrg>(query).ToList();
            }
            catch
            {
                throw;
            }
        }


    }
}
