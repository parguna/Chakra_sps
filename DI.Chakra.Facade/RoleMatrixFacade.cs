using DI.Chakra.DataAccess;
using System.Collections.Generic;

namespace DI.Chakra.Facade
{
    public class RoleMatrixFacade
    {
        RoleMatrixDA roleMatrixDA = new RoleMatrixDA();

        public Dictionary<long, string> GetSAPRoleCatalog(long EndpointUserId)
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
