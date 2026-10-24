using AutoMapper;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.DomainModel.Presentation;

namespace DI.Chakra.DomainModel
{
    public class AutoMapperHelper
    {
        public static void BootStrap()
        {

            #region "ACL"
            Mapper.CreateMap<EntityModel.PageUrl, PageUrl>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Permission, Permission>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.WebPage, WebPage>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationPagePermission, PresentationPagePermission>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.PagePermission, PagePermission>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.TabMenu, TabMenu>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationTabMenu, PresentationTabMenu>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.AdminModeItem, AdminModeItem>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.UserPreference, UserPreference>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.QuickLink, QuickLink>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.QuickLinkAccessRights, QuickLinkAccessRights>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.AccessRightHistory, AccessRightHistory>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.AccessRights, AccessRights>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.RoleHistory, RoleHistory>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.PresentationAccessRights, PresentationAccessRights>()
                .ReverseMap();
            #endregion

            #region "Admin"
            Mapper.CreateMap<EntityModel.Role, Role>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationUser, PresentationUser>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationUserExclusion, PresentationUserExclusion>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationAnonymousUser, PresentationAnonymousUser>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.User, User>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.UserExclusion, UserExclusion>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.UserAttachment, UserAttachment>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationReportingEmployees, PresentationReportingEmployees>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.GlobalQuestion, GlobalQuestion>().ReverseMap();

            Mapper.CreateMap<EntityModel.ComplianceType, ComplianceType>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.SODMitigation, SODMitigation>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.GlobalAnswer, GlobalAnswer>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.UserImage, UserImage>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.PasswordActionInfo, PasswordActionInfo>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODExceptionItem, SODExceptionItem>().ReverseMap();
            Mapper.CreateMap<EntityModel.Role, Role>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.QuickLink, QuickLink>()
              .ReverseMap();
            #endregion
            Mapper.CreateMap(typeof(EntityModel.PresentationGridPagedData<>), typeof(PresentationGridPagedData<>)).ReverseMap();
            Mapper.CreateMap(typeof(EntityModel.PresentationGridPagedDataParameter), typeof(PresentationGridPagedDataParameter)).ReverseMap();
            #region AdhocAccessRequest
            Mapper.CreateMap<EntityModel.AdHocAccessRequest, AdHocAccessRequest>()
         .ReverseMap();

            Mapper.CreateMap<EntityModel.AccessRequestConfiguration, AccessRequestConfiguration>()
         .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAdHocAccessRequest, PresentationAdHocAccessRequest>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAdHocRequest, PresentationAdHocRequest>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationMyRequestData, PresentationMyRequestData>().ReverseMap();
            #endregion

            #region "Compliance"

            Mapper.CreateMap<EntityModel.SODRiskLevel, SODRiskLevel>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODRiskScore, SODRiskScore>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODRiskCode, SODRiskCode>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODMitigation, SODMitigation>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.CompliancePolicy, CompliancePolicy>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODConflict, SODConflict>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODConflictFunctions, SODConflictFunctions>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.SODEndPointPolicySelection, SODEndPointPolicySelection>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODException, SODException>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODExceptionItem, SODExceptionItem>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODFunction, SODFunction>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODFunctionItem, SODFunctionItem>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.SODPolicyExecutionDefinition, SODPolicyExecutionDefinition>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.UserSODComplianceReport, UserSODComplianceReport>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.UserQuickLink, UserQuickLink>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.DefaultRoleQuickLink, DefaultRoleQuickLink>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.UserGridFilterInfo, UserGridFilterInfo>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.UserGridInfo, UserGridInfo>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.UserGridColumnInfo, UserGridColumnInfo>()
           .ReverseMap();

            //Mapper.AssertConfigurationIsValid();

            Mapper.CreateMap<EntityModel.Presentation.PresentationComplianceProcess, PresentationComplianceProcess>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.ComplianceProcessHistory, ComplianceProcessHistory>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ComplianceProcess, ComplianceProcess>()
               .ReverseMap();
            #endregion

            #region Common
            Mapper.CreateMap<EntityModel.Group, Group>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.UserAccessDetailMatrixGridData, DomainModel.Presentation.UserAccessDetailMatrixGridData>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.SapRoleToAuthorizationReportData, DomainModel.Presentation.SapRoleToAuthorizationReportData>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.GroupHistory, GroupHistory>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.TableProperties, TableProperties>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.AdhocRequestAccountTyebyMonth, AdhocRequestAccountTyebyMonth>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.AccessRequestSourcePieSeries, AccessRequestSourcePieSeries>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.AccessRequestSourceChartData, AccessRequestSourceChartData>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationcompletedSourcePieSeries, AttestationcompletedSourcePieSeries>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.AccessRequestSourceByMonth, AccessRequestSourceByMonth>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.AccessRequestProgressStatusByMonth, AccessRequestProgressStatusByMonth>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Attention, Attention>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSnapshotActionHistory, PresentationSnapshotActionHistory>()
                .ReverseMap();
            #endregion

            #region Configuration
            Mapper.CreateMap<EntityModel.ConfigurationMaster, ConfigurationMaster>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.ConfigurationMasterPIM, ConfigurationMasterPIM>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.ConfigurationMasterPasswordManagement, ConfigurationMasterPasswordManagement>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.ConfigurationMasterIM, ConfigurationMasterIM>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.ConfigurationMasterDGM, ConfigurationMasterDGM>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.ConfigurationMasterCompliance, ConfigurationMasterCompliance>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.ConfigurationMasterHistory, ConfigurationMasterHistory>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.PasswordSyncConfigurationDetail, PasswordSyncConfigurationDetail>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.ConfigGeneralTrustedSource, ConfigGeneralTrustedSource>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.PasswordPolicy, PasswordPolicy>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.PasswordPolicyEndpoint, PasswordPolicyEndpoint>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.PasswordSyncConfiguration, PasswordSyncConfiguration>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.RequestApprovalConfiguration, RequestApprovalConfiguration>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.AuditConfiguration, AuditConfiguration>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.AuditConfigurationDetail, AuditConfigurationDetail>()
              .ReverseMap();
            #endregion

            #region Database

            Mapper.CreateMap<EntityModel.MySqlConfiguration, MySqlConfiguration>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.SqlConfiguration, SqlConfiguration>()
            .ReverseMap();

            #endregion

            #region Discovery

            Mapper.CreateMap<EntityModel.DiscoveryProcess, DiscoveryProcess>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.DiscoveryProcessHistory, DiscoveryProcessHistory>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationDiscoveryProcess, PresentationDiscoveryProcess>()
            .ReverseMap();

            #endregion

            #region EndpointConfiguration
            Mapper.CreateMap<EntityModel.EndpointConfiguration, EndpointConfiguration>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.EndpointConfigurationDetail, EndpointConfigurationDetail>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.EndpointConfigurationKey, EndpointConfigurationKey>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.EndpointKeyConfiguration, EndpointKeyConfiguration>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.EndpointUserViewConfiguration, EndpointUserViewConfiguration>()
            .ReverseMap();
            #endregion

            #region Email

            Mapper.CreateMap<EntityModel.EmailInfo, EmailInfo>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.EmailInfoHistory, EmailInfoHistory>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.MailServerConfiguration, MailServerConfiguration>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.MailTemplate, MailTemplate>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationMail, PresentationMail>().ReverseMap();

            #endregion

            #region Reconciliation

            Mapper.CreateMap<EntityModel.ExceptionAttribute, ExceptionAttribute>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.ExceptionAttributeChange, ExceptionAttributeChange>()
                .ReverseMap();

            #endregion

            #region MetaData

            Mapper.CreateMap<EntityModel.MetaData, MetaData>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.AccountCategory, AccountCategory>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.AccountActionCategory, AccountActionCategory>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.AccessRequestCategory, AccessRequestCategory>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.AccessRequestProgressTBD, AccessRequestProgressTBD>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.AccessRequestSourceTBD, AccessRequestSourceTBD>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.AccessRequestStatusTBD, AccessRequestStatusTBD>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.ApprovalUserTypes, ApprovalUserTypes>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.UserPreference, UserPreference>()
             .ReverseMap();

            Mapper.CreateMap<EntityModel.UserCulturePreference, UserCulturePreference>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.RoleTypes, RoleTypes>()
             .ReverseMap();

            Mapper.CreateMap<EntityModel.RequestTypes, RequestTypes>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.EmployeeTerm, EmployeeTerm>()
             .ReverseMap();

            Mapper.CreateMap<EntityModel.SubTask, SubTask>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.OffboardingAccountAction, OffboardingAccountAction>()
            .ReverseMap();



            #endregion

            #region Tasks
            Mapper.CreateMap<EntityModel.InterimStore, InterimStore>()
                    .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationInterimStore, PresentationInterimStore>()
                    .ReverseMap();
            Mapper.CreateMap<EntityModel.RequestAttachment, RequestAttachment>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.TaskDelegation, TaskDelegation>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Task, Task>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationTaskdelegation, PresentationTaskdelegation>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationTaskDelegationReport, PresentationTaskDelegationReport>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAttestationTaskSummary, PresentationAttestationTaskSummary>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationTask, PresentationTask>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationPreviousApprovers, PresentationPreviousApprovers>()
             .ReverseMap();
            #endregion

            #region Notifications
            Mapper.CreateMap<EntityModel.Notification, Notification>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.NotificationAttachment, NotificationAttachment>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationNotification, PresentationNotification>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationRequestActionUsers, PresentationRequestActionUsers>()
           .ReverseMap();
            #endregion

            #region Attachments
            Mapper.CreateMap<EntityModel.Attachment, Attachment>()
             .ReverseMap();
            #endregion

            #region Provisioning

            Mapper.CreateMap<EntityModel.Presentation.PresentationAccessRequest, PresentationAccessRequest>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationOrganisation, PresentationOrganisation>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.EntitlementReconcilation, EntitlementReconcilation>()
        .ReverseMap();

            Mapper.CreateMap<EntityModel.AccountReconcilation, AccountReconcilation>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationAccountReconcilationReport, PresentationAccountReconcilationReport>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationResolvedAccountReconcilationReport, PresentationResolvedAccountReconcilationReport>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationOUChangeSnapshots, PresentationOUChangeSnapshots>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationAccessSnapshots, PresentationAccessSnapshots>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.EmployeeEventTrigger, EmployeeEventTrigger>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationEmployeeEventTrigger, PresentationEmployeeEventTrigger>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationEntitlementReconcilation, PresentationEntitlementReconcilation>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationEntitlementReconcilationReport, PresentationEntitlementReconcilationReport>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.OUChangeSnapshot, OUChangeSnapshot>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationOUChangeSnapshots, PresentationOUChangeSnapshots>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationAccessSnapshots, PresentationAccessSnapshots>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.AccessRequest, AccessRequest>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationApprovers, PresentationApprovers>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.FailedRequestGridData, FailedRequestGridData>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.MyRequestsGridData, MyRequestsGridData>().ReverseMap();

            #endregion

            #region Scheduler
            Mapper.CreateMap<EntityModel.Scheduler, Scheduler>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Process, Process>()
       .ReverseMap();
            Mapper.CreateMap<EntityModel.ProcessHistory, ProcessHistory>()
       .ReverseMap();

            #endregion

            #region Dashboard
            Mapper.CreateMap<EntityModel.Dashboard, Dashboard>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.DashboardAccessRights, DashboardAccessRights>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.DefaultRoleDashboardPreference, DefaultRoleDashboardPreference>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.UserDashboardPreference, UserDashboardPreference>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationDashBoardPreference, PresentationDashBoardPreference>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationQuickLinkAccessRights, PresentationQuickLinkAccessRights>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationDefaultRoleQuickLink, PresentationDefaultRoleQuickLink>()
          .ReverseMap();
            #endregion

            #region  EndPoint
            Mapper.CreateMap<EntityModel.EventUndoRequest, EventUndoRequest>().ReverseMap();
            Mapper.CreateMap<EntityModel.EndPoint, EndPoint>().ReverseMap();
            Mapper.CreateMap<EntityModel.EndPointSystemType, EndPointSystemType>().ReverseMap();
            Mapper.CreateMap<EntityModel.EndpointObjects, EndpointObjects>().ReverseMap();
            Mapper.CreateMap<EntityModel.AttributeMappingColumns, AttributeMappingColumns>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPActivityInfo, SAPActivityInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPAuthorizationProfile, SAPAuthorizationProfile>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPAuthorizationToObject, SAPAuthorizationToObject>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPCompositeProfileInfo, SAPCompositeProfileInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.EndPointAutoExpiry, EndPointAutoExpiry>().ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationEndPoint, PresentationEndPoint>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationUserEntitlement, PresentationUserEntitlement>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationExceptionReconData, PresentationExceptionReconData>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAccountDetail, PresentationAccountDetail>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAccessRequestFilterCriteria, PresentationAccessRequestFilterCriteria>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAccessRequestItem, PresentationAccessRequestItem>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationOrgChangeTrack, PresentationOrgChangeTrack>().ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationUserAccount, PresentationUserAccount>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationAccessRequestFilterCriteria, PresentationAccessRequestFilterCriteria>();

            Mapper.CreateMap<EntityModel.Presentation.PresentationEndpointProvisioningField, PresentationEndpointProvisioningField>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationUserDetails, PresentationUserDetails>()
                .ReverseMap();
            #endregion

            #region "IACM"
            Mapper.CreateMap<EntityModel.IACMUserInfo, IACMUserInfo>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.IACMRoleInfo, IACMRoleInfo>()
               .ReverseMap();

            Mapper.CreateMap<EntityModel.IACMGroupInfo, IACMGroupInfo>()
               .ReverseMap();
            #endregion

            #region "ActiveDirectory"

            Mapper.CreateMap<EntityModel.ADSUserInfo, ADSUserInfo>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.ADSGroupInfo, ADSGroupInfo>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSComputerGroup, ADSComputerGroup>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSComputerInfo, ADSComputerInfo>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSGroupInfoHistory, ADSGroupInfoHistory>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSOuInfo, ADSOuInfo>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSOuInfoHistory, ADSOuInfoHistory>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSPrinterInfo, ADSPrinterInfo>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSSharedFolderInfo, ADSSharedFolderInfo>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSUserInfoHistory, ADSUserInfoHistory>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.ADSNestedGroup, ADSNestedGroup>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationADSComputerInfo, PresentationADSComputerInfo>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADSPrinterInfo, PresentationADSPrinterInfo>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADSOUs, PresentationADSOUs>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADSUsers, PresentationADSUsers>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADSUserToGroups, PresentationADSUserToGroups>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADSActiveAccountsWithInActiveUser, PresentationADSActiveAccountsWithInActiveUser>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADSDormantAccount, PresentationADSDormantAccount>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADSFutureAccountExpiryAccount, PresentationADSFutureAccountExpiryAccount>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADSFuturePasswordExpiryAccount, PresentationADSFuturePasswordExpiryAccount>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationRoleAccess, PresentationRoleAccess>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationADUserDetails, PresentationADUserDetails>()
                .ReverseMap();
            #endregion

            #region Office365

            Mapper.CreateMap<EntityModel.Office365GroupInfo, Office365GroupInfo>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.Office365UserInfo, Office365UserInfo>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Office365License, Office365License>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Office365LicenseConfig, Office365LicenseConfig>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Office365MassLicense, Office365MassLicense>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Office365RetentionPolicy, Office365RetentionPolicy>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Office365RoleAssignmentPolicy, Office365RoleAssignmentPolicy>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Office365ServicePlan, Office365ServicePlan>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.Office365SharingPolicy, Office365SharingPolicy>()
           .ReverseMap();


            #endregion

            #region "SAP"

            Mapper.CreateMap<EntityModel.SAPUserinfo, SAPUserinfo>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.SAPActivityInfo, SAPActivityInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPAuthorizationProfile, SAPAuthorizationProfile>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPAuthorizationToObject, SAPAuthorizationToObject>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPCompositeProfileInfo, SAPCompositeProfileInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPCompositeRoleInfo, SAPCompositeRoleInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPSingleRoleInfo, SAPSingleRoleInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPTCodeInfo, SAPTCodeInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPObjectInfo, SAPObjectInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPObjectValues, SAPObjectValues>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPParameterHistoryInfo, SAPParameterHistoryInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPParameterInfo, SAPParameterInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPPosition, SAPPosition>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPProfileInfo, SAPProfileInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHRP1001, SAPHRP1001>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPRoleInfo, SAPRoleInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SapRoleProfileMapping, SapRoleProfileMapping>().ReverseMap();
            Mapper.CreateMap<EntityModel.SapRoleTCodeMapping, SapRoleTCodeMapping>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPRoleToTCodes, SAPRoleToTCodes>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPRoleToAuthMap, SAPRoleToAuthMap>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPTCodeObjectMap, SAPTCodeObjectMap>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPUserHistoryInfo, SAPUserHistoryInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SapUserProfile, SapUserProfile>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPUserToObject, SAPUserToObject>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPEPGroupInfoHistory, SAPEPGroupInfoHistory>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPEPRoleInfoHistory, SAPEPRoleInfoHistory>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPEPRoleInfoTemp, SAPEPRoleInfoTemp>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPEPUserInfoHistory, SAPEPUserInfoHistory>().ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPUserinfo, PresentationSAPUserinfo>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPTCodeReport, PresentationSAPTCodeReport>()
                .ReverseMap();


            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPProfileReport, PresentationSAPProfileReport>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPRoleReport, PresentationSAPRoleReport>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPRoleToAuthorizationReport, PresentationSAPRoleToAuthorizationReport>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPUser, PresentationSAPUser>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPUserRoleTCodeAuth, PresentationSAPUserRoleTCodeAuth>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPUsersToGroups, PresentationSAPUsersToGroups>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPUsersToProfileReport, PresentationSAPUsersToProfileReport>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPUsersToRolesReport, PresentationSAPUsersToRolesReport>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPRoleWithNoUsers, PresentationSAPRoleWithNoUsers>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPRoleToUsers, PresentationSAPRoleToUsers>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPUserDetails, PresentationSAPUserDetails>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.SAPConflictingRoleReport, SAPConflictingRoleReport>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPCriticalTcodeReport, SAPCriticalTcodeReport>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationConflictRemarks, PresentationConflictRemarks>()
                .ReverseMap();

            #endregion

            #region "SAPEP"

            Mapper.CreateMap<EntityModel.SAPEPUserInfo, SAPEPUserInfo>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.SAPEPGroupInfo, SAPEPGroupInfo>()
     .ReverseMap();

            Mapper.CreateMap<EntityModel.SAPEPRoleInfo, SAPEPRoleInfo>()
     .ReverseMap();

            Mapper.CreateMap<EntityModel.SAPFieldValueMapping, SAPFieldValueMapping>()
     .ReverseMap();

            Mapper.CreateMap<EntityModel.SAPGroupHistoryInfo, SAPGroupHistoryInfo>()
     .ReverseMap();
            Mapper.CreateMap<EntityModel.SAPGroupInfo, SAPGroupInfo>()
     .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPEPRoleToUsersReport, PresentationSAPEPRoleToUsersReport>()
     .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPEPGroupToUsersReport, PresentationSAPEPGroupToUsersReport>()
     .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPEPUserInfo, PresentationSAPEPUserInfo>()
     .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPEPUserToGroups, PresentationSAPEPUserToGroups>()
     .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPEPUserToRoles, PresentationSAPEPUserToRoles>()
     .ReverseMap();
            #endregion

            #region "SAP FLINS"

            Mapper.CreateMap<EntityModel.SAPHRFLINSRoleMatrix, DomainModel.SAPHRFLINSRoleMatrix>().ReverseMap();

            Mapper.CreateMap<EntityModel.SAPHRFLINSSuppRoleMatrix, DomainModel.SAPHRFLINSSuppRoleMatrix>().ReverseMap();

            Mapper.CreateMap<EntityModel.ProvisioningConfigMatrixSapEEGroup, DomainModel.ProvisioningConfigMatrixSapEEGroup>().ReverseMap();
            Mapper.CreateMap<EntityModel.ProvisioningConfigMatrixSAPOrg, DomainModel.ProvisioningConfigMatrixSAPOrg>().ReverseMap();
            Mapper.CreateMap<EntityModel.ProvisioningConfigMatrixSAPPosition, DomainModel.ProvisioningConfigMatrixSAPPosition>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPBIRoleCategory, DomainModel.SAPBIRoleCategory>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHRFLINSConflictingRole, DomainModel.SAPHRFLINSConflictingRole>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPRoleMandatoryField, DomainModel.SAPRoleMandatoryField>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPRoleMatrix, DomainModel.SAPRoleMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPSODConflictingRole, DomainModel.SAPSODConflictingRole>().ReverseMap();


            #endregion "SAP FLINS"


            #region "InHouse Systems"

            Mapper.CreateMap<EntityModel.InhouseUserInfo, InhouseUserInfo>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.InhousePermission, InhousePermission>().ReverseMap();
            Mapper.CreateMap<EntityModel.InhouseFunction, InhouseFunction>().ReverseMap();
            Mapper.CreateMap<EntityModel.InhUserPermissionFunction, InhUserPermissionFunction>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationInhouseSystemEntitlements, PresentationInhouseSystemEntitlements>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationUserAccessDetail, PresentationUserAccessDetail>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationInhouseSystem, PresentationInhouseSystem>().ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationInhouseUserInfo, PresentationInhouseUserInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationInhouseUserToPermission, PresentationInhouseUserToPermission>()
               .ReverseMap();
            #endregion

            #region WorkFlow

            Mapper.CreateMap<EntityModel.WorkflowConfiguration, WorkflowConfiguration>().ReverseMap();
            Mapper.CreateMap<EntityModel.WorkflowConfigurationDetail, WorkflowConfigurationDetail>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationWorkflowConfiguration, PresentationWorkflowConfiguration>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationWorkflowApproverGridData, PresentationWorkflowApproverGridData>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationWorkflowCriteriaBase, PresentationWorkflowCriteriaBase>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationWorkflow, PresentationWorkflow>()
            .ReverseMap();



            #endregion

            #region Logs
            Mapper.CreateMap<EntityModel.UILog, UILog>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.ServiceLog, ServiceLog>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationLog, AttestationLog>().ReverseMap();
            Mapper.CreateMap<EntityModel.DiscoveryLog, DiscoveryLog>().ReverseMap();
            Mapper.CreateMap<EntityModel.ProvisioningLog, ProvisioningLog>().ReverseMap();
            Mapper.CreateMap<EntityModel.EmailLog, EmailLog>().ReverseMap();

            #endregion

            #region SOD
            Mapper.CreateMap<EntityModel.ComplianceReportModules, ComplianceReportModules>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.SODRunPolicySelection, SODRunPolicySelection>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.SODRunPolicy, SODRunPolicy>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.SODReport, SODReport>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.SODConflictFunctionMap, SODConflictFunctionMap>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.SODExecutionConfig, SODExecutionConfig>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.SODPolicy, SODPolicy>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.SODPolicyReport, SODPolicyReport>()
               .ReverseMap();
            Mapper.CreateMap<EntityModel.SODPolicyConfiguration, SODPolicyConfiguration>()
              .ReverseMap();
            #endregion

            #region Attestation

            Mapper.CreateMap<EntityModel.Presentation.PresentationReviewer, PresentationReviewer>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationAttestationEndPoint, PresentationAttestationEndPoint>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAttestation, PresentationAttestation>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.Attestation, Attestation>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.AttestationScope, AttestationScope>()
             .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationAttestationScope, PresentationAttestationScope>()
              .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationListAttestation, PresentationListAttestation>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationJobListAttestation, PresentationJobListAttestation>()
        .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationProcess, AttestationProcess>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationEndpoint, AttestationEndpoint>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationProcessHistory, AttestationProcessHistory>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAttestationReviewPageSAP_Level1, Presentation.PresentationAttestationReviewPageSAP_Level1>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAttestationStatusSummary, Presentation.PresentationAttestationStatusSummary>()
              .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSapUserRoleTCodeAuthorization, Presentation.PresentationSapUserRoleTCodeAuthorization>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAttestationEntitlementReviewDetail, PresentationAttestationEntitlementReviewDetail>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationExportAttestationEntitlementReviewDetail, PresentationExportAttestationEntitlementReviewDetail>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationExportAttestationReportForUI, PresentationExportAttestationReportForUI>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationCampaign, AttestationCampaign>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationCampaignAttachment, AttestationCampaignAttachment>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationCampaignReport, AttestationCampaignReport>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationAttetstationCampaign, PresentationAttetstationCampaign>()
             .ReverseMap();
            #endregion

            #region GridSettings
            Mapper.CreateMap<EntityModel.UserGridFilterSettings, UserGridFilterSettings>()
             .ReverseMap();
            Mapper.CreateMap<EntityModel.UserGridSettings, UserGridSettings>()
             .ReverseMap();

            #endregion

            #region Preference

            Mapper.CreateMap<EntityModel.QuickLinkAccessRights, QuickLinkAccessRights>()
           .ReverseMap();

            #endregion

            #region HouseKeeping

            Mapper.CreateMap<EntityModel.PurgeConfig, PurgeConfig>()
                .ReverseMap();
            #endregion

            #region Attestation Revamp
            Mapper.CreateMap<EntityModel.AttestationHierarchy, DomainModel.AttestationHierarchy>()
       .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationSnapshot, DomainModel.AttestationSnapshot>()
        .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationSnapshotAction, DomainModel.AttestationSnapshotAction>()
        .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationSnapshotActionInterim, DomainModel.AttestationSnapshotActionInterim>()
        .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationSnapshotTaskStatus, DomainModel.AttestationSnapshotTaskStatus>()
        .ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationSnapshotActionHistory, DomainModel.AttestationSnapshotActionHistory>()
        .ReverseMap();
            Mapper.CreateMap<EntityModel.EndpointHierarchy, DomainModel.EndpointHierarchy>()
        .ReverseMap();
            #endregion

            #region OrgRevamp

            Mapper.CreateMap<EntityModel.Company, DomainModel.Company>().ReverseMap();
            Mapper.CreateMap<EntityModel.PersonnelArea, DomainModel.PersonnelArea>().ReverseMap();
            Mapper.CreateMap<EntityModel.PersonnelSubArea, DomainModel.PersonnelSubArea>().ReverseMap();


            Mapper.CreateMap<EntityModel.NDesignation, DomainModel.NDesignation>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NCostCenter, DomainModel.NCostCenter>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NOrganisation, DomainModel.NOrganisation>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NOrganisationPresentation, DomainModel.NOrganisationPresentation>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NOrganisationHead, DomainModel.NOrganisationHead>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NOrganisationRelationship, DomainModel.NOrganisationRelationship>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NOrganisationScope, DomainModel.NOrganisationScope>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NProvisioningField, DomainModel.NProvisioningField>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NRuleset, DomainModel.NRuleset>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NRulesetForApplication, DomainModel.NRulesetForApplication>()
                .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationNRulesetForApplication, DomainModel.Presentation.PresentationNRulesetForApplication>()
             .ReverseMap();

            Mapper.CreateMap<EntityModel.NRulesetType, DomainModel.NRulesetType>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NScopeFor, DomainModel.NScopeFor>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NUserDesignation, DomainModel.NUserDesignation>()
                .ReverseMap();
            Mapper.CreateMap<EntityModel.NUserOrganisation, DomainModel.NUserOrganisation>()
                .ReverseMap();

            #endregion

            #region OrgChart
            Mapper.CreateMap<EntityModel.Presentation.PresentationOrgChart, DomainModel.Presentation.PresentationOrgChart>()
               .ReverseMap();
            #endregion

            #region Reporing Schedule
            Mapper.CreateMap<EntityModel.Report, Report>()
          .ReverseMap();

            Mapper.CreateMap<EntityModel.ReportAttributes, ReportAttributes>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.ReportHistory, ReportHistory>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationReportProcess, PresentationReportProcess>()
            .ReverseMap();


            Mapper.CreateMap<EntityModel.CustomReportParameter, CustomReportParameter>()
          .ReverseMap();

            Mapper.CreateMap<EntityModel.CustomReport, CustomReport>()
          .ReverseMap();

            Mapper.CreateMap<EntityModel.ServiceAccountQuestion, ServiceAccountQuestion>()
            .ReverseMap();

            #endregion 

            #region Bulk Request
            Mapper.CreateMap<EntityModel.BulkRequest, BulkRequest>()
          .ReverseMap();

            Mapper.CreateMap<EntityModel.BulkRequestDetail, BulkRequestDetail>()
           .ReverseMap();

            #endregion

            #region Undo Request
            Mapper.CreateMap<EntityModel.EventUndoRequest, EventUndoRequest>()
          .ReverseMap();

            #endregion

            #region XL Upload
            Mapper.CreateMap<EntityModel.ADSXLProvisioningMatrix, ADSXLProvisioningMatrix>()
          .ReverseMap();

            Mapper.CreateMap<EntityModel.SMSXLProvisioningMatrix, SMSXLProvisioningMatrix>().ReverseMap();

            Mapper.CreateMap<EntityModel.SMSXLRoleCatalogue, SMSXLRoleCatalogue>().ReverseMap();

            #endregion

            #region GEMS
            Mapper.CreateMap<EntityModel.GEMSRoleInfo, GEMSRoleInfo>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.GEMSRoleMatrix, GEMSRoleMatrix>()
          .ReverseMap();
            Mapper.CreateMap<EntityModel.GEMSUserInfo, GEMSUserInfo>()
          .ReverseMap();
            Mapper.CreateMap<EntityModel.UserBusinessRoleMapping, UserBusinessRoleMapping>()
        .ReverseMap();
            #endregion

            #region NPAC
            Mapper.CreateMap<EntityModel.NPACDesignationInfo, NPACDesignationInfo>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.NPACLevelInfo, NPACLevelInfo>()
          .ReverseMap();
            Mapper.CreateMap<EntityModel.NPACSectionInfo, NPACSectionInfo>()
          .ReverseMap();
            Mapper.CreateMap<EntityModel.NPACUserInfo, NPACUserInfo>()
        .ReverseMap();
            #endregion

            #region SSIDS
            Mapper.CreateMap<EntityModel.SSIDSRolelnfo, SSIDSRolelnfo>()
            .ReverseMap();
            Mapper.CreateMap<EntityModel.SSIDSUserInfo, SSIDSUserInfo>()
          .ReverseMap();
            #endregion

            #region SAP PI
            Mapper.CreateMap<EntityModel.SAPPIRole, DomainModel.SAPPIRole>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPGRCRole, DomainModel.SAPGRCRole>().ReverseMap();
            #endregion

            #region SAP GRC
            Mapper.CreateMap<EntityModel.SAPGRCRole, DomainModel.SAPGRCRole>().ReverseMap();
            #endregion

            #region SAP MSSL
            Mapper.CreateMap<EntityModel.SAPXLMSSLCriticalTCodeMatrix, DomainModel.SAPXLMSSLCriticalTCodeMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPXLMSSLPositionsMatrix, DomainModel.SAPXLMSSLPositionsMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPXLMSSLRolesMatrix, DomainModel.SAPXLMSSLRolesMatrix>().ReverseMap();
            #endregion

            #region SAP EBS
            Mapper.CreateMap<EntityModel.SAPEBSXLCriticalTCodesMatrix, DomainModel.SAPEBSXLCriticalTCodesMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPEBSXLPositionsMatrix, DomainModel.SAPEBSXLPositionsMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPEBSXLRoleMatrix, DomainModel.SAPEBSXLRoleMatrix>().ReverseMap();
            #endregion

            #region O365
            Mapper.CreateMap<EntityModel.Office365LicenseConfig, DomainModel.Office365LicenseConfig>().ReverseMap();
            Mapper.CreateMap<EntityModel.Office365GroupInfo, DomainModel.Office365GroupInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.Office365License, DomainModel.Office365License>().ReverseMap();
            Mapper.CreateMap<EntityModel.Office365MassLicense, DomainModel.Office365MassLicense>().ReverseMap();
            Mapper.CreateMap<EntityModel.Office365RetentionPolicy, DomainModel.Office365RetentionPolicy>().ReverseMap();
            Mapper.CreateMap<EntityModel.Office365RoleAssignmentPolicy, DomainModel.Office365RoleAssignmentPolicy>().ReverseMap();
            Mapper.CreateMap<EntityModel.Office365ServicePlan, DomainModel.Office365ServicePlan>().ReverseMap();
            Mapper.CreateMap<EntityModel.Office365SharingPolicy, DomainModel.Office365SharingPolicy>().ReverseMap();
            Mapper.CreateMap<EntityModel.Office365UserInfo, DomainModel.Office365UserInfo>().ReverseMap();
            #endregion

            #region NOVA
            Mapper.CreateMap<EntityModel.NOVAUserRoleMatrix, DomainModel.NOVAUserRoleMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.NOVAManagementRoleMatrix, DomainModel.NOVAManagementRoleMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.NOVAExceptionalRoleMatrix, DomainModel.NOVAExceptionalRoleMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.NOVASoDExceptionsMatrix, DomainModel.NOVASoDExceptionsMatrix>().ReverseMap();

            Mapper.CreateMap<EntityModel.SAPSectionMappingInfo, DomainModel.SAPSectionMappingInfo>().ReverseMap();

            Mapper.CreateMap<EntityModel.SAPEamNovaUserInfo, DomainModel.SAPEamNovaUserInfo>().ReverseMap();

            Mapper.CreateMap<EntityModel.SAPEamNovaUserRoleInfo, DomainModel.SAPEamNovaUserRoleInfo>().ReverseMap();

            Mapper.CreateMap<EntityModel.NOVACompositeRoleMatrix, DomainModel.NOVACompositeRoleMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.AccountEntitlementRecon, DomainModel.AccountEntitlementRecon>().ReverseMap();
            #endregion

            #region MISC
            Mapper.CreateMap<EntityModel.ApproverExceptionInfo, DomainModel.ApproverExceptionInfo>().ReverseMap();
            #endregion

            Mapper.CreateMap<EntityModel.UserTableInfo, DomainModel.UserTableInfo>().ReverseMap();
            #region VPN
            Mapper.CreateMap<EntityModel.VPNUserInfo, DomainModel.VPNUserInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.VPNMatrix, DomainModel.VPNMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.VPNUserApplicationEnviornment, DomainModel.VPNUserApplicationEnviornment>().ReverseMap();
            Mapper.CreateMap<EntityModel.VPNApplicationEnvironment, DomainModel.VPNApplicationEnvironment>().ReverseMap();
            #endregion
            #region SkypeForBusiness
            Mapper.CreateMap<EntityModel.SkypeForBusinessUserInfo, DomainModel.SkypeForBusinessUserInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SkypeForBusinessResponseGroupInfo, DomainModel.SkypeForBusinessResponseGroupInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SkypeForBusinessUnAssignedNumber, DomainModel.SkypeForBusinessUnAssignedNumber>().ReverseMap();
            Mapper.CreateMap<EntityModel.SkypeForBusinessProvisioningMatrix, DomainModel.SkypeForBusinessProvisioningMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SkypeForBusinessUnAssignedNumberMatrix, DomainModel.SkypeForBusinessUnAssignedNumberMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.PresentationResponseGroupMember, DomainModel.PresentationResponseGroupMember>().ReverseMap();
            Mapper.CreateMap<EntityModel.PresentationSkypeUserInfo, DomainModel.PresentationSkypeUserInfo>().ReverseMap();
            #endregion

            #region SMS
            Mapper.CreateMap<EntityModel.SMSUserInfo, DomainModel.SMSUserInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SMSCompany, DomainModel.SMSCompany>().ReverseMap();
            Mapper.CreateMap<EntityModel.SMSBadgeInfo, DomainModel.SMSBadgeInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SMSAccessLevel, DomainModel.SMSAccessLevel>().ReverseMap();
            #endregion

            #region SRM
            Mapper.CreateMap<EntityModel.SAPSRMMainRoleMatrix, SAPSRMMainRoleMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPSRMEnablerRoleMatrix, SAPSRMEnablerRoleMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPSRMBusinessRoleMapping, SAPSRMBusinessRoleMapping>().ReverseMap();
            #endregion

            Mapper.CreateMap<EntityModel.ApproverDelegationHistory, DomainModel.ApproverDelegationHistory>()
      .ReverseMap();


            Mapper.CreateMap<EntityModel.MailNotificationSettings, DomainModel.MailNotificationSettings>()
         .ReverseMap();


            Mapper.CreateMap<EntityModel.MailNotificationSettings, DomainModel.MailNotificationSettings>()
         .ReverseMap();

            Mapper.CreateMap<EntityModel.ServiceAccountQuestion, DomainModel.ServiceAccountQuestion>()
.ReverseMap();

            Mapper.CreateMap<EntityModel.SRMHangingRoleInfo, SRMHangingRoleInfo>()
            .ReverseMap();

            Mapper.CreateMap<EntityModel.EAMNovaHangingRoleInfo, EAMNovaHangingRoleInfo>()
           .ReverseMap();

            Mapper.CreateMap<EntityModel.PUBFile, PUBFile>().ReverseMap();

            Mapper.CreateMap<EntityModel.PUBEmployees, PUBEmployees>().ReverseMap();

            Mapper.CreateMap<EntityModel.TaskReminder, TaskReminder>().ReverseMap();

            Mapper.CreateMap<EntityModel.PresentationSystemDecommission, PresentationSystemDecommission>().ReverseMap();

            Mapper.CreateMap<EntityModel.AccountEntitlementAuditTrial, DomainModel.AccountEntitlementAuditTrial>().ReverseMap();
            Mapper.CreateMap<EntityModel.PresentationAccountEntitlementAuditTrial, DomainModel.PresentationAccountEntitlementAuditTrial>().ReverseMap();

            Mapper.CreateMap<EntityModel.ViewUserEntitlementData, ViewUserEntitlementData>().ReverseMap();
            Mapper.CreateMap<EntityModel.KeyValueData, KeyValueData>().ReverseMap();

            Mapper.CreateMap<EntityModel.AttestationTaskEntitlementUserComparision, AttestationTaskEntitlementUserComparision>().ReverseMap();
            Mapper.CreateMap<EntityModel.AttestationCampaignEntitlementUserComparision, AttestationCampaignEntitlementUserComparision>().ReverseMap();

            #region SAPHANA
            Mapper.CreateMap<EntityModel.SAPHANAAuthorizationProfile, SAPHANAAuthorizationProfile>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAAuthorizationToObject, SAPHANAAuthorizationToObject>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANACompositeProfileInfo, SAPHANACompositeProfileInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANACompositeRoleInfo, SAPHANACompositeRoleInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAConflictingRoleReport, SAPHANAConflictingRoleReport>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANACriticalTcodeReport, SAPHANACriticalTcodeReport>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAFieldValueMapping, SAPHANAFieldValueMapping>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAGroupHistoryInfo, SAPHANAGroupHistoryInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAGroupInfo, SAPHANAGroupInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAHRP1001, SAPHANAHRP1001>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAObjectInfo, SAPHANAObjectInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAParameterHistoryInfo, SAPHANAParameterHistoryInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAParameterInfo, SAPHANAParameterInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAPosition, SAPHANAPosition>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAProfileInfo, SAPHANAProfileInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANARoleInfo, SAPHANARoleInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANARoleProfileMapping, SAPHANARoleProfileMapping>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANARoleTCodeMapping, SAPHANARoleTCodeMapping>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANARoleToAuthMap, SAPHANARoleToAuthMap>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANARoleToTCodes, SAPHANARoleToTCodes>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANASectionMappingInfo, SAPHANASectionMappingInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANASingleRoleInfo, SAPHANASingleRoleInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANATCodeInfo, SAPHANATCodeInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANATCodeObjectMap, SAPHANATCodeObjectMap>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAUserHistoryInfo, SAPHANAUserHistoryInfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAUserinfo, SAPHANAUserinfo>().ReverseMap();
            Mapper.CreateMap<EntityModel.SapHANAUserProfile, SapHANAUserProfile>().ReverseMap();

            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPHANARoleToAuthorizationReport, PresentationSAPHANARoleToAuthorizationReport>().ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPHANATCodeReport, PresentationSAPHANATCodeReport>().ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPHANAUserDetails, PresentationSAPHANAUserDetails>().ReverseMap();
            Mapper.CreateMap<EntityModel.Presentation.PresentationSAPHANAUserinfo, PresentationSAPHANAUserinfo>().ReverseMap();

            Mapper.CreateMap<EntityModel.SAPHANAXLCriticalTCodeMatrix, SAPHANAXLCriticalTCodeMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAXLPositionsMatrix, SAPHANAXLPositionsMatrix>().ReverseMap();
            Mapper.CreateMap<EntityModel.SAPHANAXLRolesMatrix, SAPHANAXLRolesMatrix>().ReverseMap();
            #endregion

        }
    }
}
