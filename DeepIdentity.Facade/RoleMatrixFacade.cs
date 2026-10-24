using DeepIdentity.DataAccess;
using System.Collections.Generic;

namespace DeepIdentity.Facade
{
    public class RoleMatrixFacade
    {
        RoleMatrixDA roleMatrixDA = new RoleMatrixDA();

        public Dictionary<int, string> GetSAPRoleCatalog(int EndpointUserId)
        {
            try
            {
                return roleMatrixDA.GetSAPRoleCatalog(EndpointUserId);
            }
            catch
            {
                throw;
            }
        }

    }
}
