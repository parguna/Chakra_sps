using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.Web
{
    public static class DomainApiAction
    {
        #region ACL
        public static class ACL
        {
            public static class PageUrl
            {
                public const string DoesExists = "DoesExists";
                public const string HasAnyWebPage = "HasAnyWebPage";
                public const string GetByUrl = "GetByUrl";
            }
            public static class Permission
            {
                public const string DoesExists = "DoesExists";
                public const string GetBypageId = "GetBypageId";
            }

            public static class Role
            {
                public const string GetAllRolesByEndpointID = "GetAllRolesByEndpointID";
                public const string GetByIdIncludingUser = "GetByIdIncludingUser";
                public const string SaveRole = "SaveRole";
                public const string DeleteUserFromRole = "DeleteUserFromRole";
                public const string DoesExists = "DoesExists";
                public const string DeleteRole = "DeleteRole";
            }


            public static class TabMenu
            {
                public const string GetTabMenu = "GetTabMenu";
                public const string GetMenu = "GetMenu";
                public const string GetByMenuname = "GetByMenuname";
                public const string GetCategoryById = "GetCategoryById";
                public const string DoesChildExists = "DoesChildExists";
                public const string DoesAnyExists = "DoesAnyExists";
                public const string GetBySearchString = "GetBySearchString";
                public const string GetByApplicationMode = "GetByApplicationMode";
                public const string GetAllReportMenu = "GetAllReportMenu";
            }

            public static class WebPage
            {
                public const string DoesAnyExists = "DoesAnyExists";
                public const string GetByIdInludePageUrl = "GetByIdInludePageUrl";
                public const string GetAllInludePageUrl = "GetAllInludePageUrl";
                public const string DeletePageById = "DeletePageById";
                public const string GetByAbsolutePath = "GetByAbsolutePath";
                public const string GetByName = "GetByName";
            }

            public static class PagePermission
            {
                public const string HasAccess = "HasAccess";
                public const string GetPagePermissions = "GetPagePermissions";
                public const string DoesExistsByPermissionId = "DoesExistsByPermissionId";
                public const string GetByUrlId = "GetByUrlId";
                public const string UpdateUrlPermission = "UpdateUrlPermission";
                public const string GetPagePermissionsByUser = "GetPagePermissionsByUser";
            }

            public static class PresentationPagePermission
            {
                public const string GetByPageId = "GetByPageId";
            }

            public static class PresentationTabMenu
            {
                public const string GetAll = "GetAll";
            }

            public static class AccessRights
            {
                public const string CloneRole = "CloneRole";
                public const string GetAccessRightsByRoleId = "GetAccessRightsByRoleId";
                public const string DeleteAccessRights = "DeleteAccessRights";
                public const string GetByPagePermission = "GetByPagePermission";
                public const string GetExportData = "GetExportData";
            }
        }
        #endregion

        #region Ruleset(OrgRevamp)
        public static class Ruleset
        {
            public static class NRulesetType
            {
                public const string GetAllRulesetType = "GetNRulesetTypes";
            }
            public static class NRuleset
            {
                public const string GetAllRuleset = "GetAllRuleset";
                public const string GetByRuleId = "GetByRuleId";
                public const string GetSingleRulesetByRuleId = "GetSingleRulesetByRuleId";
                public const string GetRuleSetByCodeAndRuleId = "GetRuleSetByCodeAndRuleId";
                public const string GetRulesetForApplication = "GetRulesetForApplication";
                public const string DoesExists = "DoesExists";
                public const string GetRulesByRuleID = "GetRulesByRuleID";
                public const string GetByTypeId = "GetByTypeId";
            }
            public static class NRulesetforApplication
            {
                public const string SaveApplicationDefault = "SaveApplicationDefault";
                public const string GetAll = "GetAll";
            }
            public static class PresentationNRulesetForApplication
            {
                public const string GetByRuleset = "GetByRuleset";

            }
            public static class NProvisioningField
            {
                public const string GetByEndPointSystemIncludingValueDisplayName = "GetByEndPointSystemIncludingValueDisplayName";
                public const string GetByEndpointSystemId = "GetByEndpointSystemId";
                public const string GetByEndpointSystemIdAndFieldName = "GetByEndpointSystemIdAndName";
                public const string HasAnyByEndPointsystemId = "HasAnyByEndPointsystemId";
                public const string DoesExists = "DoesExists";
                public const string DeleteByEndPointSystemId = "DeleteByEndPointSystemId";
            }
            public static class NRulesetForProvisioning
            {
                public const string SaveProvisioningDefaults = "SaveProvisioningDefaults";
                public const string GetByRule = "GetByRule";
                public const string SaveUploadData = "SaveUploadData";
                public const string GetByIdIncludingProvisioningField = "GetByIdIncludingProvisioningField";
            }
            public static class PresentationNRulesetForProvisioning
            {
                public const string GetByRule = "GetByRule";
                public const string GetAllProvisioningDefaults = "GetAllProvisioningDefaults";
                public const string GetByRuleId = "GetByRuleId";
            }
            public static class PresentationNRulesetForTechnicalRole
            {
                public const string GetByRuleEndpoint = "GetByRuleEndpoint";
                public const string GetAll = "GetAll";
            }
            //Adding for Rule Accounts
            public static class PresentationNRulesetForAccounts
            {
                public const string GetByRuleEndpoint = "GetByRuleEndpoint";
                public const string GetAll = "GetAll";
            }
            //Adding for PIMTechnicalRole
            public static class PresentationNRulesetForPIMTechnicalRole
            {
                public const string GetByRuleEndpoint = "GetByRuleEndpoint";
                public const string GetAll = "GetAll";
            }

            public static class PresentationRoleGroupDetail
            {
                public const string GetCatalogs = "GetCatalogs";
                public const string GetRulsetRoleGroupDetailsById = "GetRulsetRoleGroupDetailsById";
            }
            public static class NRulesetForRoleGroupDetail
            {
                public const string DoesExists = "DoesExists";
            }
            public static class NProvisioningProcess
            {
                public const string GetAllProvisioningProcess = "GetAllProvisioningProcess";
                public const string GetAllProvisioningSubProcess = "GetAllProvisioningSubProcess";
                public const string GetAllProvisioningCustomProcess = "GetAllProvisioningCustomProcess";
                public const string GetByEndpointSystemAndTypeId = "GetByEndpointSystemAndTypeId";

            }
            public static class Designation
            {
                public const string GetNDesignations = "GetNDesignations";
                public const string GetByCode = "GetByCode";
            }
            public static class Jobcode
            {
                public const string GetNJobcodes = "GetNJobcodes";
                public const string GetByCode = "GetByCode";
            }
            public static class CostCenter
            {
                public const string GetNCostCenters = "GetCostCenters";
                // public const string GetByCode = "GetByCode";
            }
            public static class NProvisioningEndpointTriggerProcess
            {
                public const string GetByEndpointTypeAndSystem = "GetByEndpointTypeAndSystem";
                public const string DoesExists = "DoesExists";
                public const string DeleteByEndPointSystemId = "DeleteByEndPointSystemId";
            }
            public static class NProvisioningEventProcess
            {
                public const string DoesExists = "DoesExists";
                public const string DeleteByEndPointSystemId = "DeleteByEndPointSystemId";
                public const string GetByEndPoints = "GetByEndPoints";
            }
        }
        #endregion

        #region Reconciliation
        public static class Recon
        {
            public static class ExceptionAttribute
            {
                public const string GetByEndpointTypeId = "GetByEndpointTypeId";
                //public const string SaveList = "SaveList";
            }
            public static class ExceptionAttributeChange
            {
                public const string GetByReconcilationId = "GetByReconcilationId";
            }
        }
        #endregion

        #region ADMIN

        public static class Admin
        {

            public static class TrustedSource
            {
                public const string GetByTableName = "GetByTableName";
            }
        }
        #endregion

        #region SecurityQA
        public static class SecurityQA
        {
            public static class GlobalAnswer
            {
                public const string GetByUserId = "GetByUserId";
                public const string HasAnyQuestionByUserId = "HasAnyQuestionByUserId";
                public const string GetByUser = "GetByUser";
                public const string GetsecurityquestionById = "GetsecurityquestionById";
                public const string GetByIdAndUserName = "GetByIdAndUserName";
            }
            public static class GlobalQuestion
            {
                public const string DoesExists = "DoesExists";
            }
        }
        #endregion

        #region Attestation
        public static class Attestation
        {
            public const string GetByIdIncludingAttestationProcesses = "GetByIdIncludingAttestationProcesses";
            public const string DoesExists = "DoesExists";
            public const string DeleteAllNavigation = "DeleteAllNavigation";
            public const string IsLinkedWithAtestation = "IsLinkedWithAtestation";
            public const string GetByAttestationName = "GetByAttestationName";
            public const string UpdateReviewerDetail = "UpdateReviewerDetail";
            public const string GetByAttestationCampaignId = "GetByAttestationCampaignId";
            public const string GetAttachmentById = "GetAttachmentById";
            public const string SaveAttachementList = "SaveAttachementList";

            public static class AttestationReviewEndpointSystemHistory
            {
                public const string GetWithValidComments = "GetWithValidComments";
            }
            public static class PresentationAttetstationCampaign
            {
                public const string GetAttestationOverallStatus = "GetAttestationOverallStatus";
                public const string GetCampaignStatus = "GetCampaignStatus";
                public const string GetCampaignTaskDetail = "GetCampaignTaskDetail";
                public const string GetCampaignReport = "GetCampaignReport";
            }
            public static class AttestationSnapshotActionHistory
            {
                public const string GetAttestationActionHistory = "GetAttestationActionHistory";
                public const string GetWithValidComments = "GetWithValidComments";
            }

            public static class AttestationProcess
            {
                public const string GetByAttestationId = "GetByAttestationId";
                public const string GetByIdIncludeAttestation = "GetByIdIncludeAttestation";
                public const string DoesExists = "DoesExists";
                public const string DoesExistsByWorkflowStatus = "DoesExistsByWorkflowStatus";
                public const string DeleteAttestationProcess = "DeleteAttestationProcess";
            }

            public static class PresentationAttestationScope
            {
                public const string GetAttestationScopeByAttestationId = "GetAttestationScopeByAttestationId";
            }

            public static class Process
            {
                public const string DeleteAllNavigation = "DeleteAllNavigation";
                public const string GetComplianceJobs = "GetComplianceJobs";

            }

            public static class AttestationEndpoint
            {
                public const string GetAttestationEndpoints = "GetAttestationEndpoints";
                public const string GetAttestationByEndpointName = "GetAttestationByEndpointName";
                public const string GetAttestationEndpointsForActiveCampaign = "GetAttestationEndpointsForActiveCampaign";
            }

            public static class AttestationCampaignReport
            {
                public const string GetByAttestationCampaignId = "GetByAttestationCampaignId";
                public const string GetReportByType = "GetReportByType";
            }
            public static class AttestationCampaign
            {
                public const string GetByWorkflowInstanceID = "GetByWorkflowInstanceId";
                public const string GetByCampaignId = "GetByCampaignId";
                public const string GetAttachmentCount = "GetAttachmentCount";
                public const string DoesExistsOtherSystemEndpoint = "DoesExistsOtherSystemEndpoint";
                
            }



            public static class Presentation
            {
                public static class PresentationAttestationStatusSummary
                {
                    public const string GetSummaryByEndPointTypeName = "GetSummaryByEndPointTypeName";
                    public const string GetTaskSummary = "GetTaskSummary";
                }
                public static class PresentationListAttestation
                {
                    public const string GetAllByEnpointSystemTypeId = "GetAllByEnpointSystemTypeId";
                    public const string GetAuthorizationDataForAttestationReview = "GetAuthorizationDataForAttestationReview";
                }

                public static class PresentationJobListAttestation
                {
                    public const string GetAttestationProcessJobs = "GetAttestationProcessJobs";
                    public const string GetAttestationJobs = "GetAttestationJobs";
                }

                public static class PresentationSapUserRoleTCodeAuthorization
                {
                    public const string GetSapUserRoleTCodeAuthorization = "GetSapUserRoleTCodeAuthorization";
                }
                public static class PresentationMyRequest
                {
                    public const string GetByRequesterId = "GetByRequesterId";
                    public const string GetByActionUserId = "GetByActionUserId";
                    public const string GetByActionUserDepartment = "GetByActionUserDepartment";
                    public const string GetByAdhocId = "GetByAdhocId";
                    public const string GetOnlyEventTriggedAccessRequests = "GetOnlyEventTriggedAccessRequests";
                    public const string GetPagedByActionUserId = "GetPagedByActionUserId";
                }

                public static class PresentationPreviousApprovers
                {
                    public const string GetByWorkflowInstanceId = "GetByWorkflowInstanceId";

                }
            }
        }
        #endregion

        #region Risk
        public static class Risks
        {
            public static class SODRiskCode
            {
                public const string DoesExists = "DoesExists";
                public const string GetBySearchString = "GetBySearchString";
                public const string GetAllByIncludeUser = "GetAllByIncludeUser";
                public const string HasRiskLevel = "HasRiskLevel";
                public const string HasRiskScore = "HasRiskScore";
                public const string GetByRiskCode = "GetByRiskCode";
            }

            public static class SODRiskLevel
            {
                public const string DoesExists = "DoesExists";
                public const string GetBySearchString = "GetBySearchString";
            }

            public static class SODRiskScore
            {
                public const string DoesExists = "DoesExists";
                public const string GetBySearchString = "GetBySearchString";
            }
        }
        #endregion
        public static class AdhocJob
        {
            public static class PresentationAdHocJobProcess
            {
                public const string GetAdHocProcessJobs = "GetAdHocProcessJobs";

            }
            public static class AdHocJobProcess
            {
                public const string DoesProcessNameAlreadyExists = "DoesProcessNameAlreadyExists";
                public const string DeleteAdHocJobProcess = "DeleteAdhocProcess";
            }
        }
        #region SOD
        public static class SODException
        {
            public static class Exception
            {
                public const string GetByIdIncludeRiskOwner = "GetByIdIncludeRiskOwner";
                public const string GetAllIncludingRiskOwner = "GetAllIncludingRiskOwner";
                public const string GetByExceptionCode = "GetByExceptionCode";
                public const string GetBySearchString = "GetBySearchString";
                public const string HasFunction = "HasFunction";
                public const string DoesExists = "DoesExists";
            }
        }
        public static class SODReport
        {
            public const string GetBySODRunPolicyIdIncludeSodFunctionItem = "GetBySODRunPolicyIdIncludeSodFunctionItem";
            public const string DoesEntitlementConflict = "DoesEntitlementConflict";
            public const string GetConflictedPolicy = "GetConflictedPolicy";
        }
        public static class SODPolicyReport
        {
            public const string GetPolicyReport = "GetPolicyReport";
            public const string GetUniqueRunCodes = "GetUniqueRunCodes";
            public const string GetSODEntitlementConflicts = "GetSODEntitlementConflicts";
        }
        public static class SODMitigation
        {
            public const string GetByIdIncludeSodRiskCode = "GetByIdIncludeSodRiskCode";
            public const string HasRiskCode = "HasRiskCode";
            public const string DoesExists = "DoesExists";
            public const string DoesRiskCodeExists = "DoesRiskCodeExists";
        }
        public static class SystemSetupReports
        {
            public const string GetComplianceReportResult = "GetComplianceReportResult";
        }
        public static class ReportDefinitions
        {
            public const string GetBySearchString = "GetBySearchString";
            public const string ExecuteQuery = "ExecuteQuery";
            public const string GetDataTableExecuteQuery = "GetDataTableExecuteQuery";
        }

        public static class AdhocJobsDefinitions
        {
            public const string GetBySearchString = "GetBySearchString";
        }
        public static class SODPolicyExecutionDefinition
        {
            public static class ExecutionDefinition
            {
                public const string GetByIdIncludingCompliance = "GetByIdIncludingCompliance";
                public const string GetAllByIncludeCompliancePolicy = "GetAllByIncludeCompliancePolicy";
                public const string GetBySearchString = "GetBySearchString";
                public const string HasPolicy = "HasPolicy";
                public const string HasException = "HasException";
                public const string SODPolicyDeleteById = "SODPolicyDeleteById";
                public const string DoesExists = "DoesExists";
                public const string GetByRunCodeIncludingCompliance = "GetByRunCodeIncludingCompliance";
                public const string GetUniqueRunCodes = "GetUniqueRunCodes";
                public const string DeleteAllNavigation = "DeleteAllNavigation";
            }
            public static class SODExecutionConfig
            {
                public const string GetAllWithEndPointSystemType = "GetAllWithEndPointSystemType";
            }
        }


        public static class SODFunction
        {
            public static class Function
            {
                public const string DoesExists = "DoesExists";
                public const string GetByIdIncludeSodFunctionItem = "GetByIdIncludeSodFunctionItem";
                public const string GetAllIncludeSodFunctionItems = "GetAllIncludeSodFunctionItems";
                public const string GetBySodFunctionCodeIncludingSodFunctionItems = "GetBySodFunctionCodeIncludingSodFunctionItems";
                public const string GetBySearchString = "GetBySearchString";
            }
            public static class SODFunctionItem
            {
                public const string GetBySearchString = "GetBySearchString";
                public const string GetByIdWithNavigation = "GetByIdWithNavigation";
            }
            public static class SODConflict
            {
                public const string DeleteBySodConflictCode = "DeleteBySodConflictCode";
                public const string DeleteConflictByFunction = "DeleteConflictByFunction";
            }

            public static class SODConfiguration
            {
                public const string DoesExists = "DoesExists";
            }
            public static class SODConflictFunction
            {
                public const string DoesExists = "DoesExists";
                public const string GetByIdWithNavigation = "GetByIdWithNavigation";
                public const string DeleteBySodConflictCode = "DeleteBySodConflictCode";
                public const string GetByIdIncludingSODRiskCode = "GetByIdIncludingSODRiskCode";
                public const string GetBySodConflictCode = "GetBySodConflictCode";
                public const string GetAllByIncludeSodRiskCode = "GetAllByIncludeSodRiskCode";
                public const string HasRiskCode = "HasRiskCode";
                public const string HasFunction = "HasFunction";
                public const string GetConflictByConflictAndRiskCodeIncludingFunctions = "GetConflictByConflictAndRiskCodeIncludingFunctions";
                public const string GetSodConflictListByConflictType = "GetSodConflictListByConflictType";
                public const string DeleteAllNavigation = "DeleteAllNavigation";
            }

            public static class UserSODComplianceReport
            {
                public const string GetReportbyUserId = "GetReportbyUserId";
            }

            public static class SODPolicyConfig
            {
                public const string DoesExists = "DoesExists";
                public const string isExists = "isExists";
            }

        }

        #endregion
        public static class CustomReport
        {
            public const string GetAllRunReports = "GetAllRunReports";
            public const string GetReportViewerData = "GetReportViewerData";
            public const string GetReportViewerDataCount = "GetReportViewerDataCount";
            public const string GetReportViewerDataForExport = "GetReportViewerDataForExport";
            public const string GetAllColumnsFromQuery = "GetAllColumnsFromQuery";
        }
        public static class ReportJob
        {
            public static class PresentationReportJobProcess
            {
                public const string GetReportProcessJobs = "GetReportProcessJobs";
            }
            public static class ReportJobProcess
            {
                public const string DoesProcessNameAlreadyExists = "DoesProcessNameAlreadyExists";
                public const string DeleteReportProcess = "DeleteReportProcess";
            }
            public static class AdhocJob
            {
                public static class PresentationAdHocJobProcess
                {
                    public const string GetAdHocProcessJobs = "GetAdHocProcessJobs";
                }
                public static class AdHocJobProcess
                {
                    public const string DoesProcessNameAlreadyExists = "DoesProcessNameAlreadyExists";
                    public const string DeleteAdHocJobProcess = "DeleteAdHocJobProcess";
                }
            }
        }

        #region Complaince
        public static class Complaince
        {
            public static class Presentation
            {
                public static class PresentationComlainceProcess
                {
                    public const string GetComplianceProcessJobs = "GetComplianceProcessJobs";
                }
            }
            public static class ComplianceProcess
            {
                public const string GetByIdIncludePolicyExecutionDefinition = "GetByIdIncludePolicyExecutionDefinition";
                public const string DoesExistsByProcessName = "DoesExistsByProcessName";
                public const string DeleteComplianceProcess = "DeleteComplianceProcess";
                public const string GetComplianceProcessByPolicyExecutionId = "GetComplianceProcessByPolicyExecutionId";
            }

            public static class CompliancePolicy
            {
                public static class Policy
                {
                    public const string DoesExists = "DoesExists";
                    public const string HasConflictFunction = "HasConflictFunction";
                    public const string GetBySearchString = "GetBySearchString";
                    public const string GetByIdWithNavigation = "GetByIdWithNavigation";
                    public const string GetUserSODComplianceReport = "GetUserSODComplianceReport";
                    public const string DeleteAllNavigation = "DeleteAllNavigation";
                }
            }

            public static class ComplianceEndPointMatrix
            {
                public const string GetComplianceEndPointMatrixByConflictCodes = "GetComplianceEndPointMatrixByConflictCodes";
                public const string DoesExistsByEntitlements = "DoesExistsByEntitlements";
                public const string DeleteByEndPointSystemIdAndEntitlement = "DeleteByEndPointSystemIdAndEntitlement";
                public const string GetByConflictCodes = "GetByConflictCodes";
                public const string DoesExistsByEntitlementsAndObjectLevel = "DoesExistsByEntitlementsAndObjectLevel";
                public const string GetComplianceEndPointMatrix = "GetComplianceEndPointMatrix";
            }

            public static class ComplianceGroup
            {
                public const string GetByIdIncludingGroupObject = "GetByIdIncludingGroupObject";
                public const string DoesExist = "DoesExist";
                public const string GetAllComplianceGroups = "GetAllComplianceGroups";
                public const string DeleteObjectFromComplianceGroup = "DeleteObjectFromComplianceGroup";
                public const string GetComplianceGroupById = "GetComplianceGroupById";
            }

            public static class ComplianceAuthorizationMatrix
            {
                public const string DoesExistsByObjectCode = "DoesExistsByObjectCode";
                public const string GetByConflictCodes = "GetByConflictCodes";
                public const string DeleteByObjectCode = "DeleteByObjectCode";
            }
        }
        #endregion

        #region Email
        public static class Email
        {
            public static class MailServerConfiguration
            {
                public const string GetAllConfig = "GetAllConfig";
            }
            public static class MailTemplateVariable
            {
                public const string GetActiveMailTemplateVariables = "GetActiveMailTemplateVariables";
                public const string GetActiveMailTemplateVariablesByScope = "GetActiveMailTemplateVariablesByScope";
            }

            public static class SendOTP
            {
                public const string SendOTPEmail = "SendOTPEmail";
                public const string EmailCompose = "EmailCompose";
                public const string SendPASOTPEmail = "SendPASOTPEmail";
                public const string PASEmailCompose = "PASEmailCompose";
            }

            public static class MailTemplate
            {
                public const string GetByTemplateName = "GetByTemplateName";
                public const string GetByTemplateId = "GetByTemplateId";
                public const string GetReplacedTokensByTemplateId = "GetReplacedTokensByTemplateId";

                public const string DoesNameAlreadyExists = "DoesNameAlreadyExists";
            }
            public static class PresentationMail
            {
                public const string GetByActivityType = "GetByActivityType";
                public const string GetPagedDataByActivityType = "GetPagedDataByActivityType";
                public const string DownloadEmail = "DownloadEmail";
            }

        }
        #endregion

        #region Log
        public static class Log
        {
            public static class UILog
            {
                public const string GetAllOrderByLogDate = "GetAllOrderByLogDate";
                public const string GetAllCount = "GetAllCount";
                public const string SaveUILog = "SaveUILog";

            }
            public static class ServiceLog
            {
                public const string GetByLogleveltype = "GetByLogleveltype";
                public const string GetCountByLogleveltype = "GetCountByLogleveltype";
                public const string GetByReferenceType = "GetByReferenceType";
                public const string GetByLogDate = "GetByLogDate";
                public const string GetCountByLogDate = "GetCountByLogDate";
                public const string GetLoginDetail = "GetLoginDetail";
            }

            public static class PIMSessionLog
            {
                public const string SavePIMSessionLog = "SavePIMSessionLog";
            }
        }
        #endregion

        #region HouseKeeping
        public static class HouseKeeping
        {
            public static class PurgeConfig
            {
                public const string DoesExists = "DoesExists";
            }
        }
        #endregion

        #region Tenant
        public static class Tenant
        {
            public const string DoesExists = "DoesExists";
            public const string Name = "Name";
        }
        #endregion

        #region Configuration
        public static class Configuration
        {
            public static class ConfigGeneralTrustedSource
            {
                public const string IsSelfUpdated = "IsSelfUpdated";
                public const string AddConfigTrustedSource = "AddConfigTrustedSource";
                public const string DoesAlreadyExists = "DoesAlreadyExists";
            }

            public static class ConfigurationMaster
            {
                public const string DoesExists = "DoesExists";
                public const string GetByProduct = "GetByProduct";
                public const string GetByConfigKey = "GetByConfigKey";
                public const string SaveConfiguration = "SaveConfiguration";
                public const string DeleteConfiguration = "DeleteConfiguration";
                public const string GetConfigurationWithUIDisplayByProduct = "GetConfigurationWithUIDisplayByProduct";
            }


            public static class PasswordPolicy
            {
                public const string GetByEndpointSystemId = "GetByEndpointSystemId";
                public const string GetByPolicyId = "GetByPolicyId";
                public const string GetPasswordPolicyIdByName = "GetPasswordPolicyIdByName";
                public const string DoesExists = "DoesExists";
                public const string GetIACMLastPasswordChangeByEndpointUser = "GetIACMLastPasswordChangeByEndpointUser";
                public const string GetByName = "GetByName";
                public const string GetEndpointPasswordPolicy = "GetEndpointPasswordPolicy";
                public const string ConstructGeneralUserPassword = "ConstructGeneralUserPassword";
            }
            public static class PasswordPolicyEndpoint
            {
                public const string GetByPolicyId = "GetByPolicyId";
            }
            public static class AuditConfiguration
            {
                public const string GetByTableName = "GetByTableName";
                public const string GetLogs = "GetLogs";
                public const string GetTableList = "GetTableList";
                public const string GetAuditLogDetail = "GetAuditLogDetail";
                public const string GetEntitiesToAudit = "GetEntitiesToAudit";
                public const string GetAllTableList = "GetAllTableList";
                public const string GetAuditLogRecordDetail = "GetAuditLogRecordDetail";
            }
            public static class AuditConfigurationDetail
            {
                public const string GetPropertiesByTableName = "GetPropertiesByTableName";
                public const string GetByTableProperties = "GetByTableProperties";

            }
        }
        #endregion

        #region User
        public static class User
        {
            public const string UpdateUserADAccount = "UpdateUserADAccount";
            public const string UpdateUserEmailDetail = "UpdateUserEmailDetail";
            public const string GetByUsrId = "GetByUsrId";
            public const string Authenticate = "Authenticate";
            public const string GetByIdIncludeBusinessRoleAndOrganisation = "GetByIdIncludeBusinessRoleAndOrganisation";
            public const string GetActiveByUserName = "GetActiveByUserName";
            public const string GetByUserName = "GetByUserName";
            public const string GetByNRIC = "GetByNRIC";
            public const string GetByEmployeeId = "GetByEmployeeId";
            public const string UpdateManagerForUsers = "UpdateManagerForUsers";
            public const string DoesADIDExists = "DoesADIDExists";
            public const string DoesEmployeeExists = "DoesEmployeeExists";
            public const string DoesExists = "DoesExists";
            public const string DoesIdentificationNumberExists = "DoesIdentificationNumberExists";
            public const string GetActiveEmployees = "GetActiveEmployees";
            public const string GetEmployeeId = "GetEmployeeId";
            public const string GetCompanyDetails = "GetCompanyDetails";
            public const string GetMembers = "GetMembers";
            public const string UpdateLastLogonInformation = "UpdateLastLogonInformation";
            public const string GetOnBoardingUsers = "GetOnBoardingUsers";
            public const string GetOffBoardingUsers = "GetOffBoardingUsers";
            public const string GetOUChangeUsers = "GetOUChangeUsers";
            public const string GetReOrgUsers = "GetReOrgUsers";
            public const string UpdateUserFromDiscovery = "UpdateUserFromDiscovery";
            public const string GetNManagerFromOrg = "GetNManagerFromOrg";
            public const string InsertUserRole = "InsertUserRole";
            public const string CheckUserGroup = "CheckUserGroup";
            public const string GetEndPointDetailByID = "GetEndPointDetailByID";
            public const string GetAutoGeneratedNRICCount = "GetAutoGeneratedNRICCount";
            public const string GetAssigneeInfo = "GetAssigneeInfo";
            public const string GetReportingEmployees = "GetReportingEmployees";
            public const string ConstructUserName = "ConstructUserName";
            public const string GetBySearchString = "GetBySearchString";
            public const string GetROUsersBySearchString = "GetROUsersBySearchString";
            public const string GetBySearchStringAndConsiderJoinDate = "GetBySearchStringAndConsiderJoinDate";
            public const string GetReferenceUser = "GetReferenceUser";
            public const string GetReferenceUserOnlyPermanent = "GetReferenceUserOnlyPermanent"; 
            public const string GetReportingManager = "GetReportingManager";
            public const string GetCostCenters = "GetCostCenters";
            public const string GetOrganisationsWithPrefix = "GetOrganisationsWithPrefix";
            public const string GetUsersByGroup = "GetUsersByGroup";
            public const string GetNonReportingUsersOfManagerID = "GetNonReportingUsersOfManagerID";
            public const string GetActiveAndNonTerminaters = "GetActiveAndNonTerminaters";
            public const string GetDisticntSectionFromUsers = "GetDisticntSectionFromUsers";
            public const string GetPosition = "GetPosition";
            public const string GetByIdReferenceUser = "GetByIdReferenceUser";
            public const string GetNotifiableUsers = "GetNotifiableUsers";
            public const string GetIdByUserName = "GetIdByUserName";
            public const string ForgotUser = "ForgotUser";
            public const string SaveUser = "SaveUser";
            public const string GetByUserId = "GetByUserId";
            public const string GetGroupIds = "GetGroupIds";
            public const string ComposeNewUserCredentialMail = "ComposeNewUserCredentialMail";
            public const string GetPIMUsersBySearchString = "GetPIMUsersBySearchString";
            public const string GetByIsPIMUser = "GetByIsPIMUser";
            public const string GetUsersByOnscreenException = "GetUsersByOnscreenException";
            public const string GetBackkDatedExceptions = "GetBackkDatedExceptions";
            public const string GetFullTimeUsers = "GetFullTimeUsers";
            public const string GetVendorUsers = "GetVendorUsers";
            public const string GetUsersForSupervisor = "GetUsersForSupervisor";
            public const string GetUsersWithSameGradeOrAbove = "GetUsersWithSameGradeOrAbove";
            public const string GetRequesteeInfo = "GetRequesteeInfo";
            public const string WriteLoginResult = "WriteLoginResult";
            public const string GetROByUser = "GetROByUser";
            public const string GetCOByUser = "GetCOByUser";
            public const string GetUsersForBulkRequest = "GetUsersForBulkRequest";

            public const string GetPersonnelAreaById = "GetPersonnelAreaById";
            public const string GetCompanyById = "GetCompanyById";
            public const string GetPersonnelSubAreaById = "GetPersonnelSubAreaById";
            public const string GetOrganisationByCode = "GetOrganisationByCode";
            public const string GetAllOrganisations = "GetAllOrganisations";
            public const string GetCompanyByCode = "GetCompanyByCode";

            public const string GetGroupsById = "GetGroupsById";
            public const string GetRolesById = "GetRolesById";
            public const string DoAfterSaveUser = "DoAfterSaveUser";
            public const string GetOUChangeUsersForAdminCheck = "GetOUChangeUsersForAdminCheck";
            public const string GetReOrgUsersForAdminCheck = "GetReOrgUsersForAdminCheck";
            public const string GetOnBoardUsersForAdminCheck = "GetOnBoardUsersForAdminCheck";
            public const string GetReHireUsersForAdminCheck = "GetReHireUsersForAdminCheck";
            public const string IsProvisioningDefaultExists = "IsProvisioningDefaultExists";
            public const string IsAdminUser = "IsAdminUser";
            public const string GetOrgCompanies = "GetOrgCompanies";
            public const string GetOrgDepartments = "GetOrgDepartments";
            public const string GetOrgDivisions = "GetOrgDivisions";
            public const string GetOrgBranches = "GetOrgBranches";
            public const string GetOrgSections = "GetOrgSections";
            public const string GetOrgUnits = "GetOrgUnits";
            public const string GetReportingUsersOfThisManagerIDandAdmin_New = "GetReportingUsersOfThisManagerIDandAdmin_New";
            public const string GetAllEmployeesUnderRO = "GetAllEmployeesUnderRO";
            public const string GetUserAccessDetailMatrixGridData = "GetUserAccessDetailMatrixGridData";
            public const string IsADDefaultsExists = "IsADDefaultsExists";
            public const string IsSMSDefaultsExists = "IsFLINSDefaultsExists";
            public const string IsFLINSDefaultsExists = "IsFLINSDefaultsExists";
            public const string GetGroupMembersbyUserId = "GetGroupMembersbyUserId";
            public const string GetUsersByGroupId = "GetUsersByGroupId";
            public const string GetUserAccessDetailMatrixPagedData = "GetUserAccessDetailMatrixPagedData";
            public const string DownloadUserAccessDetail = "DownloadUserAccessDetail";
            public const string CheckDisplayNameDuplicate = "CheckDisplayNameDuplicate";
            public const string GetSameOUUserIDs = "GetSameOUUserIDs";
            public const string PerformLogoutCleanup = "PerformLogoutCleanup";
            public const string GetCustomReportUserBySearchString = "GetCustomReportUserBySearchString";
            public const string GetUserIsActive = "GetUserIsActive";
            public const string IsEmailIdAlreadyExist = "IsEmailIdAlreadyExist";


        }
        #endregion

        #region UserExclusion
        public static class UserExclusion
        {
            public const string SaveUserList = "SaveUserList";
            public const string GetAllUserExclusionByUserID = "GetAllUserExclusionByUserID";
        }
        public static class PresentationUserExclusion
        {
            public const string GetAllUserExclusion = "GetAllUserExclusion";
        }
        #endregion

        #region UserAttachment
        public static class UserAttachment
        {
            public const string GetByUserId = "GetByUserId";
        }
        #endregion

        #region Group
        public static class Group
        {
            public const string DeleteUserFromGroup = "DeleteUserFromGroup";
            public const string DoesExist = "DoesExist";
            public const string GetAllGroups = "GetAllGroups";
            public const string GetAllGroupsByEndpointID = "GetAllGroupsByEndpointID";
            public const string GetByIdIncludingUser = "GetByIdIncludingUser";
            public const string DeleteGroupByID = "DeleteGroupByID";
            public const string CheckUserGroup = "CheckUserGroup";
            public const string GetByGroupName = "GetByGroupName";
        }
        #endregion

        #region

        public static class NDesignation
        {
            public const string GetById = "GetById";
        }

        public static class NGrade
        {
            public const string GetById = "GetById";
        }

        #endregion

        #region Organisation
        public static class Org
        {
            public static class Organisation
            {
                public const string GetOrgUnitPrefixById = "GetOrgUnitPrefixById";
                public const string GetByOrgUnitPrefix = "GetByIdIncludeBusinessRoleAndOrganisation";
                public const string GetNOrganisations = "GetNOrganisations";
                public const string GetParentOrgs = "GetParentOrgs";
                public const string GetByParentOrgId = "GetByParentOrgId";
                public const string GetTopmostByHeadId = "GetTopmostByHeadId";
                public const string GetOrganisationsByLevel = "GetNOrganisationsByLevel";
                public const string GetOrganisationsByName = "GetOrganisationsByName"; 
            }

            public static class Company
            {
                public const string GetCompanyName = "GetCompanyName";
                public const string GetByCompanyName = "GetByCompanyName";
                public const string GetAllCompanies = "GetAllCompanies";
                public const string isExists = "isExists";
            }
            public static class Department
            {
                public const string GetDepartmentName = "GetDepartmentName";
                public const string GetDepartments = "GetDepartments";
                public const string GetAllDepartments = "GetAllDepartments";
            }
            public static class Division
            {
                public const string GetByDepartmentId = "GetByDepartmentId";
                public const string GetByWithFormattedDivisionCode = "GetByWithFormattedDivisionCode";
            }
            public static class CostCenter
            {
                public const string Get = "Get";
            }
            public static class NGrade
            {
                public const string GetByCode = "GetByCode";
                public const string GetNGrade = "GetNGrade";
            }
            public static class NPlan
            {
                public const string GetNPlan = "GetNPlan";
            }
            public static class NOrganisation
            {
                public const string GetById = "GetById";
                public const string GetOrgChart = "GetOrgChart";
                public const string GetNOrganisations = "GetNOrganisations";
                public const string GetOrgChartForDisplay = "GetOrgChartForDisplay";
                public const string GetOrganisationScopeUsers = "GetOrganisationScopeUsers";
                public const string GetByOrgCode = "GetByOrgCode";
                public const string GetByOrganisationCode = "GetByOrganisationCode";
                public const string GetByScopeId = "GetByScopeId";
                public const string GetDepartMents = "GetDepartMents";
                public const string GetCostCenters = "GetCostCenters";
                public const string GetBusinessUnits = "GetBusinessUnits";
                public const string GetCompanies = "GetCompanies";
                public const string GetInternalCompanies = "GetInternalCompanies";
                public const string GetExternalCompanies = "GetExternalCompanies";
                public const string GetLocations = "GetLocations";

                public const string GetDivisionsByDeptID = "GetDivisionsByDeptID";
                public const string GetDisticntSectionFromUsers = "GetDisticntSectionFromUsers";
                public const string GetCostCenterCodeAndTextByCompanyID = "GetCostCenterCodeAndTextByCompanyID";
                public const string GetOrganisations = "GetOrganisations";
            }
            public static class NOrganisationHead
            {
                public const string IsOrgHead = "IsOrgHead";
                public const string GetOrganisationHead = "GetOrganisationHead";
            }
            public static class NOrganisationScope
            {
                public const string GetAllOrganisationScope = "GetAllOrganisationScope";
                public const string GetOrganisationScope = "GetOrganisationScope";
            }
            public static class PresentationOrgChart
            {
                public const string GetOrgChartNodes = "GetOrgChartNodes";
            }
        }
        #endregion
        #region PresentationMyRequestData
        public static class PresentationMyRequestData
        {
            public const string GetByProgressStatus = "GetByProgressStatus";
            public const string GetByAdhocId = "GetByAdhocId";
        }
        #endregion
        #region AlertCentreAttachment
        public static class AlertCentreAttachment
        {
            public const string GetByAccessRequestId = "GetByAccessRequestId";
            public const string GetAlertCenterDetails = "GetAlertCenterDetails";
        }
        #endregion
        #region AlertCenterNotification
        public static class AlertCenterNotification
        {
            public const string GetAlertCenterNotifications = "GetAlertCenterNotifications";
            public const string GetAlertCenterNotificationsByEntityId = "GetAlertCenterNotificationsByEntityId";
        }
        #endregion

        #region UserImage
        public static class UserImage
        {
            public const string GetImageByUserId = "GetImageByUserId";
        }
        #endregion

        #region Preference
        public static class Preference
        {
            public static class UserGridFilterInfo
            {
                public const string Getfiltervalue = "Getfiltervalue";
            }
            public static class AdminModeItem
            {
                public const string DoesExists = "DoesExists";
                public const string GetAllIncludingPageUrl = "GetAllIncludingPageUrl";
            }

            public static class QuickLink
            {
                public static class QL
                {
                    public const string GetByName = "GetByName";
                    public const string DoesExists = "DoesExists";
                    public const string GetByRoleId = "GetByRoleId";
                }

                public static class QLAcessRights
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetByCategory = "GetByCategory";
                    public const string GetQuickLinkAccessRights = "GetQuickLinkAccessRights";
                }

                public static class DefaultRoleQuicklink
                {
                    public const string DoesExists = "DoesExists";
                    public const string DoesExistsByPosition = "DoesExistsByPosition";

                    public const string GetQLCatagoriesByRoleID = "GetQLCatagoriesByRoleID";
                }
            }
            public static class Dashboard
            {
                public const string GetByDashBoardCategory = "GetByDashBoardCategory";
                public const string GetDashboardCategoryByRoleId = "GetDashboardCategoryByRoleId";
                public const string DoesExists = "DoesExists";
                public const string GetDashboardType = "GetDashboardType";
                public const string GetByRoleId = "GetByRoleId";
                public const string GetByRoleIdAndCategory = "GetByRoleIdAndCategory";
            }

            public static class DefaultRoleDashboardPreference
            {
                public const string DoesExists = "DoesExists";
                public const string GetByIdWithNavigaton = "GetByIdWithNavigaton";
                public const string GetAllOrderedWithNavigaton = "GetAllOrderedWithNavigaton";
                public const string DoesExistsByPosition = "DoesExistsByPosition";
            }

            public static class UserPreference
            {
                public const string GetDefaultMenus = "GetDefaultMenus";
                public const string GetUserPreferenceMenus = "GetUserPreferenceMenus";
            }

            public static class DashboardAccessRights
            {
                public const string DoesExists = "DoesExists";
                public const string GetAllWithNavigation = "GetAllWithNavigation";
                public const string Delete = "Delete";
            }

            public static class UserFilterInfo
            {
                public const string GetFilterByUserId = "GetFilterByUserId";
                public const string Delete = "Delete";
            }

            public static class UserGridInfo
            {
                public const string GetByUserId = "GetByUserId";
            }

            public static class UserColumnInfo
            {
                public const string GetcolumnsByUserId = "GetcolumnsByUserId";
                public const string Delete = "Delete";
            }

            public static class UserGridAllInfo
            {
                public const string GetByUserId = "GetByUserId";
            }

            public static class PresentationDashBoardPreference
            {
                public const string GetUserDashboards = "GetUserDashboards";
                public const string GetDefaultDashboards = "GetDefaultDashboards";
            }
            public static class PresentationDefaultRoleQuickLink
            {
                public const string GetDefaultRoleQLRights = "GetDefaultRoleQLRights";
            }
            public static class UserCulturePreference
            {
                public const string GetByUserId = "GetByUserId";
            }

            public static class UserDashboardPreference
            {
                public const string Delete = "Delete";
            }

            public static class UserQuickLink
            {
                public const string Delete = "Delete";
                public const string GetByUserId = "GetByUserId";
            }
        }
        #endregion

        #region Notification
        public static class Notification
        {
            public static class PresentationNotification
            {
                public const string GetByUserIdStatus = "GetByUserIdStatus";
                public const string GetByTaskId = "GetByTaskId";
                public const string GetByRequestId = "GetByRequestId";
                public const string GetReplyChainById = "GetReplyChainById";
                public const string GetNotificationsById = "GetNotificationsById";
            }

            public static class PresentationRequestActionUsers
            {
                public const string GetActionUsersByRequestId = "GetActionUsersByRequestId";
            }
        }
        #endregion

        #region Attachment
        public static class Attachment
        {
            public const string GetByRequestId = "GetByRequestId";
            public const string GetAttachmentCount = "GetAttachmentCount";
        }
        #endregion

        #region Task
        public static class Tasks
        {
            public static class Presentation
            {
                public static class PresentationTask
                {
                    public const string GetAllGroupTask = "GetAllGroupTask";
                    public const string GetCompletedTask = "GetCompletedTask";
                    public const string GetTaskdetail = "GetTaskdetail";
                    public const string GetAll = "GetAll";
                    public const string GetByStatus = "GetByStatus";
                    public const string GetByActionUserId = "GetByActionUserId";
                    public const string GetTaskDetailsByTaskStatus = "GetTaskDetailsByTaskStatus";
                    public const string GetGroupTaskDetailsByActionUserId = "GetGroupTaskDetailsByActionUserId";
                    public const string GetGroupTaskDetailsByTaskStatus = "GetGroupTaskDetailsByTaskStatus";
                    public const string GetAllExecutorTask = "GetAllExecutorTask";
                    public const string GetAllExecutorTaskReport = "GetAllExecutorTaskReport";
                    public const string GetByWorkflowInstanceId = "GetByWorkflowInstanceId";
                    public const string GetPIMTaskDetailsByTaskStatus = "GetPIMTaskDetailsByTaskStatus";
                    public const string GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign = "GetTaskDetailsByActionUserIdAndTaskStatusForBulkAssign";
                }

                public static class PresentationPreviousApprovers
                {
                    public const string GetByWorkflowInstanceId = "GetByWorkflowInstanceId";
                }
                
               
            }
            public static class PresentationTaskdelegation
            {
                public const string GetAllDelegationByActionUser = "GetAllDelegationByActionUser";
                public const string GetPagedAllDelegationByActionUser = "GetPagedAllDelegationByActionUser";
                public const string DownloadDelegation = "DownloadDelegation";

            }
            public static class InterimStore
            {
                public const string GetByUserId = "GetByUserId";
                public const string GetForSpecialAccounts = "GetForSpecialAccounts";
                public const string GetByTaskId = "GetByTaskId";
                public const string GetByUserIdAndItemType = "GetByUserIdAndItemType";
                public const string DeleteByUserId = "DeleteByUserId";
                public const string DeleteByInterimId = "DeleteByInterimId";
                public const string GetByType = "GetByType";
                public const string GetFromInterimStoreByUserId = "GetFromInterimStoreByUserId";
            }

            public static class Task
            {
                public const string UpdateAttestationReviewEffectiveDate = "UpdateAttestationReviewEffectiveDate";
                public const string UpdateAllAttestationReviewEffectiveDate = "UpdateAllAttestationReviewEffectiveDate";
                public const string DoesGroupTaskExistsForUser = "DoesGroupTaskExistsForUser";
                public const string GetPendingTaskByAccessRequestId = "GetPendingTaskByAccessRequestId";
                public const string GetTaskInfo = "GetTaskInfo";
                public const string DoesAttestationTasksAlreadyExists = "DoesAttestationTasksAlreadyExists";
                public const string GetProvisioningTask = "GetProvisioningTask";
                public const string GetUserForAttestationTaskReview = "GetUserForAttestationTaskReview";
                public const string GetTaskHasUnsavedChanges = "GetTaskHasUnsavedChanges";
                public const string GetAttestationTypeForTask = "GetAttestationTypeForTask";
                public const string GetDetailsForAttestationTaskReviewByUser = "GetDetailsForAttestationTaskReviewByUser";
                public const string GetDetailsForAttestationTaskReviewByRole = "GetDetailsForAttestationTaskReviewByRole";
                public const string GetEndpointsForAttestationTaskReview = "GetEndpointsForAttestationTaskReview";
                public const string GetEndpointUserForAttestationTaskReview = "GetEndpointUserForAttestationTaskReview";
                public const string GetEndpointEntitlementsForAttestationTaskReview = "GetEndpointEntitlementsForAttestationTaskReview";
                public const string GetUserEntitlementForAttestationTaskReview = "GetUserEntitlementForAttestationTaskReview";
                public const string GetEntitlementUsersForAttestationTaskReview = "GetEntitlementUsersForAttestationTaskReview";
                public const string UpdateAttestationReviewAction = "UpdateAttestationReviewAction";
                public const string UpdateAttestationReviewAll = "UpdateAttestationReviewAll";
                public const string UpdateRoleBasedAttestationReview = "UpdateRoleBasedAttestationReview";
                public const string UpdateAttestationReviewComments = "UpdateAttestationReviewComments";
                public const string GetAttestationTaskReviewForExportFlat = "GetAttestationTaskReviewForExportFlat";
                public const string GetAttestationTaskReviewForExportHierarchy = "GetAttestationTaskReviewForExportHierarchy";
                public const string SaveAttestationReview = "SaveAttestationReview";
                public const string RevertAttestationReview = "RevertAttestationReview";
                public const string GetTaskCompleteness = "GetTaskCompleteness";
                public const string GetCampaignTaskDetail = "GetCampaignTaskDetail";
                public const string GetAllCompletedTask = "GetAllCompletedTask";
                public const string Serialize = "Serialize";
                public const string DeSerialize = "DeSerialize";
                public const string GekByAccessRequestId = "GekByAccessRequestId";
                public const string EmailCompose = "EmailCompose";
                public const string DoesExistsByWorkflowConfigurationId = "DoesExistsByWorkflowConfigurationId";
                public const string CanSubmitTask = "CanSubmitTask";
                public const string UpdateAttestationComment = "UpdateAttestationComment";
                public const string SaveAttestationCampaignAttachment = "SaveAttestationCampaignAttachment";
                public const string UpdateAttestationReviewReservedColumn = "UpdateAttestationReviewReservedColumn";
                public const string GetTaskByBulkRequestId = "GetTaskByBulkRequestId";
                public const string GetBulkRequestTask = "GetBulkRequestTask";
                

            }

            public static class TaskDelegation
            {
                public const string CreateSelectedTasksDelegation = "CreateSelectedTasksDelegation";
                public const string GetByActionUser = "GetByActionUser";
                public const string CreateAdvanceTasksDelegationEmail = "CreateAdvanceTasksDelegationEmail";
                public const string IsDelegationAllowed = "IsDelegationAllowed";
                public const string GetAdvanceDelegationByActionUser = "GetAdvanceDelegationByActionUser";
            }
        }
        #endregion

        #region PUB

        public static class PUB
        {
            public const string GetLazyLoadPUBFiles = "GetLazyLoadPUBFiles";
        }

        #endregion

        #region Endpoint
        public static class Endpoint
        {
            public static class NPAC
            {
                public const string GetNPACLevelInfoList = "GetNPACLevelInfoList";
                public const string GetPrimaryByDIUserId = "GetPrimaryByDIUserId";
            }

            #region EndPointFunctionType
            public static class EndPointFunctionType
            {
                public const string GetByEndpointSystemTypeId = "GetByEndpointSystemTypeId";
            }
            #endregion

            #region EndPointSystemType
            public static class EndPointSystemType
            {
                public const string GetByIdIncludingEndpoints = "GetByIdIncludingEndpoints";
                public const string GetAllforAttestation = "GetAllforAttestation";
                public const string GetByEndpointSystemTypeName = "GetByEndpointSystemTypeName";
                public const string GetEndPointTypeName = "GetEndPointTypeName";
                public const string GetByName = "GetByName";
                public const string GetByIsActiveForProvisioning = "GetByIsActiveForProvisioning";
                public const string GetActiveEndPointType = "GetActiveEndPointType";
                public const string GetEndpointSystemType = "GetEndpointSystemType";
                public const string GetEndpointPIMSystem = "GetEndpointPIMSystem";
                public const string GetPrivilegeEndpoints = "GetPrivilegeEndpoints";
                public const string GetActiveEndpoints = "GetActiveEndpoints";
                public const string GetByEndpointSystemTypeIdIncludingEndPointSystemType = "GetByEndpointSystemTypeIdIncludingEndPointSystemType";

                public static string EndPointSystemMethod(string MethodName = "")
                {
                    return MethodName;
                }

            }
            #endregion

            public static class EndpointObjects
            {
                public const string GetByEndpointSystemTypeId = "GetByEndpointSystemTypeId";
                public const string GetTrustedSourceObjectsByEndpointSystemType = "GetTrustedSourceObjectsByEndpointSystemType";
                public const string GetByEndpointIdKeyName = "GetByEndpointIdKeyName";
            }

            #region EndPoint
            public static class EndPoint
            {
                public const string GetEndPointByApplicationTypeNonTrusted = "GetEndPointByApplicationTypeNonTrusted";
                public const string GetEndPointByApplicationType = "GetEndPointByApplicationType";
                public const string GetByEndpointSystemTypeIdTrustedSource = "GetByEndpointSystemTypeIdTrustedSource";
                public const string GetByEndpointSystemTypeIdIsActive = "GetByEndpointSystemTypeIdIsActive";
                public const string GetByEndpointSystemTypeId = "GetByEndpointSystemTypeId";
                public const string GetByEnPoint = "GetByEnPoint";
                public const string GetByIdIncludingOwner = "GetByIdIncludingOwner";
                public const string GetByEndpointType = "GetByEndpointType";
                public const string DoesExists = "DoesExists";
                public const string DoesExistsbyApplicationCode = "DoesExistsbyApplicationCode";
                public const string GetNonTrustedSource = "GetNonTrustedSource";
                public const string GetByIdIncludingEndPointSystemType = "GetByIdIncludingEndPointSystemType";
                public const string SaveConfiguration = "SaveConfiguration";
                public const string GetConfiguration = "GetConfiguration";
                public const string GetAllIncludingEndPointSystemType = "GetAllIncludingEndPointSystemType";
                public const string GetByEndpointTypeIncludingOwner = "GetByEndpointTypeIncludingOwner";
                public const string GetByDisplayNameIncludingEndPointSystemType = "GetByDisplayNameIncludingEndPointSystemType";
                public const string Delete = "Delete";
                public const string GetSelectedSystem = "GetSelectedSystem";
                public const string GetByTrustedSource = "GetByTrustedSource";
                public const string GetInhouseEndpoints = "GetInhouseEndpoints";
                public const string GetInhouseMockEndpoints = "GetInhouseMockEndpoints";
                public const string GetByEndpointConfiguration = "GetByEndpointConfiguration";
                public const string GetByDescriptionSearchString = "GetByDescriptionSearchString";
                public const string GetApplicableApplications = "GetApplicableApplications";
                public const string GetAttestationByEndpointSystemTypeIds = "GetAttestationByEndpointSystemTypeIds";
                public const string GetEndpointSystemByIds = "GetEndpointSystemByIds";
                public const string GetByEndpointSystemTypeIds = "GetByEndpointSystemTypeIds";
                public const string GetSingleEntitlements = "GetSingleEntitlements";
                public const string GetPasswordPolicyApplications = "GetPasswordPolicyApplications";
                public const string GetByApplicationCode = "GetByApplicationCode";
                public const string GetApplicationCodeById = "GetApplicationCodeById";
                public const string GetPasswordPolicies = "GetPasswordPolicies";
                public const string GetActiveNonTrustedSrcNonInhouseEndpoints = "GetActiveNonTrustedSrcNonInhouseEndpoints";
                public const string GetActiveEndpoints = "GetActiveEndpoints";
                public const string SaveSubSystemList = "SaveSubSystemList";
                public const string IsActiveEndpointsystem = "IsActiveEndpointsystem";
                public const string GetEndpointHierarchyByEndpoint = "GetEndpointHierarchyByEndpoint";
                public const string GetRequestablePIMEndpoints = "GetRequestablePIMEndpoints";
                public const string GetPrivilegeAssetsByAssetType = "GetPrivilegeAssetsByAssetType";
                public const string GetByEnablePIM = "GetByEnablePIM";
                public const string GetOrgEndpointByType = "GetOrgEndpointByType";
                public const string GetByEndpointTypeAndTrustedSource = "GetByEndpointTypeAndTrustedSource";

                public static class Configuration
                {
                    public static class EndpointConfiguration
                    {
                        public const string GetByEndpointSystemId = "GetByEndpointSystemId";
                        public const string GetByEndpointSystemTypeId = "GetByEndpointSystemTypeId";
                    }

                    public static class EndpointKeyConfiguration
                    {
                        public const string GetByEndpointSystemTypeId = "GetByEndpointSystemTypeId";
                    }

                    public static class EndpointConfigurationKey
                    {
                        public const string GetByKeyName = "GetByKeyName";
                    }

                    public static class EndpointUserViewConfiguration
                    {
                        public const string GetByEndpointSystemId = "GetByEndpointSystemId";
                    }
                    public static class EndpointConfigurationDetail
                    {
                        public const string GetByEndpointConfigurationId = "GetByEndpointConfigurationId";
                        public const string GetByKeyId = "GetByKeyId";

                    }
                }

            }
            public static class PresentationEndpointForFilter
            {
                public const string GetRequestableEndpoints = "GetRequestableEndpoints";
            }
            #endregion

            #region ActiveDirectory
            public static class ActiveDirectory
            {
                public static class ADSUserInfo
                {
                    public const string GetByIdIncludeADSGroups = "GetByIdIncludeADSGroups";
                    public const string DoesExists = "DoesExists";
                    public const string GetBysAMAccountName = "GetBysAMAccountName";
                    public const string GetByDIUserId = "GetByDIUserId";
                    public const string GetSharedADSUserInfoByName = "GetSharedADSUserInfoByName";
                    public const string DoesExistActiveADSUserInfoByAccountType = "DoesExistActiveADSUserInfoByAccountType";
                }

                public static class ADSGroupInfo
                {
                    public const string GetByName = "GetByName";
                    public const string GetByCn = "GetByCn";
                    public const string GetAllByGroupTypeAndEndpointId = "GetAllByGroupTypeAndEndpointId";
                    public const string GetADGroupsByUserId = "GetADGroupsByUserId";
                    public const string GetAllByName = "GetAllByName";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetADSGroupBySearchString = "GetADSGroupBySearchString";
                    public const string GetADGroupsByUserIdAndEndpointId = "GetADGroupsByUserIdAndEndpointId";
                }

                public static class ADSOuInfo
                {
                    public const string GetBySearchString = "GetBySearchString";
                    public const string GetDefaultOU = "GetDefaultOU";
                }
            }
            #endregion

            #region LotusNotes
            public static class LotusNotes
            {
                public const string GetEntitlementDetail = "GetEntitlementDetail";
                public static class LotusGroupInfo
                {
                    public const string GetByGroup = "GetByGroup";
                    public const string GetBySearchString = "GetBySearchString";
                }

                public static class LotusUserInfo
                {
                    public const string DoesLastNameExists = "DoesLastNameExists";
                    public const string DoesShortNameExists = "DoesShortNameExists";
                }
            }
            #endregion

            #region SAP
            public static class SAP
            {


                public static class SAPRoleinfo
                {
                    public const string GetByRoleCode = "GetByRoleCode";
                    public const string GetSAPRoles = "GetSAPRoles";
                    public const string GetSAPRoleBySearchString = "GetSAPRoleBySearchString";
                }

                public static class SAPUserinfo
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetSAPUserByUserId = "GetSAPUserByUserId";
                    public const string GetPositionTextByUser = "GetPositionTextByUser";
                    public const string GetActivePrimarySAPUserByDIUserId = "GetActivePrimarySAPUserByDIUserId";
                    public const string GetConflictRemarks = "GetConflictRemarks";
                    public const string GetSAPSODConflicts = "GetSAPSODConflicts";
                    public const string GetSAPSODConflictsShowingrid = "GetSAPSODConflictsShowingrid"; 
                }

                public static class SAPGroupInfo
                {
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                }

                public static class SAPProfileInfo
                {
                    public const string GetByProfileCode = "GetByProfileCode";
                    public const string GetByEndpointSystemId = "GetByEndpointSystemId";
                    public const string GetSAPProfileBySearchString = "GetSAPProfileBySearchString";
                }

                public static class SAPParameterHistoryInfo
                {
                    public const string GetSAPParameters = "GetSAPParameters";
                }

                public static class SAPAuthorizationToObject
                {
                    public const string GetByObjectCode = "GetByObjectCode";
                }

                public static class SAPObjectInfo
                {
                    public const string GetSapFields = "GetSapFields";
                    public const string GetDictionaryByObjCode = "GetDictionaryByObjCode";
                    public const string GetByRoleCode = "GetByRoleCode";
                }

                public static class SAPTCodeInfo
                {
                    public const string GetByRoleCode = "GetByRoleCode";
                    public const string GetByRoleCodeIncludingRoles = "GetByRoleCodeIncludingRoles";
                    public const string GetByTCode = "GetByTCode";
                    public const string GetSapRolesToTCodeData = "GetSapRolesToTCodeData";
                    public const string GetByEndpointId = "GetByEndpointId";
                    public const string GetSAPTCodeBySearchString = "GetSAPTCodeBySearchString";
                }
            }
            #endregion

            #region SAPEP
            public static class SAPEP
            {
                public static class SAPEPUserinfo
                {
                    public const string DoesExists = "DoesExists";
                }
                public static class SAPEPGroupInfo
                {
                    public const string GetByName = "GetByName";
                    public const string GetByEndPointId = "GetByEndPointId";
                    public const string GetPortalGroups = "GetPortalGroups";
                    public const string GetGroupSAPEP = "GetGroupSAPEP";
                    public const string GetGroupSAPEPForRemove = "GetGroupSAPEPForRemove";
                    public const string GetGroupsById = "GetGroupsById";
                    public const string GetPortalGroupsByActionType = "GetPortalGroupsByActionType";
                }
                public static class SAPEPRoleInfo
                {
                    public const string GetByName = "GetByName";
                    public const string GetByEndPointId = "GetByEndPointId";
                    public const string GetPortalRoles = "GetPortalRoles";
                    public const string GetRoleSAPEP = "GetRoleSAPEP";
                    public const string GetRolesSAPEPForRemove = "GetRolesSAPEPForRemove";
                    public const string GetRolesById = "GetRolesById";
                    public const string GetPortalRolesByActionType = "GetPortalRolesByActionType";

                }
            }
            #endregion


            #region LINUX
            public static class LINUX
            {
                //public const string GetAllOrderByLogDate = "GetAllOrderByLogDate";
                public static class LinuxUserInfo
                {
                    public const string GetUnLockedUserInfosById = "GetUnLockedUserInfosById";

                }
                public static class LinuxGroupInfo
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetListByGroupNameEndPointSystemId = "GetListByGroupNameEndPointSystemId";
                    public const string GetRolesbySearchstring = "GetRolesbySearchstring";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                }

                public const string GetEntitlementDetail = "GetEntitlementDetail";
            }
            #endregion

            public static class SRM
            {
                public const string GetSAPSRMRoles = "GetSAPSRMRoles";
                public const string GetAllSAPSRMRoles = "GetAllSAPSRMRoles";

                public static class SAPSRMBusinessRoleMappings
                {
                    public const string GetSAPSRMPgrps = "GetSAPSRMPgrps";
                    public const string GetSAPSRMPgrp = "GetSAPSRMPgrp";
                    public const string GetSAPSRMEndpointUsers = "GetSAPSRMEndpointUsers";
                    public const string GetSAPSRMPgrpEndpointUsers = "GetSAPSRMPgrpEndpointUsers";
                    public const string GetSAPSRMPorg = "GetSAPSRMPorg";
                    public const string GetSAPSRMPorgs = "GetSAPSRMPorgs";
                    public const string GetSAPSRMUserRoles = "GetSAPSRMUserRoles";
                }

                public static class SAPSRMMainRoleMatrix
                {
                    public const string IsAdGroupsExist = "IsAdGroupsExist";
                }
            }
            public static class O365
            {
                public const string GetO365UserByUserId = "GetO365UserByUserId";
                public const string GetAdhocOffice365ServicePlans = "GetAdhocOffice365ServicePlans";
                public const string GetAdhocOffice365LicenseInfos = "GetAdhocOffice365LicenseInfos";
                public const string GetLicenseInfoForBulkReq = "GetLicenseInfoForBulkReq";
                public const string GetO365Users = "GetO365Users";
                public const string GetOffice365ServicePlansforBulkReq = "GetOffice365ServicePlansforBulkReq";
            }

            public static class OneCard
            {
                public const string GetAllBuildings = "GetAllBuildings";
                public const string GetSectionByBuilding = "GetSectionByBuilding";
                public const string GetTechRolesByBuildingandSection = "GetTechRolesByBuildingandSection";
                public const string GetExistingRolesByEndpointUserId = "GetExistingRolesByEndpointUserId";
                public const string GetDescription = "GetDescription";
                public const string GetBuildingandSectionByTechRoles = "GetBuildingandSectionByTechRoles";
                public const string GetDistinctTechRoles = "GetDistinctTechRoles";
                public const string GetExistingTechRoles = "GetExistingTechRoles";
            }

            #region OracleEBS
            public static class OracleEBS
            {
                //public const string GetAllOrderByLogDate = "GetAllOrderByLogDate";
                public const string GetByIsLocked = "GetByIsLocked";
                public static class OracleEBSRoleInfo
                {
                    public const string GetByGroup = "GetByGroup";
                    public const string GetBySearchString = "GetBySearchString";
                    public const string GetAllByName = "GetAllByName";
                    public const string DoesExists = "DoesExists";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetByRoleCode = "GetByRoleCode";
                    public const string GetOEBSRoles = "GetOEBSRoles";
                    public const string GetOEBSRoleBySearchString = "GetOEBSRoleBySearchString";
                    public const string GetAllByRoleTypeAndEndpointId = "GetAllByRoleTypeAndEndpointId";
                }
                public static class OracleEBSResponsibilitiesInfo
                {
                    public const string GetAllByName = "GetAllByName";
                    public const string GetAllOEBSRespByEP = "GetAllOEBSRespByEP";
                    public const string DoesExists = "DoesExists";
                    public const string GetByResposilbilityKey = "GetByResposilbilityKey";
                    public const string GetBySearchString = "GetBySearchString";
                }
            }
            #endregion

            #region ORCLUM
            public static class ORCLUM
            {
                //public const string GetAllOrderByLogDate = "GetAllOrderByLogDate";
                public static class OracleUMRoleInfo
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetRolesbySearchstring = "GetRolesbySearchstring";
                }
            }
            #endregion


            #region MSSQL
            public static class MSSQL
            {
                //public const string GetAllOrderByLogDate = "GetAllOrderByLogDate";
                public const string GetByIsLocked = "GetByIsLocked";
                public static class MSSQLUMUserInfo
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetByIdIncludeMSSQLRoles = "GetByIdIncludeMSSQLRoles";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";

                }


                public static class MSSQLUMRoleInfo
                {
                    public const string GetByName = "GetByName";
                    public const string DoesExists = "DoesExists";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetAllByGroupTypeAndEndpointId = "GetAllByGroupTypeAndEndpointId";
                    public const string GetADGroupsByUserId = "GetADGroupsByUserId";
                    public const string GetAllByName = "GetAllByName";
                }
            }
            #endregion

            #region GEMS
            public static class GEMS
            {
                //public const string GetAllOrderByLogDate = "GetAllOrderByLogDate";
                public const string GetGEMSUsersByDIUserId = "GetGEMSUsersByDIUserId";
            }
            #endregion

            #region SSIDS
            public static class SSIDS
            {
                public const string GetAllRoles = "GetAllRoles";
            }
            #endregion

            #region Inhouse Systems
            public static class InhouseSystems
            {
                public const string DeleteByUserid = "DeleteByUserid";
                public const string EndpointHasAnyRoleAndAssignedToUser = "EndpointHasAnyRoleAndAssignedToUser";
                public const string SaveInhouseMockInfo = "SaveInhouseMockInfo";
                public const string GetInhousePermissionGridData = "GetInhousePermissionGridData";
                public const string GetInhouseFunctionGridData = "GetInhouseFunctionGridData";
                public const string GetInhouseFunctionActive = "GetInhouseFunctionActive";
                public const string GetInhUserPermissionFunctionGridData = "GetInhUserPermissionFunctionGridData";
                public const string CheckAccountExistsInDependentSystems = "CheckAccountExistsInDependentSystems";
                public const string DeleteByAccReconId = "DeleteByAccReconId";
                public static class InhouseUserInfo
                {
                    public const string GetByIdIncludeEndPoint = "GetByIdIncludeEndPoint";
                    public const string GetByUserName = "GetByUserName";
                    public const string MapUser = "MapUser";
                    public const string DoesExists = "DoesExists";
                    public const string EmailCompose = "EmailCompose";
                    public const string GetByUserNameAndEndPointSystemId = "GetByUserNameAndEndPointSystemId";
                }
                public static class InhousePermission
                {
                    public const string GetByIdIncludeEndpoint = "GetByIdIncludeEndpoint";
                    public const string GetByIdIncludeEndpointFunction = "GetByIdIncludeEndpointFunction";
                    public const string GetByUserId = "GetByUserId";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string DoesExistsByEndPointSystemId = "DoesExistsByEndPointSystemId";
                    public const string GetPermissionsByEndpointSystemId = "GetPermissionsByEndpointSystemId";
                    public const string GetByNameAndEndpointSystemId = "GetByNameAndEndpointSystemId";
                    public const string GetPermissionsByNames = "GetPermissionsByNames";
                    public const string GetBulkPermissionsByNames = "GetBulkPermissionsByNames";
                    
                }
                public static class InhouseFunction
                {
                    public const string GetByIdIncludeEndpoint = "GetByIdIncludeEndpoint";
                    public const string GetByUserId = "GetByUserId";
                    public const string GetINHFunctionsByRoleId = "GetINHFunctionsByRoleId";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetFunctionWithNoPermissions = "GetFunctionWithNoPermissions";
                    public const string DoesExistsByEndPointSystemId = "DoesExistsByEndPointSystemId";
                    public const string GetPermissionsByEndpointSystemId = "GetPermissionsByEndpointSystemId";
                    public const string GetByRoleId = "GetByRoleId";
                    public const string GetByNameAndEndpointSystemId = "GetByNameAndEndpointSystemId";
                }
                public static class PresentationInhouseSystemEntitlements
                {
                    public const string GetByInhouseSystemsCode = "GetByInhouseSystemsCode";
                }

            }
            #endregion

            #region VPN
            public static class VPNMaster
            {
                public const string Activate = "Activate";
                public const string GetAllActiveList = "GetAllActiveList";
                public const string GetApplications = "GetApplications";
            }
            #endregion
            #region Skypeforbusiness
            public static class SkypeForBusiness
            {
                public const string GetResponseGroup = "GetResponseGroup";
                public const string GetSkypeForBusinessUserById = "GetSkypeForBusinessUserById";
                public const string CheckNumberGeneratedAndAssigned = "CheckNumberGeneratedAndAssigned";
                public const string DeleteNumberRangeAndMatrix = "DeleteRangeAndGeneratedMatrix";
                public const string GetAllByEndpointId = "GetAllByEndpointId";
                public const string GetAllReservedNumbers = "GetAllReservedNumbers";
                public const string GetUserWithResponseGroup = "GetUserWithResponseGroup";
                public const string IsDCCNumberExists = "IsDCCNumberExists";
                public const string GetResponseGroupForBulkRequest = "GetResponseGroupForBulkRequest";
                public const string HasUserHoldAnyResponseGroup = "HasUserHoldAnyResponseGroup";
                public const string GetSFBExtensionMatrixData = "GetSFBExtensionMatrixData";
            }
            #endregion
            #region Exchange
            public static class Exchange
            {
                public static class ExUserInfo
                {
                    public const string GetByIdIncludeExGroups = "GetByIdIncludeExGroups";
                    public const string DoesExists = "DoesExists";
                    public const string GetBysAMAccountName = "GetBysAMAccountName";
                }

                public static class ExGroupInfo
                {
                    public const string GetByName = "GetByName";
                    public const string GetByCn = "GetByCn";
                    public const string DoesExists = "DoesExists";
                    public const string GetAllByGroupTypeAndEndpointId = "GetAllByGroupTypeAndEndpointId";
                    public const string GetGroupbySearchstring = "GetGroupbySearchstring";
                    public const string GetADGroupsByUserId = "GetADGroupsByUserId";
                    public const string GetAllByName = "GetAllByName";
                }
            }
            #endregion


            #region Peoplesoft
            public static class Peoplesoft
            {
                //public const string GetAllOrderByLogDate = "GetAllOrderByLogDate";
                public const string GetByIsLocked = "GetByIsLocked";
                public static class PeoplesoftRoleInfo
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetRolesbySearchstring = "GetRolesbySearchstring";
                    public const string GetByCn = "GetByCn";
                    public const string GetAllByName = "GetAllByName";
                }
                public static class PeoplesoftPermission
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetByResposilbilityKey = "GetByResposilbilityKey";
                }
            }
            #endregion
            #region PeopleSoftFinanceBI
            public static class PeopleSoftFinanceBI
            {
                //public const string GetAllOrderByLogDate = "GetAllOrderByLogDate";
                public const string GetByIsLocked = "GetByIsLocked";
                public const string GetByEndpointSystemId = "GetByEndpointSystemId";
                public static class PeopleSoftFinanceBIRoleInfo
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetRolesbySearchstring = "GetRolesbySearchstring";
                    public const string GetByCn = "GetByCn";
                    public const string GetAllByName = "GetAllByName";
                }
                public static class PeopleSoftFinanceBIPermission
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetByResposilbilityKey = "GetByResposilbilityKey";
                }
                public static class PeopleSoftFinanceBIMenus
                {

                    public const string GetMenusbySearchstring = "GetMenusbySearchstring";

                }
                public static class PeopleSoftFinanceBIPortals
                {

                    public const string GetPortalsbySearchstring = "GetPortalbySearchstring";
                }
                public static class PeopleSoftFinanceBIAccessLogs
                {
                    public const string GetAccessLogsbySearchstring = "GetAccessLogsbySearchstring";
                }
                public static class PeopleSoftFinanceBIRoleClasss
                {
                    public const string GetRoleClasssbySearchstring = "GetRoleClasssbySearchstring";
                }
                public static class PeopleSoftFinanceBIPermissions
                {
                    public const string GetPermissionsbySearchstring = "GetPermissionsbySearchstring";
                }
            }
            #endregion

            #region Windows
            public static class Windows
            {
                public static class WinUserInfo
                {
                    public const string GetByIdIncludeADSGroups = "GetByIdIncludeADSGroups";
                    public const string DoesExists = "DoesExists";
                    public const string GetBysAMAccountName = "GetBysAMAccountName";
                }

                public static class WinGroupInfo
                {
                    public const string GetByName = "GetByName";
                    public const string GetByCn = "GetByCn";
                    public const string GetAllByGroupTypeAndEndpointId = "GetAllByGroupTypeAndEndpointId";
                    public const string GetADGroupsByUserId = "GetADGroupsByUserId";
                    public const string GetAllByName = "GetAllByName";
                    public const string GetRolesbySearchstring = "GetRolesbySearchstring";
                    public const string DoesExists = "DoesExists";
                    public const string GetListByGroupNameEndPointSystemId = "GetListByGroupNameEndPointSystemId";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                }
            }
            #endregion


            #region  Presentation
            public static class Presentation
            {
                public const string GetByCriteria = "GetByCriteria";
                public const string GetByPIMCriteria = "GetByPIMCriteria";
                public static class INH
                {
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetByEndPointSystemIdExisting = "GetByEndPointSystemIdExisting";
                    public const string GetAllPermissionsExceptExisting = "GetAllPermissionsExceptExisting";
                    public const string GetUserToPermissionsReportData = "GetUserToPermissionsReportData";
                    public const string GetPermissionToFunctionsReportData = "GetPermissionToFunctionsReportData";
                    public const string GetAccountWithNoPermissionsReportData = "GetAccountWithNoPermissionsReportData";
                    public const string GetPermissionWithNoAccountsReportData = "GetPermissionWithNoAccountsReportData";
                    public const string GetPermissionWithNoFunctionsReportData = "GetPermissionWithNoFunctionsReportData";
                }
                public static class AD
                {
                    public const string GetADSUserToGroupsReportData = "GetADSUserToGroupsReportData";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetADSActiveAccountsWithInActiveUser = "GetADSActiveAccountsWithInActiveUser";
                    public const string GetADSUserByDormantDaysReportData = "GetADSUserByDormantDaysReportData";
                    public const string GetADSUserByLockedDisabledReportData = "GetADSUserByLockedDisabledReportData";
                    public const string GetADSAccountWithNoGroupsReportData = "GetADSAccountWithNoGroupsReportData";
                    public const string GetADSGroupWithNoAccountsReportData = "GetADSGroupWithNoAccountsReportData";
                    public const string GetADSFuturePasswordExpiryAccountReportData = "GetADSFuturePasswordExpiryAccountReportData";
                    public const string GetADSFutureAccountExpiryAccountReportData = "GetADSFutureAccountExpiryAccountReportData";
                    public const string GetADSUserToGroupsReportDataByDepartment = "GetADSUserToGroupsReportDataByDepartment";
                    public const string GetADSUserToGroupsReportDataByDepartmentAndGroup = "GetADSUserToGroupsReportDataByDepartmentAndGroup";
                }
                public static class Exchange
                {
                    public const string GetExchangeUserToRoleReportData = "GetExchangeUserToRoleReportData";
                    public const string GetUserReport = "GetUserReport";
                }
                public static class PresentationPrivilegedTransaction
                {
                    public const string GetProcessedPrivilegedTXS = "GetProcessedPrivilegedTXS";
                }
                public static class PeopleSoftFinanceBI
                {
                    public const string GetPeopleSoftFinanceBIRoleToPermissionReportData = "GetPeopleSoftFinanceBIRoleToPermissionReportData";
                    public const string GetPeopleSoftFinanceBIUserToRoleReportData = "GetPeopleSoftFinanceBIUserToRoleReportData";
                    public const string GetPeopleSoftFinanceBIUserToCostCenterData = "GetPresentationPeopleSoftFinanceBIUserCostCenter";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetFBIAccountWithNoRolesReportData = "GetFBIAccountWithNoRolesReportData";
                    public const string GetFBIRoleWithNoAccountsReportData = "GetFBIRoleWithNoAccountsReportData";
                    public const string GetFBIAccountWithNoCostCenter = "GetFBIAccountWithNoCostCenter";
                    public const string GetFBICostCenterWithNoAccountsReportData = "GetFBICostCenterWithNoAccountsReportData";
                }

                public static class MSSQL
                {
                    public const string GetMSSQLUserToRoleReportData = "GetMSSQLUserToRoleReportData";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                }
                public static class Linux
                {
                    public const string GetLinuxUserToRoleReportData = "GetLinuxUserToRoleReportData";
                    public const string GetUserReport = "GetUserReport";
                }
                public static class Windows
                {
                    public const string GetWindowsUserToRoleReportData = "GetWindowsUserToRoleReportData";
                    public const string GetUserReport = "GetUserReport";
                }


                public static class LotusNotes
                {
                    public const string GetAllLotusNotesGroupsReportData = "GetAllLotusNotesGroupsReportData";
                    public const string GetLNSGroupToUsersReportData = "GetLNSGroupToUsersReportData";
                    public const string GetLNSUserToGroupsReportData = "GetLNSUserToGroupsReportData";
                    public const string GetAllLotusNotesUsersReportData = "GetAllLotusNotesUsersReportData";
                    public const string GetLotusNotesUserWithNoGroupsReportData = "GetLotusNotesUserWithNoGroupsReportData";
                    public const string GetLotusNotesGroupWithNoUsersReportData = "GetLotusNotesGroupWithNoUsersReportData";
                }
                public static class OracleUM
                {
                    public const string GetOracleUMUserToRoleReportData = "GetOracleUMUserToRoleReportData";
                    public const string GetAllByUsesr = "GetAllByUsesr";
                }

                public static class OEBS
                {
                    public const string GetOEBSUserToRoleReportData = "GetOEBSUserToRoleReportData";
                    public const string GetAllByUsesr = "GetAllByUsesr";
                    public const string GetOEBSUserToRespReportData = "GetOEBSUserToRespReportData";
                }
                public static class SAP
                {
                    public const string GetSAPUserToGroupsReportData = "GetSAPUserToGroupsReportData";
                    public const string GetSAPUserToProfilesReportData = "GetSAPUserToProfilesReportData";
                    public const string GetAllSAPRolesReportData = "GetAllSAPRolesReportData";
                    public const string GetAllSapTCodesReportData = "GetAllSapTCodesReportData";
                    public const string GetAllSAPUsersReportData = "GetAllSAPUsersReportData";
                    public const string GetSapRolesToAuthorizationData = "GetSapRolesToAuthorizationData";
                    public const string GetSAPUsersWithNoRolesReportData = "GetSAPUsersWithNoRolesReportData";
                    public const string GetSAPRolesWithNoUsersReportData = "GetSAPRolesWithNoUsersReportData";
                    public const string GetSAPDormantUsersReportData = "GetSAPDormantUsersReportData";
                    public const string GetSAPUserExpiryAccountReportData = "GetSAPUserExpiryAccountReportData";
                    public const string GetSAPLockedUserReportData = "GetSAPLockedUserReportData";
                    public const string GetSAPUsersToRolesReportData = "GetSAPUsersToRolesReportData";
                    public const string GetSAPRoleToUsersReportData = "GetSAPRoleToUsersReportData";

                }
                public static class SAPEP
                {
                    public const string GetByEndpointSystemId = "GetByEndpointSystemId";
                    public const string GetSAPEPUserToGroupsReportData = "GetSAPEPUserToGroupsReportData";
                    public const string GetSAPEPUserToRolesReportData = "GetSAPEPUserToRolesReportData";
                    public const string GetAllSAPEPUsersReportData = "GetAllSAPEPUsersReportData";
                }

                public static class PresentationUserEntitlement
                {
                    public const string GetEntitlementDetail = "GetEntitlementDetail";
                    public const string GetPermissionDetail = "GetPermissionDetail";
                    public const string GetDetailById = "GetDetailById";
                    public const string GetEntitlementDetailByApplicationType = "GetEntitlementDetailByApplicationType";
                    public const string GetEntitlementDetailByApplicationTypeWithAllCanRequest = "GetEntitlementDetailByApplicationTypeWithAllCanRequest";
                    public const string GetEntitlement = "GetEntitlement";
                    public const string UpdateEntitlement = "UpdateEntitlement";
                    public const string GetUserAccountDetail = "GetUserAccountDetail";
                    public const string GetUserAccountDetailInactiveAccounts = "GetUserAccountDetailInactiveAccounts";
                    public const string GetUserAccountEntitlment = "GetUserAccountEntitlment";
                    public const string GetEntitlementHierachyDetail = "GetEntitlementHierachyDetail";
                    public const string GetEntitlementList = "GetEntitlementList";
                    public const string GetEntitlementName = "GetEntitlementName";
                }
                public static class PresentationAttestationEndPoint
                {
                    public const string GetAttestationEndPoints = "GetAttestationEndPoints";
                }
                public static class PresentationUserAccount
                {
                    public const string GetUserAccountDetail = "GetUserAccountDetail";
                    public const string GetUserSelfServiceAccountDetail = "GetUserSelfServiceAccountDetail";
                    public const string GetAccountDetailByAccountType = "GetAccountDetailByAccountType";
                    public const string GetAllUserAccountDetail = "GetAllUserAccountDetail";
                    public const string GetUserAccountDetailByEndPointSystemId = "GetUserAccountDetailByEndPointSystemId";
                }
                public static class PresentationUser
                {
                    public const string GetEmployees = "GetEmployees";
                    public const string GetNonEmployees = "GetNonEmployees";
                    public const string GetResolvedReconcilations = "GetResolvedReconcilations";
                    public const string GetActiveEmployees = "GetActiveEmployees";
                    public const string GetUserGroup = "GetUserGroup";
                    public const string GetUserRole = "GetUserRole";
                    public const string GetMembers = "GetMembers";
                    public const string GetEntitlementDetailsForAccessRequest = "GetEntitlementDetailsForAccessRequest";
                    public const string DownloadFullTimeEmployees = "DownloadFullTimeEmployees";
                    public const string GetLazyLoadFullTimeEmployees = "GetLazyLoadFullTimeEmployees";
                    public const string DownloadNonEmployees = "DownloadNonEmployees";
                    public const string GetLazyLoadAuditTrial = "GetLazyLoadOtherSystemAuditTrial";
                    public const string DownloadOtherSystemAuditTrial = "DownloadOtherSystemAuditTrial";
                }

                public static class PresentationAnonymousUser
                {
                    public const string GetUserByUserName = "GetUserByUserName";
                }
                public static class PresentationAttestation
                {
                    public const string GetAttestations = "GetAttestations";
                    public const string GetAttestationReviewers = "GetAttestationReviewers";
                    public const string GetAttestationRejectedReviewers = "GetAttestationRejectedReviewers";
                    public const string GetAttestationReviews = "GetAttestationReviews";
                    public const string GetRejectedAttestationReviews = "GetRejectedAttestationReviews";
                    public const string GetAttestationReviewEndpointSystems = "GetAttestationReviewEndpointSystems";
                    public const string GetRejectedAttestationReviewEndpointSystems = "GetRejectedAttestationReviewEndpointSystems";
                    public const string GetAttestationReviewEndpointAccessDetails = "GetAttestationReviewEndpointAccessDetails";
                }
                public static class PresentationAccessRequestItem
                {
                    public const string GetAllEntitlementForProvisioningRole = "GetAllEntitlementForProvisioningRole";
                    public const string GetByAccountReconcilationId = "GetByAccountReconcilationId";
                }

                public static class PresentationUserAccessDetail
                {

                    public const string GetUserAccessDetail = "GetUserAccessDetail";
                }
                public static class PeopleSoft
                {
                    public const string GetPeopleSoftUserToRoleReportData = "GetPeopleSoftUserToRoleReportData";
                    public const string GetAllByUsesr = "GetAllByUsesr";
                }
                public static class PresentationEndpointProvisioningField
                {
                    public const string GetEndpointProvisiongField = "GetEndpointProvisiongField";
                    public const string GetEndpointProvisiongEventProcess = "GetEndpointProvisiongEventProcess";
                    public const string GetEndpointProvisiongEndpointProcess = "GetEndpointProvisiongEndpointProcess";
                }
                public static class PresentationRoleAccess
                {
                    public const string GetAccessToRoles = "GetAccessToRoles";
                }
                public static class BizCMS
                {
                    public const string GetPresentationBIZCMSRoleFunctionsData = "GetPresentationBIZCMSRoleFunctionsData";
                    public const string GetPresentationBIZCMSUserRoleData = "GetPresentationBIZCMSUserRoleData";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetBizCMSAccountWithNoRolesReportData = "GetBizCMSAccountWithNoRolesReportData";
                    public const string GetBizCMSRoleWithNoAccountsReportData = "GetBizCMSRoleWithNoAccountsReportData";
                    public const string GetBizCMSRoleWithNoFunctionsReportData = "GetBizCMSRoleWithNoFunctionsReportData";
                    public const string GetBizCMSUserByDormantDaysReportData = "GetBizCMSUserByDormantDaysReportData";
                }

                public static class WMS
                {
                    public const string GetPresentationWMSRoleFunctionsData = "GetPresentationWMSRoleFunctionsData";
                    public const string GetPresentationWMSUserRoleData = "GetPresentationWMSUserRoleData";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetWMSAccountWithNoRolesReportData = "GetWMSAccountWithNoRolesReportData";
                    public const string GetWMSRoleWithNoAccountsReportData = "GetWMSRoleWithNoAccountsReportData";
                    public const string GetWMSRoleWithNoFunctionsReportData = "GetWMSRoleWithNoFunctionsReportData";
                    public const string GetWMSUserByDormantDaysReportData = "GetWMSUserByDormantDaysReportData";
                }

                public static class CSC
                {
                    public const string GetPresentationCSCRoleFunctionsData = "GetPresentationCSCRoleFunctionsData";
                    public const string GetPresentationCSCUserRoleData = "GetPresentationCSCUserRoleData";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetCSCAccountWithNoRolesReportData = "GetCSCAccountWithNoRolesReportData";
                    public const string GetCSCRoleWithNoAccountsReportData = "GetCSCRoleWithNoAccountsReportData";
                    public const string GetCSCRoleWithNoFunctionsReportData = "GetCSCRoleWithNoFunctionsReportData";
                    public const string GetCSCUserByDormantDaysReportData = "GetCSCUserByDormantDaysReportData";
                }

                public static class LCS
                {
                    public const string GetPresentationLCSRoleFunctionsData = "GetPresentationLCSRoleFunctionsData";
                    public const string GetPresentationLCSUserRoleData = "GetPresentationLCSUserRoleData";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetLCSAccountWithNoRolesReportData = "GetLCSAccountWithNoRolesReportData";
                    public const string GetLCSRoleWithNoAccountsReportData = "GetLCSRoleWithNoAccountsReportData";
                    public const string GetLCSRoleWithNoFunctionsReportData = "GetLCSRoleWithNoFunctionsReportData";
                    public const string GetLCSUserByDormantDaysReportData = "GetLCSUserByDormantDaysReportData";
                }
                public static class RCS
                {
                    public const string GetPresentationRCSRoleFunctionsData = "GetPresentationRCSRoleFunctionsData";
                    public const string GetPresentationRCSUserRoleData = "GetPresentationRCSUserRoleData";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetRCSAccountWithNoRolesReportData = "GetRCSAccountWithNoRolesReportData";
                    public const string GetRCSRoleWithNoAccountsReportData = "GetRCSRoleWithNoAccountsReportData";
                    public const string GetRCSRoleWithNoFunctionsReportData = "GetRCSRoleWithNoFunctionsReportData";
                    public const string GetRCSUserByDormantDaysReportData = "GetRCSUserByDormantDaysReportData";
                }

                public static class ESV
                {
                    public const string GetPresentationESVRoleFunctionsData = "GetPresentationESVRoleFunctionsData";
                    public const string GetPresentationESVUserRoleData = "GetPresentationESVUserRoleData";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetESVAccountWithNoRolesReportData = "GetESVAccountWithNoRolesReportData";
                    public const string GetESVRoleWithNoAccountsReportData = "GetESVRoleWithNoAccountsReportData";
                    public const string GetESVRoleWithNoFunctionsReportData = "GetESVRoleWithNoFunctionsReportData";
                    public const string GetESVUserByDormantDaysReportData = "GetESVUserByDormantDaysReportData";
                }
                public static class PeopleSoftFINOrPROC
                {
                    public const string GetPeopleSoftFINOrPROCUserRoleData = "GetPeopleSoftFINOrPROCUserRoleData";
                    public const string GetFSCUserWithNoRoleData = "GetFSCUserWithNoRoleData";
                    public const string GetFSCRoleWithNoUser = "GetFSCRoleWithNoUser";
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetPresentationFSCMRoleFunctionsData = "GetPresentationFSCMRoleFunctionsData";
                    public const string GetPresentationFSCMFunctionRolesData = "GetPresentationFSCMFunctionRolesData";
                    public const string GetPeopleSoftFINOrPROCUserBankAccount = "GetPeopleSoftFINOrPROCUserBankAccount";
                    public const string GetPeopleSoftFINOrPROCBankAccountUser = "GetPeopleSoftFINOrPROCBankAccountUser";
                    public const string GetPeopleSoftFINOrPROCUserBUDescription = "GetPeopleSoftFINOrPROCUserBUDescription";
                    public const string GetPeopleSoftFINOrPROCBUDescriptionUser = "GetPeopleSoftFINOrPROCBUDescriptionUser";
                }


                public static class PeopleSoftHRMSUserInfo
                {
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetByEndPointSystemIdCount = "GetByEndPointSystemIdCount";
                }
                public static class PeopleSoftFINOrPROCUserInfo
                {
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                }
                public static class PeopleSoftFINOrPROCRoleInfo
                {
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                    public const string GetUserReport = "GetUserReport";
                    public const string GetFSCMRoleWithNoFunctionsReportData = "GetFSCMRoleWithNoFunctionsReportData";
                }
                public static class PeopleSoftFINOrPROCBUDescriptionInfo
                {
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                }
                public static class PeopleSoftFINOrPROCBankDescriptionInfo
                {
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                }
            }
            #endregion

            public static class SAPHANA
            {


                public static class SAPHANARoleinfo
                {
                    public const string GetByRoleCode = "GetByRoleCode";
                    public const string GetSAPRoles = "GetSAPRoles";
                    public const string GetSAPRoleBySearchString = "GetSAPRoleBySearchString";
                }

                public static class SAPHANAUserinfo
                {
                    public const string DoesExists = "DoesExists";
                    public const string GetSAPUserByUserId = "GetSAPUserByUserId";
                    public const string GetPositionTextByUser = "GetPositionTextByUser";
                    public const string GetActivePrimarySAPUserByDIUserId = "GetActivePrimarySAPUserByDIUserId";
                    public const string GetConflictRemarks = "GetConflictRemarks";
                    public const string GetSAPSODConflicts = "GetSAPSODConflicts";
                    public const string GetSAPSODConflictsShowingrid = "GetSAPSODConflictsShowingrid";
                }

                public static class SAPHANAGroupInfo
                {
                    public const string GetByEndPointSystemId = "GetByEndPointSystemId";
                }

                public static class SAPHANAProfileInfo
                {
                    public const string GetByProfileCode = "GetByProfileCode";
                    public const string GetByEndpointSystemId = "GetByEndpointSystemId";
                    public const string GetSAPProfileBySearchString = "GetSAPProfileBySearchString";
                }

                public static class SAPHANAParameterHistoryInfo
                {
                    public const string GetSAPParameters = "GetSAPParameters";
                }

                public static class SAPHANAAuthorizationToObject
                {
                    public const string GetByObjectCode = "GetByObjectCode";
                }

                public static class SAPHANAObjectInfo
                {
                    public const string GetSapFields = "GetSapFields";
                    public const string GetDictionaryByObjCode = "GetDictionaryByObjCode";
                    public const string GetByRoleCode = "GetByRoleCode";
                }

                public static class SAPHANATCodeInfo
                {
                    public const string GetByRoleCode = "GetByRoleCode";
                    public const string GetByRoleCodeIncludingRoles = "GetByRoleCodeIncludingRoles";
                    public const string GetByTCode = "GetByTCode";
                    public const string GetSapRolesToTCodeData = "GetSapRolesToTCodeData";
                    public const string GetByEndpointId = "GetByEndpointId";
                    public const string GetSAPTCodeBySearchString = "GetSAPTCodeBySearchString";
                }
            }
        }

        #region Provisioning
        public static class Provisioning
        {

            public static class EmployeeEventTrigger
            {
                public const string GetTriggerToExecute = "GetTriggerToExecute";
                public const string GetByUserAndEvent = "GetByUserAndEvent";
                public const string GetByUserAndEventIncludingInactive = "GetByUserAndEventIncludingInactive";
                public const string CreateEventTrigger = "CreateEventTrigger";
                public const string CreateEmployeeEventTrigger = "CreateEmployeeEventTrigger";
                public const string UdpateResignationEventTrigger = "UdpateResignationEventTrigger";
                public const string GetEmployeeEventTriggerWaitingForApproval = "GetEmployeeEventTriggerWaitingForApproval";
                public const string CreatePropagationTrigger = "CreatePropagationTrigger";
            }

            public static class AccountReconcilation
            {
                public const string GetByIdIncludingEndPoint = "GetByIdIncludingEndPoint";
                public const string GetByAccountName = "GetByAccountName";
                public const string GetBySystem = "GetBySystem";
                public const string GetByAccountType = "GetByAccountType";
                public const string GetActiveAccountsByAccountType = "GetActiveAccountsByAccountType";
                public const string GetByUser = "GetByUser";
                public const string GetByUserId = "GetByUserId";
                public const string AddOrUpdateAccountReconcilation = "AddOrUpdateAccountReconcilation";
                public const string GetByAccountTypeAndEndpointSystemId = "GetByAccountTypeAndEndpointSystemId";
                public const string GetInactiveAccounts = "GetInactiveAccounts";
                public const string GetEndpointUserInfoJson = "GetEndpointUserInfoJson";
                public static string SaveAccount = "SaveAccount";
                public const string GetUserByException = "GetUserByException";
                public const string GetByUserIdAndEndpointSystemId = "GetByUserIdAndEndpointSystemId";
                public const string GetByPIMSystem = "GetByPIMSystem";
                public const string GetEntitlementName = "GetEntitlementName";
                public const string SaveAccountReconcilationPassword = "SaveAccountReconcilationPassword";
                public const string IsSelfAccount = "IsSelfAccount";
                public const string DoesExistByAccountNameAndEndpointSystemId = "DoesExistByAccountNameAndEndpointSystemId";
                public const string GetByUserIdsDoesNotExist = "GetByUserIdsDoesNotExist";
                public const string GetByAccountTypeAndApplicationType = "GetByAccountTypeAndApplicationType";
                public const string CheckDuplicateRequestExist = "CheckDuplicateRequestExist";
                public const string DoesAccountExistInAccountRecon = "DoesAccountExistInAccountRecon";
                public const string DoesAccountNameExistInTheDependentSystems = "DoesAccountNameExistInTheDependentSystems";
                public const string GetByAccountTypeAndEndpointSystemIdAndAccountName = "GetByAccountTypeAndEndpointSystemIdAndAccountName";
                public const string ProcessAccountMapping = "ProcessAccountMapping";
                public const string GetByEndpointUserId = "GetByEndpointUserId";
            }
            public static class ProvisioningNameConstructor
            {
                public const string GetAllProvisioningFieldBySearchString = "GetAllProvisioningFieldBySearchString";
            }
            public static class EntitlementReconcilation
            {
                public const string GetByUser = "GetByUser";
                public const string GetByAccountReconcilationId = "GetByAccountReconcilationId";
                public const string GetByReconcilationId = "GetByReconcilationId";
                public const string GetByUserEntitlement = "GetByUserEntitlement";
                public const string GetAccountEntitlementRecons = "GetAccountEntitlementRecons";
                public const string GetByUserEntitlementByType = "GetByUserEntitlementByType";
                public const string GetByUserEntitlementByExceptGroup = "GetByUserEntitlementByExceptGroup";
            }

            public static class AccessRequestHistory
            {
                public const string GetByAccessRequestId = "GetByAccessRequestId";
            }

            public static class DefaultProvisioningFormGeneralData
            {
                public const string GetByEndpointSystemTypeId = "GetByEndpointSystemTypeId";
            }

            public static class AccessRequest
            {
                public const string GetYearsByActivationDate = "GetYearsByActivationDate";
                public const string GetByIdIncludeAdHocAccessRequest = "GetByIdIncludeAdHocAccessRequest";
                public const string GetByProgressStatus = "GetByProgressStatus";
                public const string GetAllByProgressStatus = "GetAllByProgressStatus";
                public const string GetByUserId = "GetByUserId";
                public const string GetByTransactionId = "GetByTransactionId";
                public const string GetMonthsByActivationDate = "GetMonthsByActivationDate";
                public const string GetUserYears = "GetUserYears";
                public const string GetUsersMonths = "GetUsersMonths";
                public const string GetByAdHocAccessRequestId = "GetByAdHocAccessRequestId";
                public const string CreateProvisioningTasks = "CreateProvisioningTasks";
                public const string EmailCompose = "EmailCompose";
                public const string GetJustSubmittedRequest = "GetJustSubmittedRequest";
                public const string SaveRemoveRequest = "SaveRemoveRequest";
                public const string SaveExtendedRequest = "SaveExtendedRequest";
                public const string GetExtendedRequestById = "GetExtendedRequestById";
                public const string GetByAccountType = "GetByAccountType";
                public const string RemoveGamingApplications = "CreateAccessRequestToRemoveGamingApplications";
                public const string GetAccessRequestById = "GetAccessRequestById";
                public const string GetIDASRequestByUserId = "GetIDASRequestByUserId";
                public const string IsBulkRequestHasPendingChildRequests = "IsBulkRequestHasPendingChildRequests";
                public const string GetFailedSystemRequests = "GetFailedSystemRequests";
                public const string GetAdhocRequestsByProgressStatus = "GetAdhocRequestsByProgressStatus";
                public const string GetPagedAllRequestDataByProgressStatus="GetPagedAllRequestDataByProgressStatus";
            }
            public static class AdhocAccessRequest
            {
                public const string GetByRequesteeUserId = "GetByRequesteeUserId";
            }
            public static class PresentationOrg
            {
                public const string GetAllOrganizationInfo = "GetAllOrganizationInfo";
            }


            public static class ProvisioningConfiguration
            {
                public const string GetADGroups = "GetADGroups";
                public const string GetLotusNotesCertifiers = "GetLotusNotesCertifiers";
                public const string GetCostCenters = "GetCostCenters";
                public const string GetADGroupsByCanRequest = "GetADGroupsByCanRequest";
                public const string GetLNSGroupsByCanRequest = "GetLNSGroupsByCanRequest";
                public const string GetCostCenterByCompanyID = "GetCostCenterByCompanyID";
                public const string GetCostCenterCodeAndTextByCompanyID = "GetCostCenterCodeAndTextByCompanyID";
            }
            public static class Presentation
            {

                public static class PresentationOUChangeSnapshots
                {
                    public const string GetByUserId = "GetByUserId";
                }

                public static class PresentationAccessSnapshots
                {
                    public const string GetByUserId = "GetByUserId";
                    public const string ExecuteUndoProvisioningEntitlements = "ExecuteUndoProvisioningEntitlements";
                }

                public static class PresentationEmployeeEventTrigger
                {
                    public const string GetById = "GetById";
                    public const string GetAll = "GetAll";
                    public const string GetByEventType = "GetByEventType";
                    public const string GetLazyLoadEmployeeEventTrigger = "GetLazyLoadEmployeeEventTrigger"; 
                }


                public static class PresentationApprovalChain
                {
                    public const string GetByRequestId = "GetByRequestId";
                    public const string GetByBulkRequestId = "GetByBulkRequestId";
                    public const string GetByEmployeeRequestId = "GetByEmployeeRequestId";
                    public const string GetByAdhocRequestId = "GetByAdhocRequestId";
                }

                public static class PresentationAccountReconcilationReport
                {
                    public const string GetExceptions = "GetExceptions";
                    public const string GetBySystem = "GetBySystem";
                }

                public static class PresentationEntitlementReconcilationReport
                {
                    public const string GetExceptions = "GetExceptions";
                }
                public static class PresentationAlertCenterNotification
                {
                    public const string GetDashboardNotification = "GetDashboardNotification";
                    public const string GetReportNotification = "GetReportNotification";
                }
            }
        }
        #endregion

        #region Workflow
        public static class Workflow
        {
            public static class WorkflowConfiguration
            {
                public const string GetWorkflowConfigurationDetailById = "GetWorkflowConfigurationDetailById";
                public const string GetByWorkflowName = "GetByWorkflowName";
                public const string GetByRequestTypeId = "GetByRequestTypeId";
                public const string GetBySearchString = "GetBySearchString";
                public const string DoesExists = "DoesExists";
                public const string CanProceedToSecondStep = "CanProceedToSecondStep";
                public const string CanProceedToSecondStepBulkRequest = "CanProceedToSecondStepBulkRequest";
            }

            #region Workflow Presentation
            public static class Presentation
            {
                public static class PresentationWorkflowApproverGridData
                {
                    public const string GetApproversList = "GetApproversList";
                }
                public static class PresentationWorkflowCriteriaBase
                {
                    public const string GetWorkFlowCriteria = "GetWorkFlowCriteria";
                }
                public static class PresentationWorkflow
                {
                    public const string GetWorkflowList = "GetWorkflowList";
                }
            }
            #endregion
        }
        #endregion

        #region Discovery
        public static class Discovery
        {
            public static class PresentationDiscoveryProcess
            {
                public const string GetDiscoveryProcessJobs = "GetDiscoveryProcessJobs";
                public const string GetDiscoveryProcessJobsByEndPointSystemId = "GetDiscoveryProcessJobsByEndPointSystemId";
            }

            public static class DiscoveryProcess
            {
                public const string GetByIdIncludeEndpoint = "GetByIdIncludeEndpoint";
                public const string DoesProcessNameAlreadyExists = "DoesProcessNameAlreadyExists";
                public const string DeleteDiscoveryProcess = "DeleteDiscoveryProcess";

            }
        }
        #endregion

        #region AdhocRequest
        public static class PresentationAdHocAccessRequest
        {
            public const string GetRequestsByActionUserId = "GetRequestsByActionUserId";
            public const string GetFailedRequestDetails = "GetFailedRequestDetails";
        }
        public static class PresentationInterimAdhocRequest
        {
            public const string GetByUser = "GetByUser";
        }
        public static class AdhocRequest
        {
            public const string DoesNewAccountExist = "DoesNewAccountExist";
            public const string IsSameRequestPending = "IsSameRequestPending";
            public const string IsSameRequestPendingSpecialACR = "IsSameRequestPendingSpecialACR";
            public const string GetAccessRequestItemString = "GetAccessRequestItemString";
            public const string GetAccessRequestItemObject = "GetAccessRequestItemObject";
            public const string GetPendingByRequesteeUserId = "GetPendingByRequesteeUserId";
            public const string GetRequestsFromInterimStore = "GetRequestsFromInterimStore";
            public const string GetRemoveRequestsFromInterimStore = "GetRemoveRequestsFromInterimStore";
            public const string GetEntitlementDetail = "GetEntitlementDetail";
            public const string IsDependentRequestPendingForEBSMSSL = "IsDependentRequestPendingForEBSMSSL";
        }
        #endregion

        #region Scheduler
        public static class Schedulers
        {
            public static class Scheduler
            {
                public const string GetDiscoveryScheduler = "GetDiscoveryScheduler";
                public const string GetComplianceScheduler = "GetComplianceScheduler";
                public const string GetAttestationScheduler = "GetAttestationScheduler";
                public const string GetReportingScheduler = "GetReportingScheduler";
                public const string GetReportingJobScheduler = "GetReportingJobScheduler";
                public const string GetAdhocJobScheduler = "GetAdhocJobScheduler";
            }
            public static class Process
            {
                public const string GetByProcessType = "GetByProcessType";
                public const string GetDiscoveryProcessByProcessId = "GetDiscoveryProcessByProcessId";
            }
            public static class ProcessHistory
            {
                public const string GetByProcessId = "GetByProcessId";
            }
        }
        #endregion

        #region Common
        public static class Common
        {
            public static class TableProperties
            {
                public const string GetByTableName = "GetByTableName";
                public const string GetTableList = "GetTableList";
                public const string GetPropertyList = "GetPropertyList";
            }

            public static class AccessRequestSourceByMonth
            {
                public const string GetAccessRequestSourceByYear = "GetAccessRequestSourceByYear";
                public const string GetAccessRequestSourceForUserByYear = "GetAccessRequestSourceForUserByYear";
            }

            public static class AccessRequestSourceChartData
            {
                public const string GetSeriesDataByAccessRequestSource = "GetSeriesDataByAccessRequestSource";
                public const string GetAccessRequestSourceByMonth = "GetAccessRequestSourceByMonth";
            }

            public static class AdhocRequestAccountTyebyMonth
            {
                public const string GetAccessRequestAccountType = "GetAccessRequestAccountType";
                public const string GetUserAccessRequestByAccountType = "GetUserAccessRequestByAccountType";
            }

            public static class AccessRequestProgressStatusByMonth
            {
                public const string GetAccessRequestProgressStatusByUser = "GetAccessRequestProgressStatusByUser";
                public const string GetAccessRequestProgressStatusByAdmin = "GetAccessRequestProgressStatusByAdmin";
            }
            public static class TargetSystemSourceChartData
            {
                public const string GetSeriesDataByTargetSystemSource = "GetSeriesDataByTargetSystemSource";
            }

            public static class TargetSystemSourceByMonth
            {
                public const string GetTargetSystemSourceByYear = "GetTargetSystemSourceByYear";
                public const string GetTargetSystemSourceForUserByYear = "GetTargetSystemSourceForUserByYear";
            }

            public static class TargetSystemProgressStatusByMonth
            {
                public const string GetTargetSystemProgressStatusByUser = "GetTargetSystemProgressStatusByUser";
                public const string GetTargetSystemProgressStatusByAdmin = "GetTargetSystemProgressStatusByAdmin";
            }
        }
        #endregion

        #region MetaData
        public static class MetaData
        {
            public const string UpdateMetadata = "UpdateMetadata";
            public const string GetMetadata = "GetMetadata";

            public static class RequestTypes
            {
                public const string GetByIsActive = "GetByIsActive";
            }
            public static class EmployeeTerm
            {
                public const string GetByIsActive = "GetByIsActive";
            }
            public static class AccountActionCategory
            {
                public const string GetByIsActive = "GetByIsActive";
            }
            public static class AccountCategory
            {
                public const string GetByIsActive = "GetByIsActive";
            }

            public static class ApprovalUserTypes
            {
                public const string GetByIsActive = "GetByIsActive";

            }

            public static class RoleTypes
            {
                public const string GetByIsActive = "GetByIsActive";

            }

        }
        #endregion

        #region XLProcessor
        public static class XLProcessor
        {
            public static class XLData
            {
                public const string ProcessXL = "ProcessXL";
                public const string ProcessXLData = "ProcessXLData";
                public const string ValidateFileColumns = "ValidateFileColumns";
                public const string ProcessXLForEntitlment = "ProcessXLForEntitlment";
                public const string GetWhoUpdatedData = "GetWhoUpdatedData";
            }
            public static class XLDataValidator
            {
                public const string GetXLDataValidator = "GetXLDataValidator";
            }

            private static List<XLTemplates> lstMatrixTemplates;
            public static List<XLTemplates> LstMatrixTemplates
            {
                get
                {
                    if (lstMatrixTemplates == null)
                    {
                        lstMatrixTemplates = new List<XLTemplates>();
                        lstMatrixTemplates.Add(XLTemplates.Inhouse_UserInfo);
                        lstMatrixTemplates.Add(XLTemplates.ADS_PROVISIONINGMATRIX);
                        lstMatrixTemplates.Add(XLTemplates.SAP_RoleBasedOnOrgMgt);
                        lstMatrixTemplates.Add(XLTemplates.SAP_BIRoleCategories);
                        lstMatrixTemplates.Add(XLTemplates.SAP_PIP_SAP_PI_Role);
                        lstMatrixTemplates.Add(XLTemplates.SAP_RoleBasedOnEEGroup);
                        lstMatrixTemplates.Add(XLTemplates.SAP_HRFLINSRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_HRFLINSSuppRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_RoleMandatoryFields);
                        lstMatrixTemplates.Add(XLTemplates.SAP_RoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_HRFLINSConflictedBusinessRoles);
                        lstMatrixTemplates.Add(XLTemplates.SAP_PGR_GRC_Role);
                        lstMatrixTemplates.Add(XLTemplates.SAP_MSSLCriticalTCodesMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_MSSLPositionMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_MSSLRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_EBSCriticalTCodesMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_EBSPositionMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_EBSRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAP_EPRoleInfo);
                        lstMatrixTemplates.Add(XLTemplates.SAP_EPGroupInfo);
                        lstMatrixTemplates.Add(XLTemplates.CE_GroupInfo);
                        lstMatrixTemplates.Add(XLTemplates.CE_RoleInfo);
                        lstMatrixTemplates.Add(XLTemplates.NPACConfigurations);
                        lstMatrixTemplates.Add(XLTemplates.NPAC_Designation);
                        lstMatrixTemplates.Add(XLTemplates.SAPSRMMainRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAPSRMEnablerRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SAPSRMBusinessRoleMapping);
                        lstMatrixTemplates.Add(XLTemplates.GEMSRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.Office365License);
                        lstMatrixTemplates.Add(XLTemplates.SMSProvisioningMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SMSRoleCatalogue);
                        lstMatrixTemplates.Add(XLTemplates.Inhouse_Permissions);
                        lstMatrixTemplates.Add(XLTemplates.Inhouse_Functions);
                        lstMatrixTemplates.Add(XLTemplates.NOVAUserRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.NOVAManagementRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.NOVAExceptionalRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.NOVASoDExceptionsMatrix);
                        lstMatrixTemplates.Add(XLTemplates.Inhouse_UserPermissionFunctionMatrix);
                        lstMatrixTemplates.Add(XLTemplates.ApprovalException);
                        lstMatrixTemplates.Add(XLTemplates.VPNConfigurations);
                        lstMatrixTemplates.Add(XLTemplates.NOVACompositeRoleMatrix);
                        lstMatrixTemplates.Add(XLTemplates.SkypeForBusinessConfigurations);
                    }
                    return lstMatrixTemplates;
                }
                set { lstMatrixTemplates = value; }
            }
        }
        #endregion

        #region PSScript
        public static class PSScriptRunner
        {
            public const string Run = "Run";
        }
        #endregion

        #region WorkflowProcessor
        public static class WorkflowProcessor
        {
            public const string SubmitWorkflowCriteria = "SubmitWorkflowCriteria";
            public const string WithDrawnRequest = "WithDrawnRequest";

        }
        #endregion

        #region Reporting

        public static class Reporting
        {
            public const string GetByReportName = "GetByReportName";
            public const string GetReportId = "GetReportId";
            public const string SaveReportAttributes = "SaveReportAttributes";
            public const string GetReportList = "GetReportList";
            public const string GetReportIncludingAttributes = "GetReportIncludingAttributes";
            public const string DeleteReportProcess = "DeleteReportProcess";

            public static class Presentation
            {
                public const string GetReportingJobs = "GetReportingJobs";
                public const string GetReportingJobsByReportType = "GetReportingJobsByReportType";
            }
            public const string GetReportAttributes = "GetReportAttributes";

            public static class ReportHistory
            {
                public const string GetReportHistoryByReportId = "GetReportHistoryByReportId";
            }
        }
        #endregion

        #region NOrgRevamp
        public static class NRoleGroup
        {
            public const string GetNRoleGroup = "GetNRoleGroup";
            public const string DeleteRoleGroups = "DeleteRoleGroups";
            public const string isExists = "isExists";
            public const string DoesCatalogNameExists = "DoesCatalogNameExists";
            public const string DoesCatalogNameEditExists = "DoesCatalogNameEditExists";
            public const string DoesExistsByCatalogName = "DoesExistsByCatalogName";
            public const string isRoleGroupDetailExists = "isRoleGroupDetailExists";
        }

        public static class NRoleGroupDetail
        {
            public const string GetByRoleGroupId = "GetByRoleGroupId";
        }

        public static class NRoleGroupApprover
        {
            public const string GetByRoleGroupId = "GetByRoleGroupId";
        }
        #endregion

        public static class BizCMS
        {
            public const string GetPeopleSoftFinanceBIRoleToPermissionReportData = "GetPeopleSoftFinanceBIRoleToPermissionReportData";
            public const string GetPeopleSoftFinanceBIUserToRoleReportData = "GetPeopleSoftFinanceBIUserToRoleReportData";
            public const string GetByEndPointSystemId = "GetByEndPointSystemId";
            public const string GetUserReport = "GetUserReport";
        }

        public static class PeopleSoftHRMSUser
        {
            public const string GetPeopleSoftFinanceBIRoleToPermissionReportData = "GetPeopleSoftFinanceBIRoleToPermissionReportData";
            public const string GetPeopleSoftFinanceBIUserToRoleReportData = "GetPeopleSoftFinanceBIUserToRoleReportData";
            public const string GetByEndPointSystemId = "GetByEndPointSystemId";
            public const string GetUserReport = "GetUserReport";
        }

        #endregion

        public static class Matrix
        {
            public static class SAP_GRC
            {
                public const string BindSubModuleFromGRCRole = "BindSubModuleFromGRCRole";
                public const string GetBusinessRoleFromGRCRole = "GetBusinessRoleFromGRCRole";
                public const string BindModuleFromGRCRole = "BindModuleFromGRCRole";
                public const string BindTechnicalFromSAPGRCRole = "BindTechnicalFromSAPGRCRole";
            }
            public static class ADMatrix
            {
                public const string GetADProvisioningMatrix = "GetADProvisioningMatrix";
            }
            public static class SAP_PI
            {
                public const string GetAllSAPPIRoleMatrix = "GetAllSAPPIRoleMatrix";
                public const string BindSubModuleFromPIRole = "BindSubModuleFromPIRole";
                public const string GetBusinessRoleFromPIRole = "GetBusinessRoleFromPIRole";
                public const string BindModuleFromPIRole = "BindModuleFromPIRole";
                public const string BindTechnicalFromSAPPIRole = "BindTechnicalFromSAPPIRole";
            }
            public static class SAP_FLINS_BI
            {
                public const string BindModuleFromFLINSBI = "BindModuleFromFLINSBI";
                public const string BindSubModuleFromFLINSBI = "BindSubModuleFromFLINSBI";
                public const string GetBusinessRoleFromFlinsBIRole = "GetBusinessRoleFromFlinsBIRole";
                public const string BindTechnicalFromFLINSBI = "BindTechnicalFromFLINSBI";
                public const string GetAllSAPBIRoleCategories = "GetAllSAPBIRoleCategories";
                public const string GetBusinessRoles = "GetBusinessRoles";
            }
            public static class SAP_OTHERS
            {
                public const string GetAllSAPGRCRoleMatrix = "GetAllSAPGRCRoleMatrix";
            }
            public static class SAP_MSSL
            {
                public const string GetAllSAPMSSLXLRoleMatrix = "GetAllSAPMSSLXLRoleMatrix";
                public const string GetAllSAPMSSLXLPositionMatrix = "GetAllSAPMSSLXLPositionMatrix";
                public const string GetAllSAPMSSLXLCriticalTCodesMatrix = "GetAllSAPMSSLXLCriticalTCodesMatrix";
                public const string GetMSSLPositionMatrixByPosition = "GetMSSLPositionMatrixByPosition";
                public const string GetPrefix = "GetPrefix";
                public const string BindRoles = "BindRoles";
                public const string GetPositionCode = "GetPositionCode";
                public const string GetRolesByPosition = "GetRolesByPosition";
                public const string BindViewDetails = "BindViewDetails";
                public const string BindPositions = "BindPositions";
            }
            public static class SAP_EBS
            {
                public const string GetAllSAPEBSXLRoleMatrix = "GetAllSAPEBSXLRoleMatrix";
                public const string GetAllSAPEBSXLPositionMatrix = "GetAllSAPEBSXLPositionMatrix";
                public const string GetAllSAPEBSXLCriticalTCodesMatrix = "GetAllSAPEBSXLCriticalTCodesMatrix";
                public const string GetPositionEBS = "GetPositionEBS";
                public const string GetEBSPositionMatrixByPosition = "GetEBSPositionMatrixByPosition";
                public const string BindPositionsEBS = "BindPositionsEBS";
                public const string BindRolesEBS = "BindRolesEBS";
                public const string GetPrefixEBS = "GetPrefixEBS";
                public const string BindViewDetailsEBS = "BindViewDetailsEBS";
                public const string GetEBSRolesByPosition = "GetEBSRolesByPosition";
            }
            public static class SAP_EP
            {
                public const string GetAllSAPEPRoles = "GetAllSAPEPRoles";
                public const string GetAllSAPEPGroups = "GetAllSAPEPGroups";
            }
            public static class NPAC
            {
                public const string GetAllNPACSections = "GetAllNPACSections";
                public const string GetNPACSectionByName = "GetNPACSectionByName";
                public const string GetNPACSectionByCode = "GetNPACSectionByCode";
                public const string GetAllNPACDesignations = "GetAllNPACDesignations";
            }
            public static class SRM
            {
                public const string GetAllSAPSRMEMainRoleMatrix = "GetAllSAPSRMEMainRoleMatrix";
                public const string GetAllSAPSRMEnablerMatrix = "GetAllSAPSRMEnablerMatrix";
                public const string GetAllSAPSRMBusinessRoleMatrix = "GetAllSAPSRMBusinessRoleMatrix";
                public const string IsAdGroupsExist = "IsAdGroupsExist";
            }
            public static class NOVA
            {
                public const string GetExisitingRequestAndRole = "GetExisitingRequestAndRole";
                public const string GetNOVAUserRoleMatrices = "GetNOVAUserRoleMatrices";
                public const string GetNOVASoDExceptionsMatrices = "GetNOVASoDExceptionsMatrices";
                public const string GetNOVAManagementRoleMatrices = "GetNOVAManagementRoleMatrices";
                public const string GetNOVAExceptionalRoleMatrix = "GetNOVAExceptionalRoleMatrix";
                public const string GetNOVACompositeRoleMatrices = "GetNOVACompositeRoleMatrices";
                public const string GetExceptionalMatrixCompanyCode = "GetExceptionalMatrixCompanyCode";
                public const string GetByCompanyCode = "GetByCompanyCode";
                public const string GetNovaJobByMatrixType = "GetNovaJobByMatrixType";
                public const string GetManagementDepartment = "GetManagementDepartment";
                public const string GetManagementMatrixByDepartemnt = "GetManagementMatrixByDepartemnt";
                public const string GetSODMatricesBySectionCode = "GetSODMatricesBySectionCode";
                public const string GetUser = "GetUser";
                public const string GetSAPSection = "GetSAPSection";
            }
            public static class GEMS
            {
                public const string GetGEMSRoleMatrices = "GetGEMSRoleMatrices";
                public const string BindRolesGEMS = "BindRolesGEMS";
                public const string GetByBusinessRole = "GetByBusinessRole";
                public const string GetAllTables = "GetAllTables";
                public const string GetAllColumnsByTableName = "GetAllColumnsByTableName";
            }
            public static class O365
            {
                public const string GetAllOffice365LicenseConfig = "GetAllOfficeLicenseConfig";
            }
            public static class MISC
            {
                public const string GetApproverException = "GetApproverException";
            }
            public static class SMS
            {
                public const string GetAllSMSProvisioningMatrix = "GetAllSMSProvisioningMatrix";
                public const string GetAllSMSRoleCatalogues = "GetAllSMSRoleCatalogues";
            }
            public static class SAP_FLINS
            {
                public static class SAPHRFLINSRoleMatrix
                {
                    public const string GetBusinessRoles = "GetBusinessRoles";
                    public const string GetUserType = "GetUserType";
                    public const string GetCompany = "GetCompany";
                    public const string GetMandatory2DataSource = "GetMandatory2DataSource";
                    public const string BindModule = "BindModule";
                    public const string BindSubModule = "BindSubModule";
                    public const string GetHrFlinsTechnicalRolesByAdhocRequestObject = "GetHrFlinsTechnicalRolesByAdhocRequestObject";
                    public const string GetAllSAPRoleBasedOnPosition = "GetAllSAPRoleBasedOnPosition";
                    public const string GetAllSAPHRFLINSRoleMatrices = "GetAllSAPHRFLINSRoleMatrices";
                    public const string GetAllSAPHRFLINSSuppRoleMatrices = "GetAllSAPHRFLINSSuppRoleMatrices";
                    public const string GetAllSAPRoleMandatoryFields = "GetAllSAPRoleMandatoryFields";
                    public const string GetAllSAPRoleEligibilityMatrices = "GetAllSAPRoleEligibilityMatrices";
                    public const string GetAllSAPHRFLINSConflictRoles = "GetAllSAPHRFLINSConflictRoles";
                    public const string GetAllSAPRoleBasedOnEEGroups = "GetAllSAPRoleBasedOnEEGroups";
                    public const string GetProvisioningConfigMatrixSAPOrg = "GetProvisioningConfigMatrixSAPOrg";
                    public const string GetEndpointUserBusinessRoles = "GetEndpointUserBusinessRoles";
                    public const string GetEndpointUserBusinessRolesIncudeSupplementary = "GetEndpointUserBusinessRolesIncudeSupplementary";
                    public const string GetAllSAPSODConflictRoles = "GetAllSAPSODConflictRoles";
                    public const string GetAllSAPSODendpointtechConflictRoles = "GetAllSAPSODendpointtechConflictRoles";
                    
                }

                public static class SAPRoleMandatoryFields
                {
                    public const string MandatoryFieldIdentification = "MandatoryFieldIdentification";
                    public const string checkBusinessRule = "checkBusinessRule";
                    public const string checkBusinessRoleCompanyOnlyMandatory = "checkBusinessRoleCompanyOnlyMandatory";
                    //public const string GetAllExistingBusinessRolesOnlyPurchaseGroup = "GetAllExistingBusinessRolesOnlyPurchaseGroup";
                }

            }
            public static class VPNMatrix
            {
                public const string GetApplicationProjectMatrix = "GetApplicationProjectMatrix";
                public const string GetApplicationForBulkRequest = "GetApplicationForBulkRequest";
                public const string GetApplicationEnviornment = "GetApplicationEnviornment";
                public const string GetAllVPNMatrix = "GetAllVPNMatrix";
                public const string DeleteVPNMatrixById = "DeleteVPNMatrixById";
            }
            public static class SkypeForBusinessMatrix
            {
                public const string GetAllSkypeForBusinessMatrix = "GetAllSkypeForBusinessMatrix";
                public const string GetSkypeForBusinessMatrixByName = "GetSkypeForBusinessMatrixByName";
            }

            public static class SAPHANA
            {
                public const string GetAllSAPHANAXLRoleMatrix = "GetAllSAPHANAXLRolesMatrix";
                public const string GetAllSAPHANAXLPositionMatrix = "GetAllSAPHANAXLPositionMatrix";
                public const string GetAllSAPHANAXLCriticalTCodesMatrix = "GetAllSAPHANAXLCriticalTCodeMatrix";
                public const string GetPositionSAPHANA = "GetPositionSAPHANA";
                public const string GetSAPHANAPositionMatrixByPosition = "GetSAPHANAPositionMatrixByPosition";
                public const string BindPositionsSAPHANA = "BindPositionsSAPHANA";
                public const string BindRolesSAPHANA = "BindRolesSAPHANA";
                public const string GetPrefixSAPHANA = "GetPrefixSAPHANA";
                public const string BindViewDetailsSAPHANA = "BindViewDetailsSAPHANA";
                public const string GetSAPHANARolesByPosition = "GetSAPHANARolesByPosition";
                public const string GetBusinessRoles = "GetBusinessRoles";
            }
        }

        #region ReportAttachment
        public static class ReportAttachment
        {
            public const string GetReportAttachmentByType = "GetReportAttachmentByType";
        }
        #endregion

        #region ReconcilationAttachment
        public static class ReconcilationAttachment
        {
            public const string SetReconAttachmentInActive = "SetReconAttachmentInActive";
        }
        #endregion

        #region PasswordLetter
        public static class PasswordLetter
        {
            public const string GetByIdWithSection = "GetByIdWithSection";
        }
        #endregion
        
    }
}


