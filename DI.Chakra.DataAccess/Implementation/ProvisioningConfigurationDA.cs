using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Model;
using DI.Chakra.Model.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace DI.Chakra.DataAccess
{
    public class ProvisioningConfigurationDA
    {
        IACMDBContext _context = new IACMDBContext();

        public List<RoleMatrixGridData> GetRoleMatrices()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from rm in _context.RoleMatrices
                            select new RoleMatrixGridData
                            {
                                SectionName = rm.SectionName,
                                CreatedBy = rm.CreatedBy,
                                CreatedDate = rm.CreatedDate,
                                Department = rm.DepartmentName,
                                CostCenter = rm.CostCenter,
                                Company = rm.CompanyName,
                                Division = rm.DivisionName,
                                Organisation = rm.SectionName,
                                IsActive = rm.IsActive,
                                RoleMatrixId = rm.Id
                            }).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool DeleteRoleMatrices(int RoleId)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    RoleMatrix delrolematrix = null;
                    delrolematrix = oIACMDBContext.RoleMatrices.Where(rid => rid.Id == RoleId).FirstOrDefault();
                    if (delrolematrix != null)
                    {
                        oIACMDBContext.RoleMatrices.Remove(delrolematrix);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while deleting Role Matrix.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public Dictionary<long, string> GetProvisioningForms(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ProvisioningForms.Where(item => item.FormName.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.FormName })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }





        public Dictionary<long, string> GetAplicableApplications(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Where(item => item.DisplayName.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.DisplayName })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                    
                    //return _context.EndpointSystems.

                }
            }
            catch
            {
                throw;
            }
        }




        public Dictionary<long, string> GetCompanies(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Companies.Where(item => item.CompanyName.Contains(searchString) && item.CompanyName != "All")
                        .Select(x => new { Key = x.Id, Value = x.CompanyCode + " - " + x.CompanyName })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetAllCompanies(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Companies.Where(item => item.CompanyName.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.CompanyCode + " - " + x.CompanyName })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetCostCenters(string searchString, int companyId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (companyId != 0)
                    {
                        return _context.CostCenters.Where(item => item.CostCenterCode.Contains(searchString) && item.CompanyId == companyId)
                       .Select(x => new { Key = x.Id, Value = x.CostCenterCode })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                    return _context.CostCenters.Where(item => item.CostCenterCode.Contains(searchString))
                   .Select(x => new { Key = x.Id, Value = x.CostCenterCode })
                   .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetDepartments(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Departments.Where(item => item.DepartmentText.Contains(searchString) && item.DepartmentText != "All")
                        .Select(x => new { Key = x.Id, Value = x.DepartmentCode + " - " + x.DepartmentText })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetAllDepartments(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Departments.Where(item => item.DepartmentText.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.DepartmentCode + " - " + x.DepartmentText })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetDivisions(string searchString, int departmentId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (departmentId != 0)
                    {
                        return _context.Divisions.Where(item => item.DivisionText.Contains(searchString) && item.DepartmentId == departmentId)
                        .Select(x => new { Key = x.Id, Value = x.DivisionText })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                    return _context.Divisions.Where(item => item.DivisionText.Contains(searchString))
                    .Select(x => new { Key = x.Id, Value = x.DivisionText })
                    .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetDivisionsByDeptID(string searchString, List<int> departmentId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    Dictionary<long, string> dic = new Dictionary<long, string>();
                    if (departmentId != null && departmentId.Count > 0)
                    {
                        foreach (int DepID in departmentId)
                        {
                            Dictionary<long, string> div = _context.Divisions.Where(item => item.DivisionText.Contains(searchString) && item.DepartmentId == DepID && item.DivisionText != "All")
                            .Select(x => new { Key = x.Id, Value = x.DivisionCode + " - " + x.DivisionText })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                            foreach (KeyValuePair<long, string> division in div)
                            {
                                dic.Add(division.Key, division.Value);
                            }
                        }
                        return dic;
                    }
                    return _context.Divisions.Where(item => item.DivisionText.Contains(searchString) && item.DivisionText != "All")
                    .Select(x => new { Key = x.Id, Value = x.DivisionCode + " - " + x.DivisionText })
                    .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetOrganisations(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Organisations.Where(item => item.OrganisationName.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.OrganisationName })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetDesignations(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Designations.Where(item => item.DesignationDesc.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.DesignationDesc }).Take(100)
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetProvisioningWokflows(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.WorkflowConfigurations.Where(item => item.WorkflowName.Contains(searchString) && item.RequestTypeId == (byte)RequestType.Provisioning)
                        .Select(x => new { Key = x.Id, Value = x.WorkflowName })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public void CreateRoleMatrix(RoleMatrix roleMatrix)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.RoleMatrices.Add(roleMatrix);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateRoleMatrix(RoleMatrix roleMatrix)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(roleMatrix);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<RoleMatrix>();
                        RoleMatrix existingEntry = set.Find(roleMatrix.Id);

                        if (existingEntry != null)
                        {
                            var attachedEntry = _context.Entry(existingEntry);
                            attachedEntry.CurrentValues.SetValues(roleMatrix);
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

        public void DeleteRoleMatrixDetailById(long roleMatrixId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var details = _context.RoleMatrixDetails.Where(item => item.RoleMatrixId == roleMatrixId);
                    _context.RoleMatrixDetails.RemoveRange(details);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void CreateRoleMatrixDetail(RoleMatrixDetail roleMatrixDetail)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.RoleMatrixDetails.Add(roleMatrixDetail);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public RoleMatrix GetRoleMatrixById(int roleMatrixId)
        {
            try
            {
                using (IACMDBContext db = new IACMDBContext())
                {
                    return (from attestation in db.RoleMatrices
                            .Include("Organsation").Include("BusinessRole").Include("Company").Include("Department")
                            .Include("Division")
                            where attestation.Id == roleMatrixId
                            select attestation).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetProvisioningFormsByRoleMatrixId(long roleMatrixId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from rmd in _context.RoleMatrixDetails
                            join form in _context.ProvisioningForms
                                on rmd.ProvisioningFormId.Value equals form.Id
                            where rmd.RoleMatrixId == roleMatrixId && rmd.ProvisioningFormId.HasValue
                            select new
                            {
                                Key = form.Id,
                                Value = form.FormName
                            }).Select(x => new { x.Key, x.Value })
                     .ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetFormsByRoleMatrixId(int roleMatrixId, bool includeDefault)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (!includeDefault)
                    {
                        return (from rmd in _context.RoleMatrixDetails
                                join form in _context.ProvisioningForms
                                    on rmd.ProvisioningFormId.Value equals form.Id
                                where rmd.RoleMatrixId == roleMatrixId && rmd.ProvisioningFormId.HasValue && rmd.IsFromDefault == false
                                select new
                                {
                                    Key = form.Id,
                                    Value = form.FormName
                                }).Select(x => new { x.Key, x.Value })
                              .ToDictionary(o => o.Key, o => o.Value);
                    }
                    return (from rmd in _context.RoleMatrixDetails
                            join form in _context.ProvisioningForms
                                on rmd.ProvisioningFormId.Value equals form.Id
                            where rmd.RoleMatrixId == roleMatrixId && rmd.ProvisioningFormId.HasValue && rmd.IsFromDefault
                            select new
                            {
                                Key = form.Id,
                                Value = form.FormName
                            }).Select(x => new { x.Key, x.Value })
                          .ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckIfMappingAlreadyExists(MatrixParams param)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.RoleMatrices.Any(item => item.OrganisationId == param.OrganisationId &&
                        item.BusinessRoleId == param.CompanyId && item.CostCenter == param.CostCenter &&
                        item.DepartmentId == param.DepartmentId && item.DivisionId == param.DivisionId);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetFormsByUser(User user)
        {
            try
            {
                List<int> forms = new List<int>();
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from rm in _context.RoleMatrices
                            join rmd in _context.RoleMatrixDetails
                                on rm.Id equals rmd.RoleMatrixId
                            join form in _context.ProvisioningForms
                            on rmd.ProvisioningFormId equals form.Id
                            where rmd.ProvisioningFormId.HasValue && rm.DepartmentId == user.UserExtensionObj.DepartmentId &&
                                  rm.DivisionId == user.UserExtensionObj.DivisionId && rm.OrganisationId == user.OrganisationId &&
                                  rm.CompanyId == user.UserExtensionObj.CompanyId && rm.CostCenter == user.UserExtensionObj.CostCenter
                            select new
                            {
                                Key = form.Id,
                                Value = form.FormName
                            }).Select(x => new { x.Key, x.Value })
                              .ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public RoleMatrix GetRoleMatrixByUser(User user)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.RoleMatrices.FirstOrDefault(item => item.OrganisationId == 0 &&
                       item.DivisionId == 0 && item.DepartmentId == 0 &&
                       item.CompanyId == 0 && item.CostCenterId == 0);

                    //return _context.RoleMatrices.FirstOrDefault(item => item.DepartmentId == user.DepartmentId &&
                    //    item.DivisionId == user.DivisionId && item.OrganisationId == user.OrganisationId &&
                    //    item.CompanyId == user.CompanyId && item.CostCenter == user.CostCenter);
                }
            }
            catch
            {
                throw;
            }
        }

       
        public ADSXLProvisioningMatrix GetADConfigMatrixByUser(User user)
        {
            try
            {
                
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //var matrix = _context.ADSXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == user.DepartmentCode &&
                    //   item.DivisionCode == user.DivisionCode && item.CompanyCode == user.CompanyCode && item.CostCenterCode == user.CostCenter);
                    var matrix = _context.ADSXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == user.UserExtensionObj.DepartmentCode &&
                       item.CompanyCode == user.UserExtensionObj.CompanyCode);

                    if (matrix == null)
                    {
                        string all = "ALL";
                        matrix = _context.ADSXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == all &&
                          item.DivisionCode == all && item.CompanyCode == all && item.CostCenterCode == all);
                    }

                    return matrix;
                }
            }
            catch
            {
                throw;
            }
        }

        public string GetLNSCertifierFileByOU(string certifierOU)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var obj = _context.LNSXLProvisioningMatrices.FirstOrDefault(item => item.CertifierOU == certifierOU);

                    if (obj == null)
                        return null;

                    return obj.CertifierIdFile;
                }
            }
            catch
            {
                throw;
            }
        }

        public string GetLNSCertifierFileByName(string certifierFileName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var obj = _context.LNSXLProvisioningMatrices.FirstOrDefault(item => item.CertifierIdFile == certifierFileName);

                    if (obj == null)
                        return null;

                    return obj.CertifierOU;
                }
            }
            catch
            {
                throw;
            }
        }

        public SAPXLProvisioningMatrix GetSAPConfigMatrixByUser(User user)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var matrix = _context.SAPXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == user.UserExtensionObj.DepartmentCode &&
                        item.DivisionCode == user.UserExtensionObj.DivisionCode && item.CompanyCode == user.UserExtensionObj.CompanyCode &&
                        item.CostCenterCode == user.UserExtensionObj.CostCenter);


                    if (matrix == null)
                    {
                        string all = "ALL";
                        matrix = _context.SAPXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == all &&
                          item.DivisionCode == all && item.CompanyCode == all && item.CostCenterCode == all);
                    }

                    return matrix;
                }
            }
            catch
            {
                throw;
            }
        }

        public OracleEBSXLProvisioningMatrix GetOEBSConfigMatrixByUserOrganisation(User user)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleEBSXLProvisioningMatrices.FirstOrDefault(item => item.OrganisationId == user.UserExtensionObj.SectionCode);
                }
            }
            catch
            {
                throw;
            }
        }


        public OracleXLProvisioningMatrix GetOracleUMConfigMatrixByUserOrganisation(User user)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.OracleUMXLProvisioningMatrix.FirstOrDefault(item => item.OrganisationId == user.UserExtensionObj.SectionCode);
                }
            }
            catch
            {
                throw;
            }
        }





        public LINUXXLProvisioningMatrix GetLINUXConfigMatrixByUserOrganisation(User user)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LINUXXLProvisioningMatrices.FirstOrDefault(item => item.OrganisationId == user.UserExtensionObj.SectionCode);
                }
            }
            catch
            {
                throw;
            }
        }

        public LNSXLProvisioningMatrix GetLNSConfigMatrixByUser(User user)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    var matrix = _context.LNSXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == user.UserExtensionObj.DepartmentCode &&
                        item.DivisionCode == user.UserExtensionObj.DivisionCode && item.CompanyCode == user.UserExtensionObj.CompanyCode &&
                        item.CostCenterCode == user.UserExtensionObj.CostCenter);

                    if (matrix == null)
                    {
                        string all = "ALL";
                        matrix = _context.LNSXLProvisioningMatrices.FirstOrDefault(item => item.DepartmentCode == all &&
                          item.DivisionCode == all && item.CompanyCode == all && item.CostCenterCode == all);
                    }

                    return matrix;
                }
            }
            catch
            {
                throw;
            }
        }

        public ADSOuInfo GetADSOUByName(string ouName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (_context.ADSOuInfos.FirstOrDefault
                        (ou => ou.ou == ouName));
                }
            }
            catch
            {
                throw;
            }
        }

        public ADSGroupInfo GetADSGroupByName(string groupName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (_context.ADSGroupInfos.Where(g => g.name == groupName)).FirstOrDefault();

                }
            }
            catch
            {
                throw;
            }
        }
        public Dictionary<long, string> GetADOrganisationUnits(string searchString, long endpointsystemid)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSOuInfos.Where(item => item.distinguishedName.Contains(searchString) && item.EndPointSystemId == endpointsystemid)
                        .Select(x => new { Key = x.Id, Value = x.distinguishedName })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetLNSGroups(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusGroupInfos.Where(item => item.Group.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.Group })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetADGroups(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSGroupInfos.Where(item => item.name.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.name })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetLotusNotesCertifiers(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusCertifierInfos.Where(item => item.FullName.Contains(searchString) && item.CertificateType == 1)
                        .Select(x => new { Key = x.Id, Value = x.FullName })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public LotusCertifierInfo GetLotusNotesCertifiersById(int certifierId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (_context.LotusCertifierInfos.FirstOrDefault
                        (certifier => certifier.Id == certifierId));
                }
            }
            catch
            {
                throw;
            }
        }

        public SAPRoleCatalog GetSAPMatrixByRequest(AdHocSAPRequest request)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPRoleCatalogs.FirstOrDefault(item => item.BusinessRole == request.Entitlement &&
                        item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetADGroupsByCanRequest(string searchString, long endpoinsystemid)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ADSGroupInfos.Where(item => item.name.Contains(searchString) && item.CanRequest == true && item.EndPointSystemId == endpoinsystemid)
                        .Select(x => new { Key = x.Id, Value = x.name })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetLNSGroupsByCanRequest(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LotusGroupInfos.Where(item => item.Group.Contains(searchString) && item.CanRequest == true)
                        .Select(x => new { Key = x.Id, Value = x.Group })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<ADSXLProvisioningMatrix> GetAllADSXLProvisioningMatrix(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.ADSXLProvisioningMatrices.ToList();
                if (isValidRecords)
                    return _context.ADSXLProvisioningMatrices.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.ADSXLProvisioningMatrices.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public IList<DI.Chakra.Model.ADSCatalog> GetAllADSCatalog(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.ADSCatalogs.ToList();
                if (isValidRecords)
                    return _context.ADSCatalogs.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.ADSCatalogs.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public IList<DI.Chakra.Model.LNSCatalog> GetAllLNSCatalog(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.LNSCatalogs.ToList();
                if (isValidRecords)
                    return _context.LNSCatalogs.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.LNSCatalogs.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public IList<LNSXLProvisioningMatrix> GetAllLNSXLProvisioningMatrix(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.LNSXLProvisioningMatrices.ToList();
                if (isValidRecords)
                    return _context.LNSXLProvisioningMatrices.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.LNSXLProvisioningMatrices.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public IList<SAPRoleCatalog> GetAllSAPRoleCatalog(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.SAPRoleCatalogs.ToList();
                if (isValidRecords)
                    return _context.SAPRoleCatalogs.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.SAPRoleCatalogs.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public void UpdateSAPRoleMatrixs(SAPRoleCatalog _saprole)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SAPRoleCatalog editmatrix = _context.SAPRoleCatalogs.Where(u => u.Id == _saprole.Id).First();
                    if (editmatrix == null)
                        return;
                    _context.Entry(editmatrix).CurrentValues.SetValues(_saprole);
                    _context.SaveChanges();
                }
            }
            catch { throw; }
        }

        public bool DeleteSAPRoleMatrix(int RoleId)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPRoleCatalog delrolematrix = null;
                    delrolematrix = oIACMDBContext.SAPRoleCatalogs.Where(rid => rid.Id == RoleId).FirstOrDefault();
                    if (delrolematrix != null)
                    {
                        oIACMDBContext.SAPRoleCatalogs.Remove(delrolematrix);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while deleting SAP Role Matrix.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public IList<SAPPositionsMatrix> GetAllSAPPositionMatrix(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.SAPPositionsMatrices.ToList();
                if (isValidRecords)
                    return _context.SAPPositionsMatrices.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.SAPPositionsMatrices.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public IList<SAPCriticalTCodeMatrix> GetAllSAPCriticalTCodesMatrix(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.SAPCriticalTCodeMatrices.ToList();
                if (isValidRecords)
                    return _context.SAPCriticalTCodeMatrices.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.SAPCriticalTCodeMatrices.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public LNSMailQuotaConfig GetMailQuotaByPosition(string position)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.LNSMailQuotaConfigs.FirstOrDefault(item => position.Equals(item.Position, StringComparison.CurrentCultureIgnoreCase) &&
                        item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now);
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<LNSMailQuotaConfig> GetAllLNSMailConfig()
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.LNSMailQuotaConfigs.ToList();
            }
        }

        public IList<VPNMaster> GetAllVPNMasters(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.VPNMasters.ToList();
                if (isValidRecords)
                    return _context.VPNMasters.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.VPNMasters.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public Dictionary<long, string> GetCostCenterByCompanyID(string searchString, List<int> companyID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    Dictionary<long, string> dic = new Dictionary<long, string>();
                    if (companyID != null && companyID.Count > 0)
                    {
                        foreach (int CompID in companyID)
                        {
                            Dictionary<long, string> res = _context.CostCenters.Where(item => item.CostCenterCode.Contains(searchString) && item.CompanyId == CompID && item.CostCenterCode != "All")
                            .Select(x => new { Key = x.Id, Value = x.CostCenterCode })
                            .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                            foreach (KeyValuePair<long, string> item in res)
                            {
                                dic.Add(item.Key, item.Value);
                            }
                        }
                        return dic;
                    }
                    return _context.CostCenters.Where(item => item.CostCenterCode.Contains(searchString) && item.CostCenterCode != "All")
                    .Select(x => new { Key = x.Id, Value = x.CostCenterCode })
                    .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string> GetCostCenterCodeAndTextByCompanyID(string searchString, List<int> companyID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    if (companyID != null && companyID.Count > 0)
                    {
                        foreach (int CompID in companyID)
                        {
                            Dictionary<string, string> res = _context.CostCenters.Where(item => item.CostCenterText.Contains(searchString) && item.CompanyId == CompID && item.CostCenterText != "All")
                            .Select(x => new { Key = x.CostCenterCode, Value = x.CostCenterCode + " - " + x.CostCenterText })
                            .Distinct().ToDictionary(o => o.Key, o => o.Value);
                            foreach (KeyValuePair<string, string> item in res)
                            {
                                dic.Add(item.Key, item.Value);
                            }
                        }
                        return dic;
                    }
                    return _context.CostCenters.Where(item => item.CostCenterText.Contains(searchString) && item.CostCenterText != "All")
                    .Select(x => new { Key = x.CostCenterCode, Value = x.CostCenterCode + " - " + x.CostCenterText })
                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public SAPPositionsMatrix GetSAPPositionMatrixByPosition(string position)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPPositionsMatrices.FirstOrDefault(item => item.PositionText.Equals(position, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch
            {
                throw;
            }
        }

        public SAPRoleCatalog GetSAPRoleMatrixByRole(string role)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPRoleCatalogs.FirstOrDefault(item => item.BusinessRole.Equals(role, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetListLNSCertifierFileByOU(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var result = _context.LNSXLProvisioningMatrices.Where(item => item.CertifierIdFile.Contains(searchString)).Select(item => item);
                    Dictionary<long, string> data = new Dictionary<long, string>();
                    foreach (var items in result)
                    {
                        if (!data.ContainsValue(items.CertifierIdFile))
                        {
                            data.Add(items.Id, items.CertifierIdFile);
                        }
                    }
                    return data;
                }
            }
            catch { throw; }
        }


        public void UpdateADXLProvisioningMatrix(ADSXLProvisioningMatrix matrix)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    ADSXLProvisioningMatrix editmatrix = _context.ADSXLProvisioningMatrices.Where(u => u.Id == matrix.Id).First();
                    if (editmatrix == null)
                        return;
                    _context.Entry(editmatrix).CurrentValues.SetValues(matrix);
                    _context.SaveChanges();
                }
            }
            catch { throw; }
        }

        public void UpdateADXLProvmatrix(ADSCatalog matrix)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    ADSCatalog editmatrix = _context.ADSCatalogs.Where(u => u.Id == matrix.Id).First();
                    if (editmatrix == null)
                        return;
                    _context.Entry(editmatrix).CurrentValues.SetValues(matrix);
                    _context.SaveChanges();
                }
            }
            catch { throw; }
        }






        public bool DeleteADSXLProvisioningMatrix(int Proid)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ADSXLProvisioningMatrix delrolematrix = null;
                    delrolematrix = oIACMDBContext.ADSXLProvisioningMatrices.Where(rid => rid.Id == Proid).FirstOrDefault();
                    if (delrolematrix != null)
                    {
                        oIACMDBContext.ADSXLProvisioningMatrices.Remove(delrolematrix);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while deleting ADSXLProvisioningMatrices.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public void UpdateLNSXLProvisioningMatrix(LNSXLProvisioningMatrix Lnsmatrix)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    LNSXLProvisioningMatrix editmatrix = _context.LNSXLProvisioningMatrices.Where(u => u.Id == Lnsmatrix.Id).First();
                    if (editmatrix == null)
                        return;
                    _context.Entry(editmatrix).CurrentValues.SetValues(Lnsmatrix);
                    _context.SaveChanges();
                }
            }
            catch { throw; }
        }

        public void UpdateLNSXLProvmatrix(LNSCatalog matrix)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    LNSCatalog editmatrix = _context.LNSCatalogs.Where(u => u.Id == matrix.Id).First();
                    if (editmatrix == null)
                        return;
                    _context.Entry(editmatrix).CurrentValues.SetValues(matrix);
                    _context.SaveChanges();
                }
            }
            catch { throw; }
        }



        public bool DeleteLNSXLProvisioningMatrix(int Lnsid)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    LNSXLProvisioningMatrix delrolematrix = null;
                    delrolematrix = oIACMDBContext.LNSXLProvisioningMatrices.Where(rid => rid.Id == Lnsid).FirstOrDefault();
                    if (delrolematrix != null)
                    {
                        oIACMDBContext.LNSXLProvisioningMatrices.Remove(delrolematrix);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
               DILog.GeneralLogger.WriteLog(LogLevelType.Error,"Error occur while deleting LNSXLProvisioningMatrices.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }

        public void UpdateSAPXLProvmatrix(SAPXLProvisioningMatrix matrix)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SAPXLProvisioningMatrix editmatrix = _context.SAPXLProvisioningMatrices.Where(u => u.Id == matrix.Id).First();
                    if (editmatrix == null)
                        return;
                    _context.Entry(editmatrix).CurrentValues.SetValues(matrix);
                    _context.SaveChanges();
                }
            }
            catch { throw; }
        }


        public bool DeleteSAPXLProvisioningMatrix(int Sapid)
        {
            bool isDataSaved = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SAPXLProvisioningMatrix delprovmatrix = null;
                    delprovmatrix = oIACMDBContext.SAPXLProvisioningMatrices.Where(rid => rid.Id == Sapid).FirstOrDefault();
                    if (delprovmatrix != null)
                    {
                        oIACMDBContext.SAPXLProvisioningMatrices.Remove(delprovmatrix);
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                isDataSaved = false;
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, "Error occur while deleting SAPXLProvisioningMatrices.", ex);
                throw;
            }
            isDataSaved = true;
            return isDataSaved;
        }


        public IList<SAPXLProvisioningMatrix> GetAllSAPProvMatrix(bool isValidRecords, bool isAllRecords)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (isAllRecords)
                    return _context.SAPXLProvisioningMatrices.ToList();
                if (isValidRecords)
                    return _context.SAPXLProvisioningMatrices.Where(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList();
                return _context.SAPXLProvisioningMatrices.Where(item => item.ValidTo != DateTime.MaxValue).ToList();
            }
        }

        public RequestApprovalConfiguration IsApprovalRequired(AccessRequestSource requestSource)
        {
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    return context.RequestApprovalConfigurations.FirstOrDefault(item => item.RequestSource == requestSource);
                }
            }
            catch
            {
                throw;
            }
        }

        public long GetWorkflowConfigurationByRequestSource(AccessRequestSource requestSource)
        {
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    return context.RequestApprovalConfigurations.FirstOrDefault(item => item.RequestSource == requestSource).WorkflowConfigurationId.Value;
                }
            }
            catch
            {
                throw;
            }
        }

    }
}

