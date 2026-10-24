using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;
using DI.Chakra.Model.ReportData;

namespace DI.Chakra.DataAccess
{
    public class OrganisationDA//: IOrganisationDA
    {
        #region IOrganisationDA Members
        private IACMDbOperator m_dbOperator = null;
        private IACMDBContext oIACMDBContext = new IACMDBContext();

        public IList<Organisation> GetAllOrganisation(string _tenantId)
        {
            //string query = "DI_spGetOrganisation";
            IList<Organisation> list = new List<Organisation>();

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (_tenantId != null)
                        list = oIACMDBContext.Organisations.Where(qOrg => qOrg.TenantId == _tenantId).ToList();
                    else
                        list = oIACMDBContext.Organisations.ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Organisation GetOrganisationByName(string _name)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Organisations.FirstOrDefault(qOrg => qOrg.OrganisationName == _name);
                }
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
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Organisations.FirstOrDefault(qOrg => qOrg.OrgUnitPrefix == code);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Organisation> GetOrganisationsByParent(string parentOrg)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Organisations.Where(qOrg => qOrg.ParentOrganisation == parentOrg).ToList();
                }
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
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.Organisations.FirstOrDefault(item => item.Id == organisationId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public IList<UsersByDepartmentReportData> getAllOrganisationsAndManager()
        {
            IList<UsersByDepartmentReportData> returnList = new List<UsersByDepartmentReportData>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    returnList = (from usr in _iacmdbcontext.Users
                                  join org in _iacmdbcontext.Organisations
                                  on usr.OrganisationId equals org.Id
                                  where org.Head == usr.Id
                                  select new UsersByDepartmentReportData
                                  {
                                      deptName = org.OrganisationName,
                                      userDisplayName = usr.DisplayName,
                                      userEmployeeId = usr.EmployeeId,
                                      userId = usr.Id,
                                      userMailId = usr.EmailId,
                                      userUserName = usr.UserName,
                                      usrFirstName = usr.FirstName,
                                      usrLastName = usr.LastName
                                  }).ToList();

                    return returnList;
                }
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
                using( IACMDBContext _context=new IACMDBContext())
                {
                    return _context.Organisations.Any(item=>item.Head==UserID);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var exception =  _context.ApproverExceptions.FirstOrDefault(item => item.Person  == pernr && item.ApprovalLevel == level);

                    if (exception != null)
                    {
                        return exception.Approver;
                    }
                    return string.Empty;
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.ApproverExceptions.Add(exception);
                    _context.SaveChanges();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand("TRUNCATE TABLE DI_M_XLApproverExceptions");
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
