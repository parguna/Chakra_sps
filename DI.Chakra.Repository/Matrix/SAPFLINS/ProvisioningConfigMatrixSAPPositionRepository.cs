using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class ProvisioningConfigMatrixSAPPositionRepository : RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPPosition>
    {
        public ProvisioningConfigMatrixSAPPositionRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }

        public List<ProvisioningConfigMatrixSAPPosition> GetSAPRolesBasedOnUserPosition(string positionName)
        {
            try
            {
                string query = string.Format("SELECT * FROM [dbo].DI_M_ProvisioningConfigMatrixSAPPosition WHERE '{0}' LIKE '' + REPLACE(PositionText,'*','%') + ''", positionName);
                return CurrentContext.Database.SqlQuery<ProvisioningConfigMatrixSAPPosition>(query).ToList();
            }
            catch
            {
                throw;
            }
        }


    }
}
