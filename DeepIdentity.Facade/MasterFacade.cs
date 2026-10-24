using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.DataAccess;
using System.Data;
using DeepIdentity.Model.Custom.GridData;

namespace DeepIdentity.Facade
{
    public class MasterFacade
    {
        MasterDA _masterDA { get; set; }

        public MasterFacade()
        {
            _masterDA = new MasterDA();
        }

        public List<EmployeeTerm> GetEmployeeTerms()
        {
            try
            {
                return _masterDA.GetEmployeeTerms();
            }
            catch
            {
                throw;
            }
        }

        public List<AccountCategory> GetAccountCategories()
        {
            try
            {
                return _masterDA.GetAccountCategories();
            }
            catch
            {
                throw;
            }
        }
        public List<OffboardingAccountAction> GetOffboardingAccountActions()
        {
            try
            {
                return _masterDA.GetOffboardingAccountActions();
            }
            catch
            {
                throw;
            }
        }
        public List<AccountActionCategory> GetAccountActionCategories()
        {
            try
            {
                return _masterDA.GetAccountActionCategories();
            }
            catch
            {
                throw;
            }
        }

        public IList<RequestTypes> GetRequestTypes()
        {
            try
            {
                return _masterDA.GetRequestTypes();
            }

            catch
            {
                throw;
            }
        }

        public IList<ApprovalUserTypes> GetApprovalUserTypes()
        {
            try
            {
                return _masterDA.GetApprovalUserTypes();
            }

            catch
            {
                throw;
            }
        }

        public IList<RoleTypes> GetRoleTypes()
        {
            try
            {
                return _masterDA.GetRoleTypes();
            }

            catch
            {
                throw;
            }
        }

        public string GetMasterConfigByKey(string configKey)
        {
            try
            {
                return _masterDA.GetMasterConfigByKey(configKey);
            }

            catch
            {
                throw;
            }
        }

        public void AddPosition(Designation _designation)
        {
            try
            {
                _masterDA.AddPosition(_designation);
            }
            catch
            {
                throw;
            }
        }

        public void UpdatePosition(Designation _designation)
        {
            try
            {
                _masterDA.UpdatePosition(_designation);
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
                return _masterDA.GetPositionByCode(positionCode);
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
                return _masterDA.GetOrganisationByCode(ouCode);
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
                return _masterDA.GetOrganisationById(organisationId);
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
                _masterDA.AddOrganisation(_organisation);
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
                _masterDA.UpdateOrganisation(organisation);
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
                return _masterDA.GetDepartmentByCode(paCode);
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
                return _masterDA.GetDepartmentById(departmentId);
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
                _masterDA.AddDepartment(_perArea);
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
                _masterDA.UpdateDepartment(_perArea);
            }
            catch
            {
                throw;
            }
        }

        public Division GetDivisionByCodeAndDepartment(string psaCode, int departmentId)
        {
            try
            {
                return _masterDA.GetDivisionByCodeAndDepartment(psaCode, departmentId);
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
                return _masterDA.GetDivisionById(divisionId);
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
                _masterDA.AddDivision(_perSubArea);
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
                _masterDA.UpdateDivision(_perSubArea);
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
                return _masterDA.GetCompanyByCode(compCode);
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
                return _masterDA.GetCompanyById(companyId);
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
                _masterDA.AddCompany(_company);
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
                _masterDA.UpdateCompany(_company);
            }
            catch
            {
                throw;
            }
        }

        public CostCenter GetCostCenterByCodeAndCompany(string ccCode, int companyId)
        {
            try
            {
                return _masterDA.GetCostCenterByCodeAndCompany(ccCode, companyId);
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
                _masterDA.AddCostCenter(_costCenter);
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
                _masterDA.UpdateCostCenter(costCenter);
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
                return _masterDA.GetPositions();
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
                return _masterDA.GetOrganisations();
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
                return _masterDA.GetDepartments();
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
                return _masterDA.GetDivisions();
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
                return _masterDA.GetSections();
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
                return _masterDA.GetCompanies();
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
                return _masterDA.GetCostCenters();
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
                return _masterDA.GetAllSections();
            }
            catch
            {
                throw;
            }
        }
        public Organisation GetAllSectionsByOrgUnitPrefix(string code)
        {
            try
            {
                return _masterDA.GetAllSections().FirstOrDefault(item => item.OrgUnitPrefix == code);
            }
            catch
            {
                throw;
            }
        }
        public void FlushHeadInformation()
        {
            try
            {
                _masterDA.FlushHeadInformation();
            }
            catch
            {
                throw;
            }
        }

