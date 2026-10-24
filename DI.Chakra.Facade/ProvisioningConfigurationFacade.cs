using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.Model.CustomEntities;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;
using DI.Chakra.Model.ReportData;

namespace DI.Chakra.Facade
{
    public class ProvisioningConfigurationFacade
    {
        ProvisioningConfigurationDA _provisioningConfigDA;

        public ProvisioningConfigurationFacade()
        {
            _provisioningConfigDA = new ProvisioningConfigurationDA();
        }

        public List<RoleMatrixGridData> GetRoleMatrices()
        {
            try
            {
                return _provisioningConfigDA.GetRoleMatrices();
            }
            catch
            {
                throw;
            }
        }

        public bool DeleteRoleMatrix(int RoleId)
        {
            return _provisioningConfigDA.DeleteRoleMatrices(RoleId);
        }

        public Dictionary<long, string> GetProvisioningForms(string searchString)
        {
            try
            {
                return _provisioningConfigDA.GetProvisioningForms(searchString);

            }
            catch
            {
                throw;
            }
        }



        public Dictionary<long, string> GetApplicableApplications(string searchString)
        {
            try
            {
                return _provisioningConfigDA.GetAplicableApplications(searchString);

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
                return _provisioningConfigDA.GetCompanies(searchString);

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
                return _provisioningConfigDA.GetAllCompanies(searchString);

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
                return _provisioningConfigDA.GetCostCenters(searchString, companyId);

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
                return _provisioningConfigDA.GetDepartments(searchString);

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
                return _provisioningConfigDA.GetAllDepartments(searchString);

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
                return _provisioningConfigDA.GetDivisions(searchString, departmentId);

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
                return _provisioningConfigDA.GetDivisionsByDeptID(searchString, departmentId);

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
                return _provisioningConfigDA.GetOrganisations(searchString);

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
                return _provisioningConfigDA.GetDesignations(searchString);

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
                return _provisioningConfigDA.GetProvisioningWokflows(searchString);

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
                _provisioningConfigDA.CreateRoleMatrix(roleMatrix);
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
                _provisioningConfigDA.UpdateRoleMatrix(roleMatrix);

                _provisioningConfigDA.DeleteRoleMatrixDetailById(roleMatrix.Id);

                foreach (RoleMatrixDetail roleMatrixDetail in roleMatrix.RoleMatrixDetails)
                {
                    roleMatrixDetail.RoleMatrixId = roleMatrix.Id;
                    _provisioningConfigDA.CreateRoleMatrixDetail(roleMatrixDetail);
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateSAPRoleMatrixs(SAPRoleCatalog _saprole)
        {
            {
                _provisioningConfigDA.UpdateSAPRoleMatrixs(_saprole);
            }
        }

        public bool DeleteSAPRoleMatrix(int RoleId)
        {
            return _provisioningConfigDA.DeleteSAPRoleMatrix(RoleId);
        }

        public RoleMatrix GetRoleMatrixById(int roleMatrixId)
        {
            try
            {
                RoleMatrix data = _provisioningConfigDA.GetRoleMatrixById(roleMatrixId);

                data.Forms = _provisioningConfigDA.GetFormsByRoleMatrixId(roleMatrixId, false);

                return data;
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
                return _provisioningConfigDA.CheckIfMappingAlreadyExists(param);
            }
            catch
            {
                throw;
            }
        }

        public RoleMatrixSimpleData GetRolesAndFormsByUser(User user)
        {
            try
            {
                RoleMatrix matrix = _provisioningConfigDA.GetRoleMatrixByUser(user);

                RoleMatrixSimpleData rolesAndForms = new RoleMatrixSimpleData();

                rolesAndForms.Forms = _provisioningConfigDA.GetProvisioningFormsByRoleMatrixId(matrix.Id);

                return rolesAndForms;
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
                return _provisioningConfigDA.GetFormsByUser(user);
            }
            catch
            {
                throw;
            }
        }

        public string GetADDefaultOUByUser(User user)
        {
            try
            {
                var adConfig = this.GetADConfigMatrixByUser(user);
                return adConfig != null ? adConfig.OU : string.Empty;
            }
            catch
            {
                throw;
            }
        }

        public string GetADDefaultGroupsByUser(User user)
        {
            try
            {
                var adConfig = this.GetADConfigMatrixByUser(user);
                return adConfig != null ? (!string.IsNullOrEmpty(adConfig.DefaultGroups) ? adConfig.DefaultGroups : string.Empty) : string.Empty;
            }
            catch
            {
                throw;
            }
        }

        public string GetLNSDefaultGroupsByUser(User user)
        {
            try
            {
                var lnsConfig = this.GetLNSConfigMatrixByUser(user);
                return lnsConfig != null ? lnsConfig.DefaultGroups : string.Empty;
            }
            catch
            {
                throw;
            }
        }

        public LNSXLProvisioningMatrix GetLNSCertifierFileByUser(User user, long endpointSystemId)
        {
            try
            {
                var lnsConfig = this.GetLNSConfigMatrixByUser(user);

                return lnsConfig;

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
                return _provisioningConfigDA.GetLNSCertifierFileByOU(certifierOU);

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
                return _provisioningConfigDA.GetLNSCertifierFileByName(certifierFileName);

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
                return _provisioningConfigDA.GetADConfigMatrixByUser(user);
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
                return _provisioningConfigDA.GetSAPConfigMatrixByUser(user);
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
                return _provisioningConfigDA.GetOEBSConfigMatrixByUserOrganisation(user);
            }
            catch
            {
                throw;
            }
        }


        public OracleXLProvisioningMatrix GetOracleConfigMatrixByUserOrganisation(User user)
        {
            try
            {
                return _provisioningConfigDA.GetOracleUMConfigMatrixByUserOrganisation(user);
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
                return _provisioningConfigDA.GetLINUXConfigMatrixByUserOrganisation(user);
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
                return _provisioningConfigDA.GetLNSConfigMatrixByUser(user);
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
                ADSOuInfo data = _provisioningConfigDA.GetADSOUByName(ouName);
                return data;
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
                return _provisioningConfigDA.GetADOrganisationUnits(searchString, endpointsystemid);

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
                return _provisioningConfigDA.GetLNSGroups(searchString);

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
                return _provisioningConfigDA.GetADSGroupByName(groupName);

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
                return _provisioningConfigDA.GetADGroups(searchString);

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
                return _provisioningConfigDA.GetLotusNotesCertifiers(searchString);

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
                return _provisioningConfigDA.GetLotusNotesCertifiersById(certifierId);
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
                return _provisioningConfigDA.GetSAPMatrixByRequest(request);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetADGroupsByCanRequest(string searchString, long endpointsystemid)
        {
            try
            {
                return _provisioningConfigDA.GetADGroupsByCanRequest(searchString, endpointsystemid);

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
                return _provisioningConfigDA.GetLNSGroupsByCanRequest(searchString);

            }
            catch
            {
                throw;
            }
        }

        public IList<ADSXLProvisioningMatrix> GetAllADSXLProvisioningMatrix(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllADSXLProvisioningMatrix(isValidRecords, isAllRecords);
        }

        public IList<DI.Chakra.Model.ADSCatalog> GetAllADSCatalog(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllADSCatalog(isValidRecords, isAllRecords);
        }

        public IList<DI.Chakra.Model.LNSCatalog> GetAllLNSCatalog(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllLNSCatalog(isValidRecords, isAllRecords);
        }
        public IList<LNSXLProvisioningMatrix> GetAllLNSXLProvisioningMatrix(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllLNSXLProvisioningMatrix(isValidRecords, isAllRecords);
        }

        public IList<SAPRoleCatalog> GetAllSAPRoleCatalog(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllSAPRoleCatalog(isValidRecords, isAllRecords);
        }

        public IList<SAPPositionsMatrix> GetAllSAPPositionMatrix(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllSAPPositionMatrix(isValidRecords, isAllRecords);
        }

        public IList<SAPCriticalTCodeMatrix> GetAllSAPCriticalTCodesMatrix(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllSAPCriticalTCodesMatrix(isValidRecords, isAllRecords);
        }

        public IList<VPNMaster> GetAllVPNMasters(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllVPNMasters(isValidRecords, isAllRecords);
        }

        public int GetMailQuotaByPosition(string position)
        {
            try
            {
                var config = _provisioningConfigDA.GetMailQuotaByPosition(position);

                if (config != null)
                {
                    return config.MailQuotaSizeInMB;
                }
                else
                {
                    return 0;
                }

            }
            catch
            {
                throw;
            }
        }

        public IList<LNSMailQuotaConfig> GetAllLNSMailConfig()
        {
            try
            {
                return _provisioningConfigDA.GetAllLNSMailConfig();
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetCostCenterByCompanyID(string searchString, List<int> companyID)
        {
            try
            {
                return _provisioningConfigDA.GetCostCenterByCompanyID(searchString, companyID);
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
                return _provisioningConfigDA.GetCostCenterCodeAndTextByCompanyID(searchString, companyID);
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
                return _provisioningConfigDA.GetSAPPositionMatrixByPosition(position);
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
                return _provisioningConfigDA.GetSAPRoleMatrixByRole(role);
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
                return _provisioningConfigDA.GetListLNSCertifierFileByOU(searchString);
            }
            catch
            {
                throw;
            }
        }


        public string GetOEBSDefaultRolesByUserOrganisation(User user)
        {
            try
            {
                var lnsConfig = this.GetOEBSConfigMatrixByUserOrganisation(user);
                return lnsConfig != null ? lnsConfig.Roles : string.Empty;
            }
            catch
            {
                throw;
            }
        }


        public string GetOracleUMDefaultRolesByUserOrganisation(User user)
        {
            try
            {
                var lnsConfig = this.GetOracleConfigMatrixByUserOrganisation(user);
                return lnsConfig != null ? lnsConfig.DefaultRoles : string.Empty;
            }
            catch
            {
                throw;
            }
        }



        public string GetLinuxDefaultRolesByUserOrganisation(User user)
        {
            try
            {
                var lnsConfig = this.GetLINUXConfigMatrixByUserOrganisation(user);
                return lnsConfig != null ? lnsConfig.Groups : string.Empty;
            }
            catch
            {
                throw;
            }
        }
        public string GetOEBSDefaultResponsibilitiesByUserOrganisation(User user)
        {
            try
            {
                var lnsConfig = this.GetOEBSConfigMatrixByUserOrganisation(user);
                return lnsConfig != null ? lnsConfig.Responsibilities : string.Empty;
            }
            catch
            {
                throw;
            }
        }


        public void UpdateADXLProvmatrix(ADSCatalog matrix)
        {
            _provisioningConfigDA.UpdateADXLProvmatrix(matrix);
        }

        public void UpdateADXLProvisioningMatrix(ADSXLProvisioningMatrix matrix)
        {
            _provisioningConfigDA.UpdateADXLProvisioningMatrix(matrix);
        }


        public bool DeleteADSXLProvisioningMatrix(int Proid)
        {
            return _provisioningConfigDA.DeleteADSXLProvisioningMatrix(Proid);
        }

        public void UpdateLNSXLProvisioningMatrix(LNSXLProvisioningMatrix Lnsmatrix)
        {
            _provisioningConfigDA.UpdateLNSXLProvisioningMatrix(Lnsmatrix);
        }
        public void UpdateLNSXLProvmatrix(LNSCatalog matrix)
        {
            _provisioningConfigDA.UpdateLNSXLProvmatrix(matrix);
        }
        public bool DeleteLNSXLProvisioningMatrix(int Lnsid)
        {
            return _provisioningConfigDA.DeleteLNSXLProvisioningMatrix(Lnsid);
        }

        public void UpdateSAPXLProvmatrix(SAPXLProvisioningMatrix matrix)
        {
            _provisioningConfigDA.UpdateSAPXLProvmatrix(matrix);
        }
        public bool DeleteSAPXLProvisioningMatrix(int Sapid)
        {
            return _provisioningConfigDA.DeleteSAPXLProvisioningMatrix(Sapid);
        }

        public IList<SAPXLProvisioningMatrix> GetAllSAPProvMatrix(bool isValidRecords, bool isAllRecords)
        {
            return _provisioningConfigDA.GetAllSAPProvMatrix(isValidRecords, isAllRecords);
        }
        public bool IsApprovalRequired(AccessRequestSource requestSource)
        {
            try
            {
                var config = _provisioningConfigDA.IsApprovalRequired(requestSource);

                if (config == null)
                {
                    return false;
                }
                else
                {
                    return config.IsApprovalRequired;
                }
            }
            catch
            {
                throw;
            }
        }

        public WorkflowConfiguration GetWorkflowConfigurationByRequestSource(AccessRequestSource requestSource)
        {
            try
            {
                var config = _provisioningConfigDA.IsApprovalRequired(requestSource);

                if (config == null)
                {
                    return null;
                }
                else
                {
                    return new WorkFlowConfigurationDA().GetWorkFlowConfigurationById(config.WorkflowConfigurationId.Value, true);
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
