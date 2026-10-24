using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class ProvisioningConfigMatrixEEGrpRepository : RepositoryBase<EntityModel.ProvisioningConfigMatrixSapEEGroup>
    {
        public ProvisioningConfigMatrixEEGrpRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }

        public List<ProvisioningConfigMatrixSapEEGroup> GetSAPRolesBasedOnEEGroup(string eeGrp, string departmentCode)
        {
            try
            {
                string query = string.Format("SELECT * FROM [dbo].[DI_M_ProvisioningConfigMatrixSAPEEGroup] WHERE [EmployeeGroupNo] = '{0}' AND '{1}' LIKE '' + REPLACE(Parea,'*','') + '%'", eeGrp, departmentCode);
                return CurrentContext.Database.SqlQuery<ProvisioningConfigMatrixSapEEGroup>(query).ToList();
            }
            catch
            {
                throw;
            }
        }


    }
}
