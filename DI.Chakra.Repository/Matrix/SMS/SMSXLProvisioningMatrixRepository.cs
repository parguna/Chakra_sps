using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SMSXLProvisioningMatrixRepository : RepositoryBase<EntityModel.SMSXLProvisioningMatrix>
    {
        public SMSXLProvisioningMatrixRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }

        public SMSXLProvisioningMatrix GetByUser(string departmentCode, string divisionCode, string postalCode)
        {
            try
            {
                //return CurrentContext.SMSXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == departmentCode &&
                //          item.DivisionCode == divisionCode && item.PostalCode == postalCode);

                return CurrentContext.SMSXLProvisioningMatrices.FirstOrDefault(item => item.PostalCode == postalCode);
            }
            catch
            {
                throw;
            }
        }

    }
}
