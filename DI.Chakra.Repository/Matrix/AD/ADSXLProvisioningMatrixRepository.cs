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
    public class ADSXLProvisioningMatrixRepository : RepositoryBase<EntityModel.ADSXLProvisioningMatrix>
    {
        public ADSXLProvisioningMatrixRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }

        public ADSXLProvisioningMatrix GetByUser(string departmentCode, string branchCode, string sectionCode, string companyCode, string costcenter, string OrganisationCode)
        {
            try
            {
                //return CurrentContext.ADSXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == departmentCode &&
                //          item.DivisionCode == divisionCode && item.OrganisationId == sectionCode &&
                //          item.CompanyCode == companyCode && item.CostCenterCode == costcenter);

                 

                var list = CurrentContext.ADSXLProvisioningMatrices.Where(item => item.OrganisationId == OrganisationCode).Select(item => item).ToList();

                if(string.IsNullOrWhiteSpace(branchCode))
                    branchCode = "".PadLeft(8, '0');

                if (list == null || list.Count() == 0)
                    return null;
                else
                    return list.FirstOrDefault(item => 
                            item.DivisionCode == branchCode 
                            && item.OrganisationId == OrganisationCode &&
                               item.CompanyCode == companyCode );
            }
            catch
            {
                throw;
            }
        }


    }
}
