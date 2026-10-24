using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Model;
using DI.Chakra.Model.Custom.GridData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace DI.Chakra.DataAccess
{
    public class MasterDA
    {

        public IList<RequestTypes> GetRequestTypes()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    IList<RequestTypes> rt = _context.RequestTypes.Where(item => item.IsActive).ToList();
                    return rt;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Getting RequestTypes", ex);
                throw ex;
            }
        }

        //public IList<ApprovalUserTypes> GetApprovalUserTypes()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.ApprovalUserTypes.Where(item => item.IsActive).ToList();
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Getting ApprovalUserTypes", ex);
        //        throw ex;
        //    }
        //}

        //public IList<RoleTypes> GetRoleTypes()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.RoleTypes.Where(item => item.IsActive).ToList();
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Getting RoleTypes", ex);
        //        throw ex;
        //    }
        //}

        //public List<EmployeeTerm> GetEmployeeTerms()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.EmployeeTerms.Where(item => item.IsActive).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public List<AccountCategory> GetAccountCategories()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.AccountCategories.Where(item => item.IsActive).ToList();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public List<OffboardingAccountAction> GetOffboardingAccountActions()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OffboardingAccountAction.ToList();
                }
            }
            catch
            {
                throw;
            }
        }
        //public List<AccountActionCategory> GetAccountActionCategories()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.AccountActionCategories.Where(item => item.IsActive).ToList();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public string GetMasterConfigByKey(string configKey)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            var configurationmaster = _context.ConfigurationMasters.FirstOrDefault(item => item.ConfigKey == configKey);
        //            if (configurationmaster != null)
        //            {
        //                return configurationmaster.ConfigValue;
        //            }
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void AddPosition(Designation _designation)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Designations.Add(_designation);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdatePosition(Designation designation)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(designation);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<Designation>();
                        Designation attached = set.Find(designation.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(designation);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public Designation GetPositionByCode(string positionCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Designations.FirstOrDefault(item => item.DesignationCode == positionCode);
                }
            }
            catch
            {
                throw;
            }
        }

        public Organisation GetOrganisationByCode(string ouCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Organisations.FirstOrDefault(item => item.OrgUnitPrefix == ouCode);
                }
            }
            catch
            {
                throw;
            }
        }

        public Organisation GetOrganisationById(int organisationId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Organisations.FirstOrDefault(item => item.Id == organisationId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddOrganisation(Organisation _organisation)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Organisations.Add(_organisation);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateOrganisation(Organisation organisation)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(organisation);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<Organisation>();
                        Organisation attached = set.Find(organisation.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(organisation);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public Department GetDepartmentByCode(string paCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Departments.FirstOrDefault(item => item.DepartmentCode == paCode);
                }
            }
            catch
            {
                throw;
            }
        }

        public Department GetDepartmentById(int departmentId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Departments.FirstOrDefault(item => item.Id == departmentId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddDepartment(Department _perArea)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Departments.Add(_perArea);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateDepartment(Department _perArea)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(_perArea);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<Department>();
                        Department attached = set.Find(_perArea.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(_perArea);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public Division GetDivisionByCodeAndDepartment(string psaCode, long departmentId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Divisions.FirstOrDefault(item => item.DivisionCode == psaCode && item.DepartmentId == departmentId);
                }
            }
            catch
            {
                throw;
            }
        }

        public Division GetDivisionById(int divisionId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Divisions.FirstOrDefault(item => item.Id == divisionId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddDivision(Division _perSubArea)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Divisions.Add(_perSubArea);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateDivision(Division _perSubArea)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(_perSubArea);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<Division>();
                        Division attached = set.Find(_perSubArea.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(_perSubArea);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public Company GetCompanyByCode(string compCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Companies.FirstOrDefault(item => item.CompanyCode == compCode);
                }
            }
            catch
            {
                throw;
            }
        }

        public Company GetCompanyById(int companyId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Companies.FirstOrDefault(item => item.Id == companyId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddCompany(Company _company)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Companies.Add(_company);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateCompany(Company _company)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(_company);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<Company>();
                        Company attached = set.Find(_company.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(_company);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public CostCenter GetCostCenterByCodeAndCompany(string ccCode, long companyId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.CostCenters.FirstOrDefault(item => item.CostCenterCode == ccCode && item.CompanyId == companyId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddCostCenter(CostCenter _costCenter)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.CostCenters.Add(_costCenter);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateCostCenter(CostCenter costCenter)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(costCenter);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<CostCenter>();
                        CostCenter attached = set.Find(costCenter.Id);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(costCenter);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Designation> GetPositions()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Designations.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Organisation> GetOrganisations()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Organisations.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Department> GetDepartments()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Departments.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Division> GetDivisions()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Divisions.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Organisation> GetSections()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from Department in _context.Organisations
                            where Department.Scope.Contains("Section")
                            select Department).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Company> GetCompanies()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Companies.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<CostCenter> GetCostCenters()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.CostCenters.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<Organisation> GetAllSections()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Organisations.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void FlushHeadInformation()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand("UPDATE DI_M_Organisation set Head = NULL, ParentOrganisation = NULL");
                }
            }
            catch
            {
                throw;
            }
        }

        //public MailTemplateVariable GetTemplateVariableByToken(string token)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.MailTemplateVariables.FirstOrDefault(item => item.VariableValue == token);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public List<MailTemplateVariable> GetTemplateVariablesByScope(VariableScope scope)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.MailTemplateVariables.Where(item => item.VariableScope == scope).ToList();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public List<MailTemplateVariable> GetTemplateVariables()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.MailTemplateVariables.Where(item => item.IsActive).ToList();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public List<MetadataGridData> GetAccessRequestProgress()    
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    
                    var AccReqList = from accreq in _context.AccessRequestProgress  
                               select new MetadataGridData
                               {
                                  Id = (int)accreq.Id,
                                  Description = accreq.Description,
                                  IsActive = accreq.IsActive
                              };
                    return AccReqList.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<MetadataGridData> GetAccessRequestSource()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var AccReqList = from accreq in _context.AccessRequestSources
                                     select new MetadataGridData
                                     {
                                         Id = (int)accreq.Id,
                                         Description = accreq.Description,
                                         IsActive = accreq.IsActive
                                     };
                    return AccReqList.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<MetadataGridData> GetAccessRequestStatus() 
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var AccReqList = from accreq in _context.AccessRequestStatus
                                     select new MetadataGridData
                                     {
                                         Id = (int)accreq.Id,
                                         Description = accreq.Description,
                                         IsActive = accreq.IsActive
                                     };
                    return AccReqList.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<MetadataGridData> GetEndpointsystemtype()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var Endpointsystype = from endsystype in _context.EndpointSystemTypes
                                     select new MetadataGridData
                                     {
                                         Id = (int)endsystype.Id,
                                         Description = endsystype.Description,
                                         IsActive =(bool)endsystype.IsActive
                                     };
                    return Endpointsystype.ToList();
                }
            }
            catch
            {
                throw;
            }
        }







        public void UpdateAccessRequestProgress(AccessRequestProgressClass _accreq)  
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    AccessRequestProgressClass editaccreq = _context.AccessRequestProgress.Where(u => u.Id == _accreq.Id).First();
                    if (editaccreq == null)
                        return;
                    _context.Entry(editaccreq).CurrentValues.SetValues(_accreq);
                    _context.SaveChanges();
                }
            }
            catch 
            {
                throw;
            }
        }

        public void UpdateAccessRequestSources(AccessRequestSourceClass _accreq) 
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    AccessRequestSourceClass editaccreq = _context.AccessRequestSources.Where(u => u.Id == _accreq.Id).First();
                    if (editaccreq == null)
                        return;
                    _context.Entry(editaccreq).CurrentValues.SetValues(_accreq);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        public void UpdateAccessStatusProgress(AccessRequestStatusClass _accreq) 
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    AccessRequestStatusClass editaccreq = _context.AccessRequestStatus.Where(u => u.Id == _accreq.Id).First();
                    if (editaccreq == null)
                        return;
                    _context.Entry(editaccreq).CurrentValues.SetValues(_accreq);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
