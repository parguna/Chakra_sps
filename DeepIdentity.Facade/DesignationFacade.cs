using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class DesignationFacade
    {
        public IList<Designation> GetAllBusinessRolesList(string _tenantId)
        {
            try
            {
                if (_tenantId == AttestationConstants.ALL || _tenantId == null)
                    return new UserDA().GetAllBusinessRolesList(null);
                else
                    return new UserDA().GetAllBusinessRolesList(_tenantId);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