        public string ConstructUserName(string userName, string strIdentificationNo)
        {
            try
            {
                StringBuilder userNameSB = new StringBuilder();
                userName.TrimStart();
                userNameSB.Append("T");
                if (!string.IsNullOrEmpty(userName)) // && userName.Length >= 3)
                {
                    string[] nameAry = userName.Split(' ');
                    if (nameAry.Length == 1)
                    {
                        if (nameAry[0].Length >= 3)
                            userNameSB.Append(nameAry[0].Substring(0, 3));
                        else
                            userNameSB.Append(nameAry[0]);
                    }
                    else if (nameAry.Length == 2)
                    {
                        if (nameAry[0].Length >= 2)
                        {
                            if (nameAry[0] != "")
                            {
                                userNameSB.Append(nameAry[0].Substring(0, 2));
                            }
                            userNameSB.Append(nameAry[1].Substring(0, 1));
                        }
                        else if (nameAry[1].Length >= 2)
                        {
                            if (nameAry[0] != "")
                            {
                                userNameSB.Append(nameAry[0].Substring(0, 1));
                            }
                            userNameSB.Append(nameAry[1].Substring(0, 2));
                        }
                        else
                        {
                            userNameSB.Append(nameAry[0]);
                            userNameSB.Append(nameAry[1]);
                        }
                    }
                    else if (nameAry.Length >= 3)
                    {
                        if (nameAry[0] != "")
                        {
                            userNameSB.Append(nameAry[0].Substring(0, 1));
                        }
                        userNameSB.Append(nameAry[1].Substring(0, 1));
                        userNameSB.Append(nameAry[2].Substring(0, 1));
                    }
                }
                if (!string.IsNullOrEmpty(strIdentificationNo) && strIdentificationNo.Length >= 5)
                {
                    userNameSB.Append(strIdentificationNo.Remove(0, strIdentificationNo.Length - 4));
                }
                userName = userNameSB.ToString().ToUpper();
                return userName;
            }
            catch
            {
                throw;
            }
        }

        public MailTemplateVariable GetTemplateVariableByToken(string token)
        {
            try
            {
                return _masterDA.GetTemplateVariableByToken(token);
            }
            catch
            {
                throw;
            }
        }

        public List<MailTemplateVariable> GetTemplateVariablesByScope(VariableScope scope)
        {
            try
            {
                return _masterDA.GetTemplateVariablesByScope(scope);
            }
            catch
            {
                throw;
            }
        }

        public List<MailTemplateVariable> GetTemplateVariables()
        {
            try
            {
                return _masterDA.GetTemplateVariables();
            }
            catch
            {
                throw;
            }
        }

        public IList<MetadataGridData> GetMetadata(string MetadataType) 
        {
            try
            {
                IList<MetadataGridData> lst=null;
                if (MetadataType == "ARPR")
                {
                    lst=_masterDA.GetAccessRequestProgress();
                }
                else if (MetadataType == "ARSO")
                {
                    lst = _masterDA.GetAccessRequestSource();
                }
                else if (MetadataType == "ARST")
                {
                    lst = _masterDA.GetAccessRequestStatus();
                }
                return lst;
            }

            catch
            {
                throw;
            }
        }

        public void UpdateMetadata(MetadataGridData _accreqprog) 
        {
            try
            {
                if (_accreqprog.Type == "ARPR")
                {
                    var obj = new AccessRequestProgressClass() { Id = (AccessRequestProgress)_accreqprog.Id, Description = _accreqprog.Description, IsActive = _accreqprog.IsActive };
                    _masterDA.UpdateAccessRequestProgress(obj);
                }
                if (_accreqprog.Type == "ARSO")
                {
                    var obj = new AccessRequestSourceClass() { Id = (AccessRequestSource)_accreqprog.Id, Description = _accreqprog.Description, IsActive = _accreqprog.IsActive };
                    _masterDA.UpdateAccessRequestSources(obj);
                }
                if (_accreqprog.Type == "ARST")
                {
                    var obj = new AccessRequestStatusClass() { Id = (AccessRequestStatus)_accreqprog.Id, Description = _accreqprog.Description, IsActive = _accreqprog.IsActive };
                    _masterDA.UpdateAccessStatusProgress(obj);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
