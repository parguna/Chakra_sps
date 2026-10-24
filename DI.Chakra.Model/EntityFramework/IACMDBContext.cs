
using DI.Chakra.Model.SAP;
using DI.Chakra.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.ModelConfiguration;
using DI.Chakra.Model.Core;
using DI.Chakra.Model.GridSettings;
using DI.Chakra.Model.Core.Endpoint;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model.SOD;
using DI.Chakra.Model.Core.House_Keeping;
using System;

namespace DI.Chakra.Model
{
    public class IACMDBContext : System.Data.Entity.DbContext
    {
        public class GroupMap : EntityTypeConfiguration<Group>
        {
            public GroupMap()
            {
                this.HasMany(e => e.Users)
                  .WithMany(set => set.Groups)
                  .Map(mc =>
                  {
                      mc.ToTable("UserGroup");
                      mc.MapLeftKey("Group_GroupId");
                      mc.MapRightKey("User_UserId");
                  });
            }
        }
        public class RoleMap : EntityTypeConfiguration<Role>
        {
            public RoleMap()
            {
                this.HasMany(e => e.Users)
                  .WithMany(set => set.Roles)
                  .Map(mc =>
                  {
                      mc.ToTable("UserRole");
                      mc.MapLeftKey("Role_RoleId");
                      mc.MapRightKey("User_UserId");
                  });
            }
        }
        public class DiscoveryProcessMap : EntityTypeConfiguration<DiscoveryProcess>
        {
            public DiscoveryProcessMap()
            {
                this.HasMany(e => e.EndpointObjects)
                  .WithMany(set => set.DiscoveryProcess)
                  .Map(mc =>
                  {
                      mc.ToTable("DiscoveryProcessEndpointObjects");
                      mc.MapLeftKey("DiscoveryProcess_Id");
                      mc.MapRightKey("EndpointObjects_Id");
                  });
            }
        }
        //public class SODRiskCodeMap : EntityTypeConfiguration<SODRiskCode>
        //{
        //    public SODRiskCodeMap()
        //    {
        //        this.HasMany(s => s.Mitigations)
        //            .WithMany(s => s.RiskCodes)
        //            .Map(mc =>
        //            {
        //                mc.ToTable("SODMitigationSODRiskCode");
        //                mc.MapLeftKey("SODRiskCode_RiskId");
        //                mc.MapRightKey("SODMitigation_MitigationId");
        //            });
        //    }
        //}
        public IACMDBContext()
            : base("name=" + AppSettingsHelper.DatabaseConnectionStringName)
        {
            Database.SetInitializer<IACMDBContext>(null);
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<EndPoint>()
            //    .HasRequired(s => s.DiscoveryProcess)
            //    .WithOptional()
            //    .WillCascadeOnDelete(true);

            //throw new UnintentionalCodeFirstException();
            modelBuilder.Configurations.Add(new GroupMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new DiscoveryProcessMap());
            //modelBuilder.Configurations.Add(new SODRiskCodeMap());
        }

        public DbSet<EndPointSystemType> EndpointSystemTypes { get; set; }
        public DbSet<EntityModel.Tenant> Tenants { get; set; }


        public DbSet<EntityModel.MailServerConfiguration> MailServerConfiguration { get; set; }
        public DbSet<MailTemplate> MailTemplate { get; set; }
        public DbSet<EntityModel.MailTemplateVariable> MailTemplateVariables { get; set; }

        public DbSet<EntityModel.ConfigGeneralTrustedSource> TrustedSources { get; set; }

        public DbSet<ADSUserInfo> ADSUserInfos { get; set; }
        public DbSet<ADSGroupInfo> ADSGroupInfos { get; set; }
        public DbSet<ADSComputerInfo> ADSComputerInfo { get; set; }
        public DbSet<ADSComputerGroup> ADSComputerGroup { get; set; }
        public DbSet<ADSGroupInfoHistory> ADSGroupInfoHistorys { get; set; }
        public DbSet<ADSOuInfo> ADSOuInfos { get; set; }
        public DbSet<ADSOuInfoHistory> ADSOuInfoHistorys { get; set; }
        public DbSet<ADSPrinterInfo> ADSPrinterInfos { get; set; }
        public DbSet<ADSSharedFolderInfo> ADSSharedFolderInfos { get; set; }
        public DbSet<ADSUserInfoHistory> ADSUserInfoHistorys { get; set; }


