using AutoMapper;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.Dto;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainApi
{
    public class AutoMapperHelper
    {
        public static void BootStrap()
        {

            #region Custom Report

            Mapper.CreateMap<DomainModel.CustomReportParameter, Dto.CustomReportParameter>()
         .ReverseMap();

            Mapper.CreateMap<DomainModel.CustomReport, Dto.CustomReport>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationCustomReportData, Dto.PresentationCustomReportData>().ReverseMap();

            #endregion

            #region "ACL"
            Mapper.CreateMap<DomainModel.Presentation.PresentationReportingEmployees, Dto.Presentation.PresentationReportingEmployees>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationPagePermission, Dto.Presentation.PresentationPagePermission>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationTabMenu, Dto.Presentation.PresentationTabMenu>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.PagePermission, Dto.PagePermission>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.PageUrl, Dto.PageUrl>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Permission, Dto.Permission>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.TabMenu, Dto.TabMenu>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.WebPage, Dto.WebPage>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.UserPreference, Dto.UserPreference>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.AdminModeItem, Dto.AdminModeItem>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.AccessRights, AccessRights>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.PresentationAccessRights, PresentationAccessRights>()
               .ReverseMap();
            #endregion

            #region "Admin"
            Mapper.CreateMap<DomainModel.Role, Dto.Role>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.User, Dto.User>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.UserExclusion, Dto.UserExclusion>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.UserAttachment, Dto.UserAttachment>()
               .ReverseMap();


            Mapper.CreateMap<DomainModel.Presentation.PresentationUser, Dto.Presentation.PresentationUser>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationUserExclusion, Dto.Presentation.PresentationUserExclusion>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationAnonymousUser, Dto.Presentation.PresentationAnonymousUser>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.QuickLink, Dto.QuickLink>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.GlobalQuestion, Dto.GlobalQuestion>().ReverseMap();

            Mapper.CreateMap<DomainModel.ComplianceType, Dto.ComplianceType>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.SODMitigation, Dto.SODMitigation>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.GlobalAnswer, Dto.GlobalAnswer>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.MailServerConfiguration, Dto.MailServerConfiguration>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.MailTemplate, Dto.MailTemplate>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationMail, Dto.Presentation.PresentationMail>().ReverseMap();


            Mapper.CreateMap<DomainModel.PurgeConfig, Dto.PurgeConfig>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.UserImage, Dto.UserImage>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.ComplianceProcessHistory, Dto.ComplianceProcessHistory>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.SODExceptionItem, Dto.SODExceptionItem>().ReverseMap();

            #endregion

            #region AdhocAccessRequest
            Mapper.CreateMap<DomainModel.AdHocAccessRequest, Dto.AdHocAccessRequest>()
         .ReverseMap();

            Mapper.CreateMap<DomainModel.AccessRequestConfiguration, Dto.AccessRequestConfiguration>()
         .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAdHocAccessRequest, Dto.Presentation.PresentationAdHocAccessRequest>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationMyRequestData, Dto.Presentation.PresentationMyRequestData>().ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAdHocRequest, Dto.Presentation.PresentationAdHocRequest>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationInterimAdhocRequest, Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest>()
                 .ReverseMap();
            #endregion
            Mapper.CreateMap(typeof(PresentationGridPagedData<>), typeof(Dto.Core.PresentationGridPagedData<>)).ReverseMap();
            Mapper.CreateMap(typeof(PresentationGridPagedDataParameter), typeof(Dto.Core.PresentationGridPagedDataParameter)).ReverseMap();
            #region BulkRequest
            Mapper.CreateMap<DomainModel.BulkRequest, Dto.BulkRequest>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.BulkRequestDetail, Dto.BulkRequestDetail>()
                .ReverseMap();
            #endregion BulkRequest

            #region Contract

            Mapper.CreateMap<DomainModel.IConfigurationMaster, Dto.IConfigurationMaster>()
              .ConvertUsing<ConfigurationMasterDtoConverter>();

            Mapper.CreateMap<Dto.IConfigurationMaster, DomainModel.IConfigurationMaster>()
                .ConvertUsing<ConfigurationMasterDomainConverter>();

            Mapper.CreateMap<DomainModel.IEndpointEntitlementInfo, Dto.IEndpointEntitlementInfo>()
                .ConvertUsing<EndpointEntitlementInfoDtoConverter>();

            Mapper.CreateMap<Dto.IEndpointEntitlementInfo, DomainModel.IEndpointEntitlementInfo>()
                .ConvertUsing<EndpointEntitlementInfoDomainConverter>();

            Mapper.CreateMap<DomainModel.IEndpointUserInfo, Dto.IEndpointUserInfo>()
                .ConvertUsing<EndpointUserInfoDtoConverter>();

            Mapper.CreateMap<Dto.IEndpointUserInfo, DomainModel.IEndpointUserInfo>()
                .ConvertUsing<EndpointUserInfoDomainConverter>();


            Mapper.CreateMap<DomainModel.IAccessRequestItem, Dto.IAccessRequestItem>()
               .ConvertUsing<PresentationAccessRequestItemDtoConverter>();

            Mapper.CreateMap<Dto.IAccessRequestItem, DomainModel.IAccessRequestItem>()
                .ConvertUsing<PresentationAccessRequestItemDomainConverter>();


            Mapper.CreateMap<DomainModel.IXLData, Dto.IXLData>()
               .ConvertUsing<XLDataConverter>();

            Mapper.CreateMap<Dto.IXLData, DomainModel.IXLData>()
                .ConvertUsing<XLDataConverterDomainConverter>();

            Mapper.CreateMap<DomainModel.Presentation.PresentationUserEntitlement, Dto.Presentation.PresentationUserEntitlement>()
                .ConvertUsing<UserEntitlementDtoConverter>();

            Mapper.CreateMap<Dto.Presentation.PresentationUserEntitlement, DomainModel.Presentation.PresentationUserEntitlement>()
                .ConvertUsing<UserEntitlementDomainConverter>();

            #endregion

            #region "Compliance"

            Mapper.CreateMap<DomainModel.SODRiskLevel, Dto.SODRiskLevel>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODRiskScore, Dto.SODRiskScore>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODRiskCode, Dto.SODRiskCode>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODMitigation, Dto.SODMitigation>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.CompliancePolicy, Dto.CompliancePolicy>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODConflict, Dto.SODConflict>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODConflictFunctions, Dto.SODConflictFunctions>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.SODEndPointPolicySelection, Dto.SODEndPointPolicySelection>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODException, Dto.SODException>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODExceptionItem, Dto.SODExceptionItem>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODFunction, Dto.SODFunction>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODFunctionItem, Dto.SODFunctionItem>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.SODPolicyExecutionDefinition, Dto.SODPolicyExecutionDefinition>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.UserSODComplianceReport, Dto.UserSODComplianceReport>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.UserQuickLink, Dto.UserQuickLink>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.DefaultRoleQuickLink, Dto.DefaultRoleQuickLink>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.UserGridFilterInfo, Dto.UserGridFilterInfo>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.UserGridInfo, Dto.UserGridInfo>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.UserGridAllInfo, Dto.Presentation.UserGridAllInfo>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.UserGridColumnInfo, Dto.UserGridColumnInfo>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationComplianceProcess, Dto.Presentation.PresentationComplianceProcess>()
            .ReverseMap();


            #endregion

            #region Common
            Mapper.CreateMap<DomainModel.Group, Dto.Group>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.UserAccessDetailMatrixGridData, Dto.Presentation.UserAccessDetailMatrixGridData>().ReverseMap();

            Mapper.CreateMap<DomainModel.GroupHistory, Dto.GroupHistory>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.TableProperties, Dto.TableProperties>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.AdhocRequestAccountTyebyMonth, Dto.AdhocRequestAccountTyebyMonth>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.AccessRequestSourcePieSeries, Dto.AccessRequestSourcePieSeries>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.AccessRequestSourceChartData, Dto.AccessRequestSourceChartData>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationcompletedSourcePieSeries, Dto.AttestationcompletedSourcePieSeries>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.AccessRequestSourceByMonth, Dto.AccessRequestSourceByMonth>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.AccessRequestProgressStatusByMonth, Dto.AccessRequestProgressStatusByMonth>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Attention, Dto.Attention>()
                .ReverseMap();
            #endregion

            #region Configuration
            Mapper.CreateMap<DomainModel.ConfigurationMaster, Dto.ConfigurationMaster>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.ConfigurationMasterPIM, ConfigurationMasterPIM>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.ConfigurationMasterPasswordManagement, ConfigurationMasterPasswordManagement>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.ConfigurationMasterIM, ConfigurationMasterIM>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.ConfigurationMasterDGM, ConfigurationMasterDGM>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.ConfigurationMasterCompliance, ConfigurationMasterCompliance>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.ConfigurationMasterHistory, Dto.ConfigurationMasterHistory>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.PasswordSyncConfigurationDetail, Dto.PasswordSyncConfigurationDetail>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.ConfigGeneralTrustedSource, Dto.ConfigGeneralTrustedSource>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.PasswordPolicy, Dto.PasswordPolicy>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.PasswordPolicyEndpoint, Dto.PasswordPolicyEndpoint>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.PasswordSyncConfiguration, Dto.PasswordSyncConfiguration>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.RequestApprovalConfiguration, Dto.RequestApprovalConfiguration>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.AuditConfiguration, Dto.AuditConfiguration>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.AuditConfigurationDetail, Dto.AuditConfigurationDetail>()
              .ReverseMap();
            #endregion

            #region Database

            Mapper.CreateMap<DomainModel.MySqlConfiguration, Dto.MySqlConfiguration>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.SqlConfiguration, Dto.SqlConfiguration>()
            .ReverseMap();

            #endregion

            #region EndpointConfiguration
            Mapper.CreateMap<DomainModel.EndpointConfiguration, EndpointConfiguration>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.EndpointConfigurationDetail, EndpointConfigurationDetail>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.EndpointConfigurationKey, EndpointConfigurationKey>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.EndpointKeyConfiguration, EndpointKeyConfiguration>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.EndpointUserViewConfiguration, EndpointUserViewConfiguration>()
               .ReverseMap();
            #endregion

            #region Discovery

            Mapper.CreateMap<DomainModel.DiscoveryProcess, Dto.DiscoveryProcess>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.DiscoveryProcessHistory, Dto.DiscoveryProcessHistory>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationDiscoveryProcess, Dto.Presentation.PresentationDiscoveryProcess>()
            .ReverseMap();

            #endregion

            #region Email

            Mapper.CreateMap<DomainModel.EmailInfo, Dto.EmailInfo>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.EmailInfoHistory, Dto.EmailInfoHistory>()
                .ReverseMap();

            #endregion

            #region Reconciliation

            Mapper.CreateMap<DomainModel.ExceptionAttribute, Dto.ExceptionAttribute>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.ExceptionAttributeChange, Dto.ExceptionAttributeChange>()
                .ReverseMap();

            #endregion

            #region MetaData

            Mapper.CreateMap<DomainModel.MetaData, Dto.MetaData>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.AccountCategory, Dto.AccountCategory>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.AccountActionCategory, Dto.AccountActionCategory>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.AccessRequestCategory, Dto.AccessRequestCategory>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.AccessRequestProgressTBD, Dto.AccessRequestProgressTBD>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.AccessRequestSourceTBD, Dto.AccessRequestSourceTBD>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.AccessRequestStatusTBD, Dto.AccessRequestStatusTBD>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.ApprovalUserTypes, Dto.ApprovalUserTypes>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.UserPreference, Dto.UserPreference>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.UserCulturePreference, Dto.UserCulturePreference>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.RoleTypes, Dto.RoleTypes>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.RequestTypes, Dto.RequestTypes>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.NDesignation, Dto.NDesignation>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.NCostCenter, Dto.NCostCenter>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.EmployeeTerm, Dto.EmployeeTerm>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.SubTask, Dto.SubTask>()
            .ReverseMap();


            Mapper.CreateMap<DomainModel.OffboardingAccountAction, Dto.OffboardingAccountAction>()
            .ReverseMap();

            #endregion

            #region Tasks
            Mapper.CreateMap<DomainModel.InterimStore, Dto.InterimStore>()
                    .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationInterimStore, Dto.Presentation.PresentationInterimStore>()
                    .ReverseMap();
            Mapper.CreateMap<DomainModel.RequestAttachment, Dto.RequestAttachment>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.TaskDelegation, Dto.TaskDelegation>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.TaskDelegation, Dto.TaskDelegationWrapper>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Task, Dto.Task>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationTaskdelegation, Dto.Presentation.PresentationTaskdelegation>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationTaskDelegationReport, Dto.Presentation.PresentationTaskDelegationReport>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAttestationTaskSummary, Dto.Presentation.PresentationAttestationTaskSummary>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationTask, Dto.Presentation.PresentationTask>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationPreviousApprovers, Dto.Presentation.PresentationPreviousApprovers>()
             .ReverseMap();
            #endregion

            #region Notifications
            Mapper.CreateMap<DomainModel.Notification, Dto.Notification>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.NotificationAttachment, Dto.NotificationAttachment>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationNotification, Dto.Presentation.PresentationNotification>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationRequestActionUsers, Dto.Presentation.PresentationRequestActionUsers>()
         .ReverseMap();

            #endregion

            #region Attachments
            Mapper.CreateMap<DomainModel.Attachment, Dto.Attachment>()
             .ReverseMap();
            #endregion

            #region Provisioning

            Mapper.CreateMap<DomainModel.Presentation.PresentationAccessRequest, Dto.Presentation.PresentationAccessRequest>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationOrganisation, Dto.Presentation.PresentationOrganisation>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.EntitlementReconcilation, Dto.EntitlementReconcilation>()
        .ReverseMap();

            Mapper.CreateMap<DomainModel.AccountReconcilation, Dto.AccountReconcilation>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.AccountEntitlementRecon, Dto.AccountEntitlementRecon>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationAccountReconcilationReport, Dto.Presentation.PresentationAccountReconcilationReport>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationResolvedAccountReconcilationReport, Dto.Presentation.PresentationResolvedAccountReconcilationReport>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.EmployeeEventTrigger, Dto.EmployeeEventTrigger>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.DeptChangedUserGridData, Dto.DeptChangedUserGridData>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationEmployeeEventTrigger, Dto.Presentation.PresentationEmployeeEventTrigger>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationEntitlementReconcilation, Dto.Presentation.PresentationEntitlementReconcilation>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationEntitlementReconcilationReport, Dto.Presentation.PresentationEntitlementReconcilationReport>()
           .ReverseMap();


            Mapper.CreateMap<DomainModel.OUChangeSnapshot, Dto.OUChangeSnapshot>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationOUChangeSnapshots, Dto.Presentation.PresentationOUChangeSnapshots>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationAccessSnapshots, Dto.Presentation.PresentationAccessSnapshots>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.AccessRequest, Dto.AccessRequest>()
                .ReverseMap();


            Mapper.CreateMap<DomainModel.Presentation.PresentationApprovers, Dto.Presentation.PresentationApprovers>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.FailedRequestGridData, Dto.FailedRequestGridData>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.MyRequestsGridData, Dto.MyRequestsGridData>().ReverseMap();
            #endregion

            #region Complaince
            Mapper.CreateMap<DomainModel.ComplianceProcess, Dto.ComplianceProcess>()
                .ReverseMap();
            #endregion

            #region Scheduler
            Mapper.CreateMap<DomainModel.Scheduler, Dto.Scheduler>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Process, Dto.Process>()
       .ReverseMap();
            Mapper.CreateMap<DomainModel.ProcessHistory, Dto.ProcessHistory>()
       .ReverseMap();

            #endregion

            #region Dashboard
            Mapper.CreateMap<DomainModel.Dashboard, Dto.Dashboard>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.DashboardAccessRights, Dto.DashboardAccessRights>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.DefaultRoleDashboardPreference, Dto.DefaultRoleDashboardPreference>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.UserDashboardPreference, Dto.UserDashboardPreference>()
            .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationDashBoardPreference, Dto.Presentation.PresentationDashBoardPreference>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationQuickLinkAccessRights, Dto.Presentation.PresentationQuickLinkAccessRights>()
          .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationDefaultRoleQuickLink, Dto.Presentation.PresentationDefaultRoleQuickLink>()
          .ReverseMap();
            #endregion

            #region  EndPoint

            Mapper.CreateMap<DomainModel.EndPoint, Dto.EndPoint>().ReverseMap();

            Mapper.CreateMap<DomainModel.EndPointSystemType, Dto.EndPointSystemType>().ReverseMap();
            Mapper.CreateMap<DomainModel.EndpointObjects, Dto.EndpointObjects>().ReverseMap();
            Mapper.CreateMap<DomainModel.AttributeMappingColumns, Dto.AttributeMappingColumns>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPActivityInfo, Dto.SAPActivityInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPAuthorizationProfile, Dto.SAPAuthorizationProfile>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPAuthorizationToObject, Dto.SAPAuthorizationToObject>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPCompositeProfileInfo, Dto.SAPCompositeProfileInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.EndPointAutoExpiry, Dto.EndPointAutoExpiry>().ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationEndPoint, Dto.Presentation.PresentationEndPoint>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationUserEntitlement, Dto.Presentation.PresentationUserEntitlement>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationExceptionReconData, Dto.Presentation.PresentationExceptionReconData>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAccountDetail, Dto.Presentation.PresentationAccountDetail>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAccessRequestFilterCriteria, Dto.Presentation.PresentationAccessRequestFilterCriteria>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAccessRequestItem, Dto.Presentation.PresentationAccessRequestItem>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationOrgChangeTrack, Dto.Presentation.PresentationOrgChangeTrack>().ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationUserAccount, Dto.Presentation.PresentationUserAccount>().ReverseMap();
            Mapper.CreateMap<Dto.IEndpointUserInfo, DomainModel.IEndpointUserInfo>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationEndPointForFilter, Dto.Presentation.PresentationEndPointForFilter>().ReverseMap();
            #endregion


            Mapper.CreateMap<DomainModel.PresentationADSUserEntitlement, Dto.PresentationADSUserEntitlement>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationInhouseUserEntitlement, Dto.PresentationInhouseUserEntitlement>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationSAPEPUserEntitlement, Dto.PresentationSAPEPUserEntitlement>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationSAPUserEntitlement, Dto.PresentationSAPUserEntitlement>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationEndpointProvisioningField, Dto.Presentation.PresentationEndpointProvisioningField>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationIACMUserEntitlement, Dto.PresentationIACMUserEntitlement>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.PresentationOffice365UserEntitlement, Dto.PresentationOffice365UserEntitlement>()
              .ReverseMap();
            #region "IACM"
            Mapper.CreateMap<DomainModel.IACMUserInfo, Dto.IACMUserInfo>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.IACMRoleInfo, Dto.IACMRoleInfo>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.IACMGroupInfo, Dto.IACMGroupInfo>()
               .ReverseMap();
            #endregion

            #region "ActiveDirectory"

            Mapper.CreateMap<DomainModel.ADSUserInfo, Dto.ADSUserInfo>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.ADSGroupInfo, Dto.ADSGroupInfo>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.ADSComputerGroup, Dto.ADSComputerGroup>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.ADSComputerInfo, Dto.ADSComputerInfo>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.ADSGroupInfoHistory, Dto.ADSGroupInfoHistory>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.ADSOuInfo, Dto.ADSOuInfo>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.ADSOuInfoHistory, Dto.ADSOuInfoHistory>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.ADSPrinterInfo, Dto.ADSPrinterInfo>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.ADSSharedFolderInfo, Dto.ADSSharedFolderInfo>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.ADSUserInfoHistory, Dto.ADSUserInfoHistory>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.ADSNestedGroup, Dto.ADSNestedGroup>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationADSComputerInfo, Dto.Presentation.PresentationADSComputerInfo>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationADSPrinterInfo, Dto.Presentation.PresentationADSPrinterInfo>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationADSOUs, Dto.Presentation.PresentationADSOUs>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationADSUsers, Dto.Presentation.PresentationADSUsers>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationADSUserToGroups, Dto.Presentation.PresentationADSUserToGroups>()
               .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationADSDormantAccount, Dto.Presentation.PresentationADSDormantAccount>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationADSLockedDisabledAccount, Dto.Presentation.PresentationADSLockedDisabledAccount>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationADSAccountWithNoGroups, Dto.Presentation.PresentationADSAccountWithNoGroups>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationADSGroupWithNoAccounts, Dto.Presentation.PresentationADSGroupWithNoAccounts>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationADSActiveAccountsWithInActiveUser, Dto.Presentation.PresentationADSActiveAccountsWithInActiveUser>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationADSFuturePasswordExpiryAccount, Dto.Presentation.PresentationADSFuturePasswordExpiryAccount>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationADSFutureAccountExpiryAccount, Dto.Presentation.PresentationADSFutureAccountExpiryAccount>()
                .ReverseMap();

            #endregion

            #region "SAP"

            Mapper.CreateMap<DomainModel.SAPUserinfo, Dto.SAPUserinfo>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.SAPActivityInfo, Dto.SAPActivityInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPAuthorizationProfile, Dto.SAPAuthorizationProfile>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPAuthorizationToObject, Dto.SAPAuthorizationToObject>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPCompositeProfileInfo, Dto.SAPCompositeProfileInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPSingleRoleInfo, Dto.SAPSingleRoleInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPTCodeInfo, Dto.SAPTCodeInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPObjectInfo, Dto.SAPObjectInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPObjectValues, Dto.SAPObjectValues>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPParameterHistoryInfo, Dto.SAPParameterHistoryInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPParameterInfo, Dto.SAPParameterInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPPosition, Dto.SAPPosition>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPProfileInfo, Dto.SAPProfileInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHRP1001, Dto.SAPHRP1001>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPRoleInfo, Dto.SAPRoleInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SapRoleProfileMapping, Dto.SapRoleProfileMapping>().ReverseMap();
            Mapper.CreateMap<DomainModel.SapRoleTCodeMapping, Dto.SapRoleTCodeMapping>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPRoleToTCodes, Dto.SAPRoleToTCodes>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPRoleToAuthMap, Dto.SAPRoleToAuthMap>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPTCodeObjectMap, Dto.SAPTCodeObjectMap>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPUserHistoryInfo, Dto.SAPUserHistoryInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SapUserProfile, Dto.SapUserProfile>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPUserToObject, Dto.SAPUserToObject>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPEPGroupInfoHistory, Dto.SAPEPGroupInfoHistory>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPEPRoleInfoHistory, Dto.SAPEPRoleInfoHistory>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPEPRoleInfoTemp, Dto.SAPEPRoleInfoTemp>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPEPUserInfoHistory, Dto.SAPEPUserInfoHistory>().ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPUserinfo, Dto.Presentation.PresentationSAPUserinfo>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPTCodeReport, Dto.Presentation.PresentationSAPTCodeReport>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPProfileReport, Dto.Presentation.PresentationSAPProfileReport>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPRoleReport, Dto.Presentation.PresentationSAPRoleReport>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPRoleToAuthorizationReport, Dto.Presentation.PresentationSAPRoleToAuthorizationReport>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPUser, Dto.Presentation.PresentationSAPUser>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPUserRoleTCodeAuth, Dto.Presentation.PresentationSAPUserRoleTCodeAuth>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPUsersToGroups, Dto.Presentation.PresentationSAPUsersToGroups>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPUsersToProfileReport, Dto.Presentation.PresentationSAPUsersToProfileReport>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPUsersToRolesReport, Dto.Presentation.PresentationSAPUsersToRolesReport>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.SAPCompositeRoleInfo, Dto.SAPCompositeRoleInfo>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPUserWithNoRoles, Dto.Presentation.PresentationSAPUserWithNoRoles>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPRoleWithNoUsers, Dto.Presentation.PresentationSAPRoleWithNoUsers>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPDormantUser, Dto.Presentation.PresentationSAPDormantUser>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPUserExpiryAccount, Dto.Presentation.PresentationSAPUserExpiryAccount>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPLockedUser, Dto.Presentation.PresentationSAPLockedUser>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPRoleToUsers, Dto.Presentation.PresentationSAPRoleToUsers>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationConflictRemarks, Dto.Presentation.PresentationConflictRemarks>()
                .ReverseMap();
            #endregion

            #region "SAPEP"

            Mapper.CreateMap<DomainModel.SAPEPUserInfo, Dto.SAPEPUserInfo>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.SAPEPGroupInfo, Dto.SAPEPGroupInfo>()
     .ReverseMap();

            Mapper.CreateMap<DomainModel.SAPEPRoleInfo, Dto.SAPEPRoleInfo>()
     .ReverseMap();

            Mapper.CreateMap<DomainModel.SAPFieldValueMapping, Dto.SAPFieldValueMapping>()
     .ReverseMap();

            Mapper.CreateMap<DomainModel.SAPGroupHistoryInfo, Dto.SAPGroupHistoryInfo>()
     .ReverseMap();
            Mapper.CreateMap<DomainModel.SAPGroupInfo, Dto.SAPGroupInfo>()
     .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPEPRoleToUsersReport, Dto.Presentation.PresentationSAPEPRoleToUsersReport>()
     .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPEPGroupToUsersReport, Dto.Presentation.PresentationSAPEPGroupToUsersReport>()
     .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPEPUserInfo, Dto.Presentation.PresentationSAPEPUserInfo>()
     .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPEPUserToGroups, Dto.Presentation.PresentationSAPEPUserToGroups>()
     .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPEPUserToRoles, Dto.Presentation.PresentationSAPEPUserToRoles>()
     .ReverseMap();
            #endregion

            #region "SAP FLINS"

            Mapper.CreateMap<DomainModel.SAPHRFLINSRoleMatrix, Dto.SAPHRFLINSRoleMatrix>().ReverseMap();

            Mapper.CreateMap<DomainModel.SAPHRFLINSSuppRoleMatrix, Dto.SAPHRFLINSSuppRoleMatrix>().ReverseMap();

            Mapper.CreateMap<DomainModel.ProvisioningConfigMatrixSapEEGroup, Dto.ProvisioningConfigMatrixSapEEGroup>().ReverseMap();
            Mapper.CreateMap<DomainModel.ProvisioningConfigMatrixSAPOrg, Dto.ProvisioningConfigMatrixSAPOrg>().ReverseMap();
            Mapper.CreateMap<DomainModel.ProvisioningConfigMatrixSAPPosition, Dto.ProvisioningConfigMatrixSAPPosition>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPBIRoleCategory, Dto.SAPBIRoleCategory>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHRFLINSConflictingRole, Dto.SAPHRFLINSConflictingRole>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPRoleMandatoryField, Dto.SAPRoleMandatoryField>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPRoleMatrix, Dto.SAPRoleMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPSODConflictingRole, Dto.SAPSODConflictingRole>().ReverseMap();

            #endregion "SAP FLINS"

            #region SAP GRC
            Mapper.CreateMap<DomainModel.SAPGRCRole, Dto.SAPGRCRole>()
             .ReverseMap();

            #endregion

            #region "InHouse Systems"

            Mapper.CreateMap<DomainModel.InhouseUserInfo, Dto.InhouseUserInfo>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.InhousePermission, Dto.InhousePermission>().ReverseMap();
            Mapper.CreateMap<DomainModel.InhouseFunction, Dto.InhouseFunction>().ReverseMap();
            Mapper.CreateMap<DomainModel.InhUserPermissionFunction, Dto.InhUserPermissionFunction>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationInhouseSystemEntitlements, Dto.Presentation.PresentationInhouseSystemEntitlements>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationUserAccessDetail, Dto.Presentation.PresentationUserAccessDetail>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationInhouseSystem, Dto.Presentation.PresentationInhouseSystem>().ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationInhouseUserInfo, Dto.Presentation.PresentationInhouseUserInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationInhouseUserToPermission, Dto.Presentation.PresentationInhouseUserToPermission>()
               .ReverseMap();
            #endregion

            #region Organization

            Mapper.CreateMap<DomainModel.NOrganisation, Dto.NOrganisation>().ReverseMap();

            #endregion

            #region WorkFlow

            Mapper.CreateMap<DomainModel.WorkflowConfiguration, Dto.WorkflowConfiguration>().ReverseMap();
            Mapper.CreateMap<DomainModel.WorkflowConfigurationDetail, Dto.WorkflowConfigurationDetail>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationWorkflowConfiguration, Dto.Presentation.PresentationWorkflowConfiguration>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationWorkflowApproverGridData, Dto.Presentation.PresentationWorkflowApproverGridData>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.ApprovalActionCriteria, Dto.Presentation.ApprovalActionCriteria>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.WithdrawActionCriteria, Dto.Presentation.WithdrawActionCriteria>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.SubmitActionCriteria, Dto.Presentation.SubmitActionCriteria>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationWorkflowCriteriaBase, Dto.Presentation.PresentationWorkflowCriteriaBase>()
                .ConvertUsing<WorkflowCriteriaDtoConverter>();
            Mapper.CreateMap<Dto.Presentation.PresentationWorkflowCriteriaBase, DomainModel.Presentation.PresentationWorkflowCriteriaBase>()
                .ConvertUsing<WorkflowCriteriaDomainConverter>();

            Mapper.CreateMap<DomainModel.Presentation.PresentationApprovalChain, Dto.Presentation.PresentationApprovalChain>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.ApproverDetail, Dto.Presentation.ApproverDetail>().ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationWorkflow, Dto.Presentation.PresentationWorkflow>()
           .ReverseMap();
            #endregion

            #region Logs
            Mapper.CreateMap<DomainModel.UILog, Dto.UILog>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.ServiceLog, Dto.ServiceLog>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.DiscoveryLog, Dto.DiscoveryLog>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.ProvisioningLog, Dto.ProvisioningLog>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationLog, Dto.AttestationLog>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.EmailLog, Dto.EmailLog>()
             .ReverseMap();
            #endregion

            #region SOD
            Mapper.CreateMap<DomainModel.ComplianceReportModules, Dto.ComplianceReportModules>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.SODRunPolicySelection, Dto.SODRunPolicySelection>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.SODRunPolicy, Dto.SODRunPolicy>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.SODReport, Dto.SODReport>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.SODConflictFunctionMap, Dto.SODConflictFunctionMap>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.SODExecutionConfig, Dto.SODExecutionConfig>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.SODPolicy, Dto.SODPolicy>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.SODPolicyReport, Dto.SODPolicyReport>()
               .ReverseMap();
            Mapper.CreateMap<DomainModel.SODPolicyConfiguration, Dto.SODPolicyConfiguration>()
   .ReverseMap();
            #endregion

            #region Preference

            Mapper.CreateMap<DomainModel.QuickLinkAccessRights, Dto.QuickLinkAccessRights>()
           .ReverseMap();

            #endregion

            #region Attestation

            Mapper.CreateMap<DomainModel.Presentation.PresentationReviewer, Dto.Presentation.PresentationReviewer>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationAttestationEndPoint, Dto.Presentation.PresentationAttestationEndPoint>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationAttestation, Dto.Presentation.PresentationAttestation>()
             .ReverseMap();

            Mapper.CreateMap<DomainModel.Attestation, Dto.Attestation>().ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationScope, Dto.AttestationScope>().ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationListAttestation, Dto.Presentation.PresentationListAttestation>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationJobListAttestation, Dto.Presentation.PresentationJobListAttestation>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationProcess, Dto.AttestationProcess>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationEndpoint, Dto.AttestationEndpoint>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationProcessHistory, Dto.AttestationProcessHistory>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAttestationReviewPageSAP_Level1, Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAttestationStatusSummary, Dto.Presentation.PresentationAttestationStatusSummary>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationSapUserRoleTCodeAuthorization, Dto.Presentation.PresentationSapUserRoleTCodeAuthorization>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail, Dto.Presentation.PresentationAttestationEntitlementReviewDetail>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationExportAttestationEntitlementReviewDetail, Dto.Presentation.PresentationExportAttestationEntitlementReviewDetail>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationExportAttestationReportForUI, Dto.Presentation.PresentationExportAttestationReportForUI>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationCampaign, Dto.AttestationCampaign>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationCampaignAttachment, Dto.AttestationCampaignAttachment>()
            .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationCampaignReport, Dto.AttestationCampaignReport>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.Presentation.PresentationAttetstationCampaign, Dto.Presentation.PresentationAttetstationCampaign>()
               .ReverseMap();
            #endregion

            #region GridSettings
            Mapper.CreateMap<DomainModel.UserGridFilterSettings, Dto.UserGridFilterSettings>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.UserGridSettings, Dto.UserGridSettings>()
             .ReverseMap();

            #endregion

            #region Data            
            #endregion

            #region Attestation Revamp
            Mapper.CreateMap<DomainModel.AttestationHierarchy, Dto.AttestationHierarchy>()
       .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationSnapshot, Dto.AttestationSnapshot>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationSnapshotAction, Dto.AttestationSnapshotAction>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationSnapshotActionInterim, Dto.AttestationSnapshotActionInterim>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationSnapshotTaskStatus, Dto.AttestationSnapshotTaskStatus>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.AttestationSnapshotActionHistory, Dto.AttestationSnapshotActionHistory>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.EndpointHierarchy, Dto.EndpointHierarchy>()
        .ReverseMap();
            #endregion

            #region Org Revamp

            Mapper.CreateMap<DomainModel.NDesignation, Dto.NDesignation>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NOrganisation, Dto.NOrganisation>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NOrganisationHead, Dto.NOrganisationHead>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NOrganisationScope, Dto.NOrganisationScope>()
        .ReverseMap();

            Mapper.CreateMap<DomainModel.NRulesetType, Dto.NRulesetType>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NScopeFor, Dto.NScopeFor>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NProvisioningField, Dto.NProvisioningField>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NOrganisationRelationship, Dto.NOrganisationRelationship>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NRuleset, Dto.NRuleset>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NRulesetForApplication, Dto.NRulesetForApplication>()
        .ReverseMap();
            Mapper.CreateMap<DomainModel.NRulesetForApplication, Dto.NRulesetForApplication>()
              .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationNRulesetForApplication, Dto.PresentationNRulesetForApplication>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.NRulesetType, Dto.NRulesetType>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.NScopeFor, Dto.NScopeFor>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.NUserDesignation, Dto.NUserDesignation>()
                .ReverseMap();
            Mapper.CreateMap<DomainModel.NUserOrganisation, Dto.NUserOrganisation>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.NOrganisationPresentation, Dto.NOrganisationPresentation>()
      .ReverseMap();
            #endregion

            #region OrgChart
            Mapper.CreateMap<DomainModel.Presentation.PresentationOrgChart, Dto.Presentation.PresentationOrgChart>()
       .ReverseMap();
            #endregion

            #region Reporting schedule
            Mapper.CreateMap<DomainModel.Report, Dto.Report>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.ReportAttributes, Dto.ReportAttributes>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.ReportHistory, Dto.ReportHistory>()
           .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationReportProcess, Dto.Presentation.PresentationReportProcess>()
           .ReverseMap();

            #endregion


            Mapper.CreateMap<DomainModel.ApproverDelegationHistory, Dto.ApproverDelegationHistory>()
         .ReverseMap();


            Mapper.CreateMap<DomainModel.MailNotificationSettings, Dto.MailNotificationSettings>()
         .ReverseMap();


            Mapper.CreateMap<DomainModel.MailNotificationSettings, Dto.MailNotificationSettings>()
         .ReverseMap();

            Mapper.CreateMap<DomainModel.ServiceAccountQuestion, Dto.ServiceAccountQuestion>()
.ReverseMap();

            #region XLUpload
            Mapper.CreateMap<DomainModel.ADSXLProvisioningMatrix, ADSXLProvisioningMatrix>().ReverseMap();
            #endregion

            #region "GEMS"

            Mapper.CreateMap<DomainModel.GEMSRoleInfo, Dto.GEMSRoleInfo>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.GEMSRoleMatrix, Dto.GEMSRoleMatrix>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.GEMSUserInfo, Dto.GEMSUserInfo>()
          .ReverseMap();
            Mapper.CreateMap<DomainModel.UserBusinessRoleMapping, Dto.UserBusinessRoleMapping>()
            .ReverseMap();

            #endregion

            #region "NPAC"

            Mapper.CreateMap<DomainModel.NPACDesignationInfo, Dto.NPACDesignationInfo>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.NPACLevelInfo, Dto.NPACLevelInfo>()
             .ReverseMap();
            Mapper.CreateMap<DomainModel.NPACSectionInfo, Dto.NPACSectionInfo>()
          .ReverseMap();
            Mapper.CreateMap<DomainModel.NPACUserInfo, Dto.NPACUserInfo>()
          .ReverseMap();
            #endregion

            #region "SSIDS"

            Mapper.CreateMap<DomainModel.SSIDSRolelnfo, Dto.SSIDSRolelnfo>()
              .ReverseMap();
            Mapper.CreateMap<DomainModel.SSIDSUserInfo, Dto.SSIDSUserInfo>()
             .ReverseMap();

            #endregion

            #region SAP Others
            Mapper.CreateMap<DomainModel.SAPPIRole, Dto.SAPPIRole>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPGRCRole, Dto.SAPGRCRole>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPCriticalTcodeReport, Dto.SAPCriticalTcodeReport>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPConflictingRoleReport, Dto.SAPConflictingRoleReport>().ReverseMap();
            #endregion

            #region SAP MSSL
            Mapper.CreateMap<DomainModel.SAPXLMSSLCriticalTCodeMatrix, Dto.SAPXLMSSLCriticalTCodeMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPXLMSSLPositionsMatrix, Dto.SAPXLMSSLPositionsMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPXLMSSLRolesMatrix, Dto.SAPXLMSSLRolesMatrix>().ReverseMap();
            #endregion

            #region SAP EBS
            Mapper.CreateMap<DomainModel.SAPEBSXLCriticalTCodesMatrix, Dto.SAPEBSXLCriticalTCodesMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPEBSXLPositionsMatrix, Dto.SAPEBSXLPositionsMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPEBSXLRoleMatrix, Dto.SAPEBSXLRoleMatrix>().ReverseMap();
            #endregion

            #region SAP SRM
            Mapper.CreateMap<DomainModel.SAPSRMMainRoleMatrix, Dto.SAPSRMMainRoleMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPSRMEnablerRoleMatrix, Dto.SAPSRMEnablerRoleMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPSRMBusinessRoleMapping, Dto.SAPSRMBusinessRoleMapping>().ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationUserBusinessRole, Dto.PresentationUserBusinessRole>().ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationSRMPOrg, Dto.PresentationSRMPOrg>().ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationSRMPGroup, Dto.PresentationSRMPGroup>().ReverseMap();
            #endregion

            #region SSIDS


            Mapper.CreateMap<DomainModel.SSIDSUserInfo, Dto.SSIDSUserInfo>()
         .ReverseMap();


            Mapper.CreateMap<DomainModel.SSIDSRolelnfo, Dto.SSIDSRolelnfo>()
         .ReverseMap();


            #endregion SSIDS

            #region O365
            Mapper.CreateMap<DomainModel.Office365LicenseConfig, Dto.Office365LicenseConfig>().ReverseMap();
            Mapper.CreateMap<DomainModel.Office365GroupInfo, Dto.Office365GroupInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.Office365License, Dto.Office365License>().ReverseMap();
            Mapper.CreateMap<DomainModel.Office365MassLicense, Dto.Office365MassLicense>().ReverseMap();
            Mapper.CreateMap<DomainModel.Office365RetentionPolicy, Dto.Office365RetentionPolicy>().ReverseMap();
            Mapper.CreateMap<DomainModel.Office365RoleAssignmentPolicy, Dto.Office365RoleAssignmentPolicy>().ReverseMap();
            Mapper.CreateMap<DomainModel.Office365ServicePlan, Dto.Office365ServicePlan>().ReverseMap();
            Mapper.CreateMap<DomainModel.Office365SharingPolicy, Dto.Office365SharingPolicy>().ReverseMap();
            Mapper.CreateMap<DomainModel.Office365UserInfo, Dto.Office365UserInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.ServicePlanDetail, Dto.ServicePlanDetail>().ReverseMap();
            Mapper.CreateMap<DomainModel.AdHocLicenseInfo, Dto.AdHocLicenseInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.AdHocO365Request, Dto.AdHocO365Request>().ReverseMap();
            Mapper.CreateMap<DomainModel.AdHocO365SharedAccount, Dto.AdHocO365SharedAccount>().ReverseMap();
            #endregion

            #region SMS
            Mapper.CreateMap<DomainModel.SMSXLProvisioningMatrix, Dto.SMSXLProvisioningMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SMSXLRoleCatalogue, Dto.SMSXLRoleCatalogue>().ReverseMap();
            Mapper.CreateMap<DomainModel.SMSUserInfo, Dto.SMSUserInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SMSCompany, Dto.SMSCompany>().ReverseMap();
            Mapper.CreateMap<DomainModel.SMSBadgeInfo, Dto.SMSBadgeInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SMSAccessLevel, Dto.SMSAccessLevel>().ReverseMap();
            #endregion

            #region Org
            Mapper.CreateMap<DomainModel.PersonnelArea, Dto.PersonnelArea>().ReverseMap();
            Mapper.CreateMap<DomainModel.PersonnelSubArea, Dto.PersonnelSubArea>().ReverseMap();
            Mapper.CreateMap<DomainModel.Company, Dto.Company>().ReverseMap();
            Mapper.CreateMap<DomainModel.NOrganisation, Dto.NOrganisation>().ReverseMap();
            #endregion

            #region NOVA
            Mapper.CreateMap<DomainModel.NOVAUserRoleMatrix, Dto.NOVAUserRoleMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.NOVAManagementRoleMatrix, Dto.NOVAManagementRoleMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.NOVAExceptionalRoleMatrix, Dto.NOVAExceptionalRoleMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.NOVASoDExceptionsMatrix, Dto.NOVASoDExceptionsMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.NOVACompositeRoleMatrix, Dto.NOVACompositeRoleMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.AdHocNovaRequest, Dto.AdHocNovaRequest>().ReverseMap();
           

            #endregion

            #region MISC
            Mapper.CreateMap<DomainModel.ApproverExceptionInfo, ApproverExceptionInfo>().ReverseMap();
            #endregion

            #region VPN
            Mapper.CreateMap<DomainModel.VPNUserInfo, VPNUserInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.VPNMatrix, VPNMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.VPNApplicationEnvironment, VPNApplicationEnvironment>().ReverseMap();
            Mapper.CreateMap<DomainModel.VPNUserApplicationEnviornment, VPNUserApplicationEnviornment>().ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationVPNUserEntitlement, PresentationVPNUserEntitlement>().ReverseMap();
            #endregion
            #region SkypeForBusiness
            
            Mapper.CreateMap<DomainModel.SkypeForBusinessUnAssignedNumber, SkypeForBusinessUnAssignedNumber>().ReverseMap();
            Mapper.CreateMap<DomainModel.SkypeForBusinessProvisioningMatrix, SkypeForBusinessProvisioningMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SkypeForBusinessUnAssignedNumberMatrix, SkypeForBusinessUnAssignedNumberMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SkypeForBusinessResponseGroupInfo, SkypeForBusinessResponseGroupInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SkypeForBusinessUserInfo, SkypeForBusinessUserInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationSkypeForBusinessUserEntitlement, PresentationSkypeForBusinessUserEntitlement>().ReverseMap();
            Mapper.CreateMap<DomainModel.AdhocSkypeForBusinessRequest, AdhocSkypeForBusinessRequest>().ReverseMap();

            #endregion
            Mapper.CreateMap<DomainModel.SRMHangingRoleInfo, Dto.SRMHangingRoleInfo>()
                    .ReverseMap();

            #region PUB 
            Mapper.CreateMap<DomainModel.PUBEmployees, Dto.PUBEmployees>()
                    .ReverseMap();

            Mapper.CreateMap<DomainModel.PUBFile, Dto.PUBFile>()
                    .ReverseMap();
            #endregion
            Mapper.CreateMap<DomainModel.TaskReminder, Dto.TaskReminder>().ReverseMap();
            Mapper.CreateMap(typeof(DomainModel.IDMSHttpResponse<>), typeof(Dto.IDMSHttpResponse<>)).ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationSystemDecommission, Dto.PresentationSystemDecommission>()
                   .ReverseMap();
            Mapper.CreateMap<DomainModel.PresentationAccountEntitlementAuditTrial, Dto.PresentationAccountEntitlementAuditTrial>()
                   .ReverseMap();

            Mapper.CreateMap<DomainModel.ViewUserEntitlementData, Dto.ViewUserEntitlementData>().ReverseMap();
            Mapper.CreateMap<DomainModel.KeyValueData, Dto.KeyValueData>().ReverseMap();

            #region SAPHANA
            Mapper.CreateMap<DomainModel.SAPHANAAuthorizationProfile, SAPHANAAuthorizationProfile>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANACompositeRoleInfo, SAPHANACompositeRoleInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAConflictingRoleReport, SAPHANAConflictingRoleReport>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANACriticalTcodeReport, SAPHANACriticalTcodeReport>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAFieldValueMapping, SAPHANAFieldValueMapping>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAGroupInfo, SAPHANAGroupInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAHRP1001, SAPHANAHRP1001>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAObjectInfo, SAPHANAObjectInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAParameterHistoryInfo, SAPHANAParameterHistoryInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAParameterInfo, SAPHANAParameterInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAPosition, SAPHANAPosition>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAProfileInfo, SAPHANAProfileInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANARoleInfo, SAPHANARoleInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANARoleProfileMapping, SAPHANARoleProfileMapping>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANARoleTCodeMapping, SAPHANARoleTCodeMapping>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANARoleToAuthMap, SAPHANARoleToAuthMap>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANARoleToTCodes, SAPHANARoleToTCodes>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANASingleRoleInfo, SAPHANASingleRoleInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANATCodeInfo, SAPHANATCodeInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAUserHistoryInfo, SAPHANAUserHistoryInfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAUserinfo, SAPHANAUserinfo>().ReverseMap();
            Mapper.CreateMap<DomainModel.SapHANAUserProfile, SapHANAUserProfile>().ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPHANAUserinfo, Dto.Presentation.PresentationSAPUserinfo>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPHANATCodeReport, Dto.Presentation.PresentationSAPTCodeReport>()
                .ReverseMap();

            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPHANARoleToAuthorizationReport, Dto.Presentation.PresentationSAPRoleToAuthorizationReport>()
                .ReverseMap();
           
            Mapper.CreateMap<DomainModel.Presentation.PresentationSAPHANAUsersToGroups, Dto.Presentation.PresentationSAPUsersToGroups>()
                .ReverseMap();
           
            Mapper.CreateMap<DomainModel.SAPHANACompositeRoleInfo, Dto.SAPHANACompositeRoleInfo>().ReverseMap();

            Mapper.CreateMap<DomainModel.SAPHANAXLCriticalTCodeMatrix, Dto.SAPHANAXLCriticalTCodeMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAXLPositionsMatrix, Dto.SAPHANAXLPositionsMatrix>().ReverseMap();
            Mapper.CreateMap<DomainModel.SAPHANAXLRolesMatrix, Dto.SAPHANAXLRolesMatrix>().ReverseMap();


            #endregion
        }

        public static TModel DtoUnMap<TDto, TModel>(TDto dto)
        {
            if (dto == null)
                return default(TModel);

            return Mapper.Map<TDto, TModel>(dto);
        }

        public static List<TModel> DtoUnMapList<TDto, TModel>(List<TDto> dtoList)
        {
            return dtoList.Select(DtoUnMap<TDto, TModel>).ToList();
        }
    }

    //public class EndPointConfigurationConverter : ITypeConverter<Dto.IEndpointConfiguration, DomainModel.IEndpointConfiguration>
    //{
    //    public DomainModel.IEndpointConfiguration Convert(ResolutionContext context)
    //    {
    //        return Mapper.DynamicMap<DomainModel.ADSConfiguration>(context.SourceValue);
    //    }
    //}
}