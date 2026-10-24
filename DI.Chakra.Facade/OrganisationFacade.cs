using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure;
using DI.Chakra.DataAccess;
using DI.Chakra.Model.ReportData;


namespace DI.Chakra.Facade
{
    public class OrganisationFacade// : OrganisationFacade
    {
        OrganisationDA OrganisationDA = null;

        public OrganisationFacade()
        {
            OrganisationDA = new OrganisationDA();
        }

        public IList<Organisation> GetAllOrganisation(string _tenantId)
        {
            try
            {
                if (null == _tenantId || AttestationConstants.ALL == _tenantId)
                    return this.GetAll();
                else
                    return OrganisationDA.GetAllOrganisation(_tenantId);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public IList<Organisation> GetOrganisationByTenantId(string _tenantId)
        {
            try
            {
                return OrganisationDA.GetAllOrganisation(_tenantId);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public Organisation GetOrganisationByName(string _name)
        {
            try
            {
                return OrganisationDA.GetOrganisationByName(_name);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Organisation GetOrganisationByCode(string code)
        {
            try
            {
                return OrganisationDA.GetOrganisationByCode(code);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Organisation GetOrganisationById(long organisationId)
        {
            try
            {
                return OrganisationDA.GetOrganisationById(organisationId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<Organisation> GetAll()
        {
            try
            {
                return OrganisationDA.GetAllOrganisation(null);
            }
            catch (Exception)
            {

                throw;
            }
        }
    
        public IList<UsersByDepartmentReportData> getAllOrganisationsAndManager()
        {
            try
            {
                return OrganisationDA.getAllOrganisationsAndManager();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public bool CheckIfUserIsHead(int UserID)
        {
            try
            {
                return OrganisationDA.CheckIfUserIsHead(UserID);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string CheckForApproverInException(string pernr, int level)
        {
            try
            {
                return OrganisationDA.CheckForApproverInException(pernr, level);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddApproverException(ApproverException exception)
        {
            try
            {
                OrganisationDA.AddApproverException(exception);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void FlushApproverExceptions()
        {
            try
            {
                OrganisationDA.FlushApproverExceptions();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<OrganisationReportData> GetAllOrganisationReport()
        {
            try
            {
                return new UserDA().GetAllOrganisationReport();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