        public DbSet<RoleHistory> RoleHistories { get; set; }
        public DbSet<AccessRights> AccessRights { get; set; }
        public DbSet<AccessRightHistory> AccessRightHistories { get; set; }
        public DbSet<AdminModeItem> AdminModeItems { get; set; }
        public DbSet<TabMenu> TabMenu { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<WebPage> Pages { get; set; }
        public DbSet<PageUrl> PageUrls { get; set; }
        public DbSet<PagePermission> PagePermissions { get; set; }
        public DbSet<EndPoint> EndpointSystems { get; set; }
        public DbSet<ADSConfiguration> ADSConfiguration { get; set; }
        public DbSet<ADSConfigurationHistory> ADSConfigurationHistories { get; set; }
        public DbSet<SAPConfiguration> SapConfiguration { get; set; }
        public DbSet<SAPEPConfiguration> SapEpConfiguration { get; set; }
        public DbSet<LNSConfiguration> lnsConfiguration { get; set; }
        public DbSet<SqlConfiguration> SqlConfiguration { get; set; }
        public DbSet<COPConfiguration> ABCConfiguration { get; set; }
        public DbSet<DiscoveryProcess> DiscoveryProcess { get; set; }
        public DbSet<Scheduler> Scheduler { get; set; }
        public DbSet<EndpointObjects> EndpointObjects { get; set; }

        public DbSet<EntityModel.ApprovalUserTypes> ApprovalUserTypes { get; set; }
        public DbSet<Attestation> Attestations { get; set; }
        public DbSet<AttestationEndpoint> AttestationEndpoints { get; set; }
        public DbSet<AttestationProcess> AttestationProcesses { get; set; }
        public DbSet<AttestationProcessHistory> AttestationProcessHistories { get; set; }
        public DbSet<AttestationReview> AttestationReviews { get; set; }
        public DbSet<AttestationReviewEndpointSystem> AttestationReviewEndpointSystems { get; set; }
        public DbSet<AttestationReviewEndpointSystemHistory> AttestationReviewEndpointSystemHistories { get; set; }
        public DbSet<AttestationReviewEndpointSystemL2Detail> AttestationReviewEndpointSystemL2Details { get; set; }
        public DbSet<AttestationReviewEndpointSystemL2DetailHistory> AttestationReviewEndpointSystemL2DetailHistories { get; set; }
        public DbSet<AttestationReviewEndpointSystemL3Detail> AttestationReviewEndpointSystemL3Details { get; set; }
        public DbSet<AttestationReviewEndpointSystemL3DetailHistory> AttestationReviewEndpointSystemL3DetailHistories { get; set; }
        public DbSet<RequestTypes> RequestTypes { get; set; }
        public DbSet<InterimStore> InterimStores { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<TaskDelegation> TaskDelegations { get; set; }
        public DbSet<TaskReview> TaskReviews { get; set; }
        public DbSet<ReviewEndpointSystem> ReviewEndpointSystems { get; set; }
        public DbSet<WorkflowConfigurationDetail> WorkflowConfigurationDetails { get; set; }
        public DbSet<WorkflowConfiguration> WorkflowConfigurations { get; set; }
        public DbSet<EntityModel.RoleTypes> RoleTypes { get; set; }
        public DbSet<EntityModel.EmployeeTerm> EmployeeTerms { get; set; }
        public DbSet<EntityModel.AccessRequestCategory> AccessRequestCategories { get; set; }
        public DbSet<DI.Chakra.Model.AccessRequestProgressClass> AccessRequestProgress { get; set; }
        public DbSet<DI.Chakra.Model.AccessRequestSourceClass> AccessRequestSources { get; set; }
        public DbSet<DI.Chakra.Model.AccessRequestStatusClass> AccessRequestStatus { get; set; }   
        public DbSet<EntityModel.AccountActionCategory> AccountActionCategories { get; set; }
        public DbSet<OffboardingAccountAction> OffboardingAccountAction { get; set; }
        public DbSet<EntityModel.AccountCategory> AccountCategories { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<AccountReconcilation> AccountReconcilations { get; set; }
        public DbSet<EntitlementReconcilation> EntitlementReconcilations { get; set; }
        public DbSet<RoleMatrix> RoleMatrices { get; set; }
        public DbSet<RoleMatrixDetail> RoleMatrixDetails { get; set; }
        public DbSet<ConfigurationMaster> ConfigurationMasters { get; set; }
        public DbSet<EntityModel.ConfigurationMasterHistory> ConfigurationMasterHistories { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<EmployeeEventTrigger> EmployeeEventTriggers { get; set; }
        public DbSet<AdHocAccessRequest> AdHocAccessRequests { get; set; }

        public DbSet<PasswordSyncConfiguration> PasswordSyncConfigurations { get; set; }
        public DbSet<PasswordSyncConfigurationDetail> PasswordSyncConfigurationDetails { get; set; }


        public DbSet<Organisation> Organisations { get; set; }


        public DbSet<AccessRequest> AccessRequests { get; set; }
        public DbSet<AccessRequestHistory> AccessRequestHistories { get; set; }
        public DbSet<ProvisioningForm> ProvisioningForms { get; set; }
        public DbSet<ProvisioningFormAD> ProvisioningFormADs { get; set; }
        public DbSet<ProvisioningFormGeneralData> ProvisioningFormGeneralDatas { get; set; }
        public DbSet<EntityModel.DefaultProvisioningFormGeneralData> DefaultProvisioningFormGeneralDatas { get; set; }
        public DbSet<ProvisioningFormLNS> ProvisioningFormLNSs { get; set; }
        public DbSet<ProvisioningFormSAP> ProvisioningFormSAPs { get; set; }
        public DbSet<ProvisioningFormSAPEP> ProvisioningFormSAPEPs { get; set; }

        public DbSet<ProvisioningRole> ProvisioningRoles { get; set; }
        public DbSet<ProvisioningRoleDetail> ProvisioningRoleDetails { get; set; }


        public DbSet<SAPRoleInfo> SAPRoleInfos { get; set; }
        public DbSet<SAPParameterInfo> SAPParameterInfos { get; set; }
        public DbSet<SAPParameterHistoryInfo> SAPParameterHistoryInfos { get; set; }

        public DbSet<SAPGroupHistoryInfo> SAPGroupHistoryInfos { get; set; }

        public DbSet<DiscoveryProcessHistory> DiscoveryProcessHistorys { get; set; }

        public DbSet<SAPEPRoleInfoTemp> SAPEPRoleInfoTemps { get; set; }

        public DbSet<AttributeMappingColumns> AttributeMappingColumnss { get; set; }

        public DbSet<PasswordPolicy> PasswordPolicies { get; set; }
        public DbSet<PasswordPolicyEndpoint> PasswordPolicyEndpoints { get; set; }


        #region USER&GROUP
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupHistory> GroupHistories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EntityModel.UserImage> UserImages { get; set; }
        public DbSet<Role> Roles { get; set; }

        #endregion

        #region LOTUSNOTES
        public DbSet<LotusGroupInfo> LotusGroupInfos { get; set; }
        public DbSet<LotusUserInfo> LotusUserInfos { get; set; }
        public DbSet<LotusCertifierInfo> LotusCertifierInfos { get; set; }
        public DbSet<LotusUserHistoryInfo> LotusUserHistoryInfos { get; set; }
        public DbSet<LotusGroupHistoryInfo> LotusGroupHistoryInfos { get; set; }
        public DbSet<LNSMailQuotaConfig> LNSMailQuotaConfigs { get; set; }
        #endregion

        #region SAPEP
        public DbSet<SAPEPGroupInfo> SAPEPGroupInfos { get; set; }
        public DbSet<SAPEPRoleInfo> SAPEPRoleInfos { get; set; }
        public DbSet<SAPEPUserInfo> SAPEPUserInfos { get; set; }
        public DbSet<SAPEPGroupInfoHistory> SAPEPGroupInfoHistorys { get; set; }
        public DbSet<SAPEPRoleInfoHistory> SAPEPRoleInfoHistorys { get; set; }
        public DbSet<SAPEPUserInfoHistory> SAPEPUserInfoHistorys { get; set; }
        #endregion

        #region SAP
        public DbSet<SAPUserinfo> SAPUserinfos { get; set; }
        public DbSet<SAPGroupInfo> SAPGroupInfos { get; set; }
        public DbSet<SAPCompositeRoleInfo> SAPCompositeRoleInfos { get; set; }
        public DbSet<SAPSingleRoleInfo> SAPSingleRoleInfos { get; set; }
        public DbSet<SAPProfileInfo> SAPProfileInfos { get; set; }
        public DbSet<SAPTCodeInfo> SAPTCodeInfos { get; set; }
        public DbSet<SAPPosition> SAPPositions { get; set; }
        public DbSet<SAPUserHistoryInfo> SAPUserHistoryinfos { get; set; }
        public DbSet<SAPCompositeProfileInfo> SAPCompositeProfileInfos { get; set; }
        public DbSet<SAPAuthorizationProfile> SAPAuthorizationInfos { get; set; }
        public DbSet<SAPAuthorizationToObject> SapAuthorizationToObject { get; set; }
        public DbSet<SAPFieldValueMapping> SAPFieldValueMap { get; set; }
        public DbSet<SAPObjectValues> SapObjectValues { get; set; }
        public DbSet<SAPTCodeObjectMap> SapTCodeObjectMap { get; set; }
        public DbSet<SAPObjectInfo> SapObject { get; set; }
        public DbSet<SapUserProfile> SapUserProfile { get; set; }
        public DbSet<SAPRoleToAuthMap> SapRoleToAuthMap { get; set; }
        public DbSet<SapRoleTCodeMapping> SapRoleTCodeMap { get; set; }
        public DbSet<SAPHRP1001> SAPHRP1001Entries { get; set; }
        public DbSet<SAPActivityInfo> SapActivityInfos { get; set; }
        //public DbSet<SAPUserRoleTCodeMapInfo> SAPUserRoleTCodeMap { get; set; }
        //public DbSet<SAPUserRoleMapInfo> SapUserRoleMapInfo { get; set; }
        #endregion

        public DbSet<EmailInfo> EmailInfos { get; set; }
        public DbSet<EntityModel.EmailInfoHistory> EmailHistoryInfos { get; set; }

        #region SOD
        public DbSet<SODReport> SodComplianceReport { get; set; }
        public DbSet<SODPolicy> SodPolicy { get; set; }
        public DbSet<SODRiskLevel> SodRiskLevel { get; set; }
        public DbSet<SODRiskCode> SodRiskId { get; set; }
        public DbSet<SodFunction> SodFunction { get; set; }
        public DbSet<SodFunctionItem> SodFunctionItem { get; set; }

        public DbSet<SODConflict> SodConflict { get; set; }
        public DbSet<ComplianceType> ComplianceType { get; set; }
        public DbSet<SODMitigation> SodMitigation { get; set; }
        public DbSet<CompliancePolicy> CompliancePolicy { get; set; }
        public DbSet<SODException> SodException { get; set; }
        public DbSet<SODConflictFunction> SodConflictFunction { get; set; }
        public DbSet<SODExecutionConfig> SodExecutionConfig { get; set; }
        public DbSet<SODRunPolicy> SodRunPolicy { get; set; }
        public DbSet<SodExceptionItem> SodExceptionItem { get; set; }
        public DbSet<SODRiskScore> SodRiskScore { get; set; }
        public DbSet<ComplianceReportModules> ComplainceReports { get; set; }

        #endregion

        public DbSet<ADSXLProvisioningMatrix> ADSXLProvisioningMatrices { get; set; }
        public DbSet<LNSXLProvisioningMatrix> LNSXLProvisioningMatrices { get; set; }
        public DbSet<SAPXLProvisioningMatrix> SAPXLProvisioningMatrices { get; set; }
        public DbSet<OracleEBSXLProvisioningMatrix> OracleEBSXLProvisioningMatrices { get; set; }
        public DbSet<LINUXXLProvisioningMatrix> LINUXXLProvisioningMatrices { get; set; }

        public DbSet<OracleXLProvisioningMatrix> OracleUMXLProvisioningMatrix { get; set; }

        #region Logger

        public DbSet<ServiceLog> ServiceLog { get; set; }
       
        public DbSet<UILog> UILog { get; set; }
        //public DbSet<ServiceLog> ServiceLog { get; set; }
       
        #endregion

        public DbSet<VPNMaster> VPNMasters { get; set; }

        #region Inhouse Systems

        public DbSet<InhouseSystemConfiguration> InhouseSystems { get; set; }

        public DbSet<InhouseUserInfo> InhouseUserInfos { get; set; }
        public DbSet<InhousePermission> InhousePermissions { get; set; }

        #endregion

        public DbSet<SAPPositionsMatrix> SAPPositionsMatrices { get; set; }
        public DbSet<SAPCriticalTCodeMatrix> SAPCriticalTCodeMatrices { get; set; }
        public DbSet<SAPRoleCatalog> SAPRoleCatalogs { get; set; }


        #region OracleEBS

        public IDbSet<OracleEBSConfiguration> OracleEBSConfiguration { get; set; }
        public IDbSet<OracleEBSHRMSUserInfo> OracleEBSHRMSUserInfo { get; set; }
        public IDbSet<OracleEBSHRMSUserInfoHistory> OracleEBSHRMSUserInfoHistory { get; set; }

        public IDbSet<OracleEBSUserInfo> OracleEBSUserInfo { get; set; }
        public IDbSet<OracleEBSUserInfoHistory> OracleEBSUserInfoHistory { get; set; }

        public IDbSet<OracleEBSRoleInfo> OracleEBSRoleInfo { get; set; }
        public IDbSet<OracleEBSRoleInfoHistory> OracleEBSRoleInfoHistory { get; set; }

        public IDbSet<OracleEBSResponsibilitiesInfo> OracleEBSResponsibilitiesInfo { get; set; }
        public IDbSet<OracleEBSResponsibilitiesInfoHistory> OracleEBSResponsibilitiesInfoHistory { get; set; }

        public DbSet<OracleEBSMenusInfo> OracleEBSMenusInfos { get; set; }

        public DbSet<OracleEBSSubmenusInfo> OracleEBSSubmenusInfos { get; set; }

        public DbSet<ProvisioningFormOracleEBS> ProvisioningFormOracleEBSs { get; set; }
        #endregion



        #region Linux
        public DbSet<LinuxConfiguration> linuxConfigurations { get; set; }
        public DbSet<LinuxUserInfo> LinuxUserInfos { get; set; }
        public DbSet<LinuxUserInfoHistory> LinuxUserInfoHistories { get; set; }
        public DbSet<LinuxGroupInfo> LinuxGroupInfos { get; set; }
        public DbSet<LinuxGroupInfoHistory> LinuxGroupInfoHistories { get; set; }
        public DbSet<LinuxResourceInfo> LinuxResourceInfos { get; set; }
        public DbSet<LinuxResourceInfoHistory> LinuxResourceInfoHistories { get; set; }
        //public DbSet<LinuxUserGroup> LinuxUserGroup { get; set; }
        public DbSet<ProvisioningFormLINUX> ProvisioningFormLINUX { get; set; }
        #endregion


        public DbSet<OUChangeSnapshot> OUChangeSnapshots { get; set; }


        ///New Tables added for Preserving Grid Values
        /// Added by Harihara : 16/6/2015.
        public DbSet<UserGridInfo> UserGridInfos { get; set; }

        public DbSet<UserFilterInfo> UserFilterInfos { get; set; }

        public DbSet<ProcessHistory> ProcessHistory { get; set; }

        public DbSet<GlobalQuestion> GlobalQuestions { get; set; }
        public DbSet<GlobalAnswer> GlobalAnswers { get; set; }

        public DbSet<Process> Process { get; set; }
        public DbSet<UserQuickLink> UserQuickLinks { get; set; }
        public DbSet<QuickLink> QuickLinks { get; set; }
        public DbSet<DefaultRoleQuickLink> DefaultRoleQuickLink { get; set; }
        public DbSet<QuickLinkAccessRights> QuickLinkAccessRights { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<DashboardAccessRights> DashboardAccessRights { get; set; }
        public DbSet<DefaultRoleDashboardPreference> DefaultRoleDashboardPreferences { get; set; }
        public DbSet<UserDashboardPreference> UserDashboardPreferences { get; set; }
        public DbSet<EntityModel.UserCulturePreference> UserCulturePreferences { get; set; }

        public DbSet<ApproverException> ApproverExceptions { get; set; }

        public DbSet<UserGridSettings> UserGridSettings { get; set; }
        public DbSet<UserGridFilterSettings> UserGridFilterSettings { get; set; }

        public DbSet<Ruleset> Rulesets { get; set; }
        public DbSet<EntityModel.RunTimeCodeSnippet> RunTimeCodeSnippets { get; set; }

        public DbSet<RequestApprovalConfiguration> RequestApprovalConfigurations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RequestAttachment> RequestAttachments { get; set; }
        
        #region Oracle11gUserManagement

        public IDbSet<OracleUMConfiguration> OracleUMConfiguration { get; set; }
        public IDbSet<OracleUMRoleInfo> OracleUMRoleInfo { get; set; }
        public IDbSet<OracleUMRoleInfoHistory> OracleUMRoleInfoHistory { get; set; }
        public IDbSet<OracleUMTableInfo> OracleUMTableInfo { get; set; }
        public IDbSet<OracleUMUserInfo> OracleUMUserInfo { get; set; }
        public IDbSet<OracleUMUserInfoHistory> OracleUMUserInfoHistory { get; set; }
        public IDbSet<OracleUMUserRole> OracleUMUserRole { get; set; }
        public IDbSet<OracleUMUserTable> OracleUMUserTable { get; set; }
        public IDbSet<OracleUMUserTableMap> OracleUMUserTableMap { get; set; }

        public DbSet<ProvisioningFormORCLUM> ProvisioningFormOracle11gs { get; set; } 

        #endregion

        public DbSet<EndPointFunctionType> EndPointFunctionTypes { get; set; }

        #region Compliance
        public DbSet<SODEndPointPolicySelection> SODEndPointPolicySelection { get; set; }
        public DbSet<SODPolicyExceutionDefinition> SODPolicyExceutionDefinitions { get; set; }

        public DbSet<ComplianceProcess> ComplianceProcess { get; set; }
        public DbSet<ComplianceProcessHistory> ComplianceProcessHistory { get; set; }

        public DbSet<SODPolicyReport> SODPolicyReport { get; set; }

        #endregion

        public DbSet<ADSCatalog> ADSCatalogs { get; set; }
        public DbSet<LNSCatalog> LNSCatalogs { get; set; }
        public DbSet<EntityModel.PurgeConfig> PurgeConfigs { get; set; }
        public DbSet<INHCatalog> INHCatalogs { get; set; } 

        #region AdminModeItem
        public DbSet Set(string name)
        {
            return base.Set(Type.GetType(name));
        }
        #endregion

    }
}
