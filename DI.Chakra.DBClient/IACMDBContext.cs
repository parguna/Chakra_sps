using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrackerEnabledDbContext;
using TrackerEnabledDbContext.Common.Configuration;



namespace DI.Chakra.DBClient
{
    public class IACMDBContext : TrackerEnabledDbContext.TrackerContext
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
        public class AttestationComparisonMap : EntityTypeConfiguration<AttestationTaskEntitlementUserComparision>
        {
            public AttestationComparisonMap()
            {
                this.HasKey(t => t.Id);
                // Table & Column Mappings
                this.ToTable("vwAttestationTaskEntitlementUserComparision");
                this.Property(t => t.TaskId).HasColumnName("TaskId");
                this.Property(t => t.Entitlement).HasColumnName("Entitlement");
                this.Property(t => t.EntitlementDescription).HasColumnName("EntitlementDescription");
                this.Property(t => t.EntitlementType).HasColumnName("EntitlementType");
                this.Property(t => t.SnapshotActionId).HasColumnName("SnapshotActionId");
                this.Property(t => t.Name).HasColumnName("Name");
                this.Property(t => t.UserName).HasColumnName("UserName");
                this.Property(t => t.EmployeeNo).HasColumnName("EmployeeNo");
                this.Property(t => t.EmployeeType).HasColumnName("EmployeeType");
                this.Property(t => t.TerminationDate).HasColumnName("TerminationDate");
                this.Property(t => t.UserId).HasColumnName("UserId");
                this.Property(t => t.AccountType).HasColumnName("AccountType");
                this.Property(t => t.EndpointSystemId).HasColumnName("EndpointSystemId");
                this.Property(t => t.EndpointSystemName).HasColumnName("EndpointSystemName");
                this.Property(t => t.EndpointType).HasColumnName("EndpointType");
                this.Property(t => t.Approve).HasColumnName("Approve");
                this.Property(t => t.Revoke).HasColumnName("Revoke");
                this.Property(t => t.IsActionable).HasColumnName("IsActionable");
            }
        }
        public class AttestationComparisonHistoryMap : EntityTypeConfiguration<AttestationTaskHistoryEntitlementUserComparision>
        {
            public AttestationComparisonHistoryMap()
            {
                this.HasKey(t => t.Id);
                // Table & Column Mappings
                this.ToTable("vwAttestationTaskHistoryEntitlementUserComparision");
                this.Property(t => t.TaskId).HasColumnName("TaskId");
                this.Property(t => t.Entitlement).HasColumnName("Entitlement");
                this.Property(t => t.EntitlementDescription).HasColumnName("EntitlementDescription");
                this.Property(t => t.EntitlementType).HasColumnName("EntitlementType");
                this.Property(t => t.SnapshotActionId).HasColumnName("SnapshotActionId");
                this.Property(t => t.Name).HasColumnName("Name");
                this.Property(t => t.UserName).HasColumnName("UserName");
                this.Property(t => t.EmployeeNo).HasColumnName("EmployeeNo");
                this.Property(t => t.EmployeeType).HasColumnName("EmployeeType");
                this.Property(t => t.TerminationDate).HasColumnName("TerminationDate");
                this.Property(t => t.UserId).HasColumnName("UserId");
                this.Property(t => t.AccountType).HasColumnName("AccountType");
                this.Property(t => t.EndpointSystemId).HasColumnName("EndpointSystemId");
                this.Property(t => t.EndpointSystemName).HasColumnName("EndpointSystemName");
                this.Property(t => t.EndpointType).HasColumnName("EndpointType");
                this.Property(t => t.Approve).HasColumnName("Approve");
                this.Property(t => t.Revoke).HasColumnName("Revoke");
                this.Property(t => t.IsActionable).HasColumnName("IsActionable");
            }
        }
        static IACMDBContext()
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public IACMDBContext()
            : base("name=" + AppSettingsHelper.DatabaseConnectionStringName)
        //: base("name=POC")
        {
            Database.SetInitializer<IACMDBContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            if (AppSettingsHelper.IsFirstTimeDBExecute)
                DataAudit();

            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AttestationTaskEntitlementUserComparision>()
            .ToTable("vwAttestationTaskEntitlementUserComparision");
            modelBuilder.Entity<AttestationTaskHistoryEntitlementUserComparision>()
            .ToTable("vwAttestationTaskHistoryEntitlementUserComparision");
            modelBuilder.Configurations.Add(new GroupMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new AttestationComparisonMap());
            modelBuilder.Configurations.Add(new AttestationComparisonHistoryMap());
        }
        public void DataAudit()
        {
            Auditing auditing = new Auditing();
            Dictionary<string, List<string>> auditConfigurationList = GetAuditConfigurationList();

            foreach (KeyValuePair<string, List<string>> item in auditConfigurationList)
            {
                string formString = "DI.Chakra.EntityModel." + item.Key + ", DI.Chakra.EntityModel";
                Type type = Type.GetType(formString);
                List<string> listDiColumns = type.GetProperties().Select(x => x.Name).ToList();
                List<string> disableProperties = listDiColumns.Except(item.Value).ToList();
                typeof(Auditing)
                    .GetMethod("TrackProperties")
                    .MakeGenericMethod(type)
                    .Invoke(auditing, new object[] { disableProperties
                });
            }
        }

        private Dictionary<string, List<string>> GetAuditConfigurationList()
        {
            Dictionary<string, List<string>> auditConfigurationList = new Dictionary<string, List<string>>();
            string conn = ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(conn))
            using (SqlCommand cmd = new SqlCommand("select * from DI_M_AuditConfiguration", connection))
            {
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            long tableId = reader.GetInt64(reader.GetOrdinal("Id"));
                            string tableName = reader.GetString(reader.GetOrdinal("TableName"));
                            SqlCommand cmd1 = new SqlCommand("select * from DI_M_AuditConfigurationDetail where TableId = @tableId", connection);
                            cmd1.Parameters.AddWithValue("@tableId", tableId);
                            SqlDataReader reader1 = cmd1.ExecuteReader();
                            if (reader1.HasRows)
                            {
                                List<string> propertyList = new List<string>();
                                while (reader1.Read())
                                {
                                    string propertyName = reader1.GetString(reader1.GetOrdinal("PropertyName"));
                                    propertyList.Add(propertyName);
                                }
                                auditConfigurationList.Add(tableName, propertyList);
                                reader1.Close();
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return auditConfigurationList;
        }

        public DbSet<TabMenu> TabMenu { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<WebPage> Pages { get; set; }
        public DbSet<PageUrl> PageUrls { get; set; }
        public DbSet<PagePermission> PagePermissions { get; set; }
        public DbSet<AccessRightHistory> AccessRightHistories { get; set; }
        public DbSet<AccessRights> AccessRights { get; set; }
        public DbSet<RoleHistory> RoleHistories { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<UserExclusion> UserExclusions { get; set; }
        public DbSet<UserAttachment> UserAttachments { get; set; }
        public DbSet<GlobalQuestion> GlobalQuestion { get; set; }
        public DbSet<GlobalAnswer> GlobalAnswers { get; set; }

        #region SOD

        public DbSet<SODRiskLevel> SODRiskLevels { get; set; }
        public DbSet<SODPolicyConfiguration> SODPolicyConfigurations { get; set; }
        public DbSet<SODRiskScore> SODRiskScores { get; set; }
        public DbSet<SODRiskCode> SODRiskCodes { get; set; }
        public DbSet<SODMitigation> SODMitigations { get; set; }
        public DbSet<SODConflict> SODConflicts { get; set; }
        public DbSet<SODFunction> SODFunctions { get; set; }
        public DbSet<SODFunctionItem> SODFuntionItem { get; set; }
        public DbSet<CompliancePolicy> CompliancePolicy { get; set; }
        public DbSet<ComplianceType> ComplianceType { get; set; }
        public DbSet<SODException> SODException { get; set; }
        public DbSet<SODExceptionItem> SODExceptionItem { get; set; }

        public DbSet<SODPolicyExecutionDefinition> SODPolicyExecutionDefinition { get; set; }
        public DbSet<ComplianceProcessHistory> ComplianceProcessHistory { get; set; }
        public DbSet<SODConflictFunctions> SODConflictFunctions { get; set; }
        public DbSet<ComplianceReportModules> ComplianceReportModules { get; set; }
        public DbSet<SODRunPolicy> SodRunPolicy { get; set; }
        public DbSet<SODReport> SodComplianceReport { get; set; }
        public DbSet<SODRunPolicySelection> SODRunPolicySelections { get; set; }
        public DbSet<SODConflictFunctionMap> SODConflictFunctionMaps { get; set; }
        public DbSet<SODExecutionConfig> SodExecutionConfig { get; set; }
        public DbSet<SODPolicy> SodPolicy { get; set; }
        public DbSet<SODPolicyReport> SODPolicyReport { get; set; }
        public DbSet<SODEndPointPolicySelection> SODEndPointPolicySelection { get; set; }

        public DbSet<UserSODComplianceReport> UserSODComplianceReport { get; set; }
        #endregion

        #region Mail

        public DbSet<MailServerConfiguration> MailServerConfiguration { get; set; }
        public DbSet<MailTemplate> MailTemplate { get; set; }
        public DbSet<EmailInfo> EmailInfos { get; set; }
        public DbSet<EmailInfoHistory> EmailHistoryInfos { get; set; }

        #endregion

        public DbSet<PurgeConfig> PurgeConfigs { get; set; }

        public DbSet<UserImage> UserImage { get; set; }
        public DbSet<PasswordActionInfo> PasswordActionInfos { get; set; }

        #region MetaData

        public DbSet<AccountCategory> AccountCategories { get; set; }

        public DbSet<AccountActionCategory> AccountActionCategories { get; set; }

        public DbSet<AccessRequestCategory> AccessRequestCategories { get; set; }

        public DbSet<DI.Chakra.EntityModel.AccessRequestProgressTBD> AccessRequestProgress { get; set; }

        public DbSet<DI.Chakra.EntityModel.AccessRequestSourceTBD> AccessRequestSources { get; set; }

        public DbSet<DI.Chakra.EntityModel.AccessRequestStatusTBD> AccessRequestStatus { get; set; }
        public DbSet<OffboardingAccountAction> OffboardingAccountAction { get; set; }

        public DbSet<ApprovalUserTypes> ApprovalUserTypes { get; set; }

        public DbSet<RoleTypes> RoleTypes { get; set; }

        public DbSet<RequestTypes> RequestTypes { get; set; }

        public DbSet<EmployeeTerm> EmployeeTerms { get; set; }

        #endregion

        public DbSet<AdminModeItem> AdminModeItems { get; set; }

        public DbSet<UserCulturePreference> UserCulturePreferences { get; set; }

        #region Logs
        public DbSet<UILog> UILogs { get; set; }

        public DbSet<ServiceLog> ServiceLog { get; set; }
        public DbSet<ProvisioningLog> ProvisioningLog { get; set; }
        public DbSet<AttestationLog> AttestationLog { get; set; }
        public DbSet<DiscoveryLog> DiscoveryLog { get; set; }
        public DbSet<EmailLog> EmailLog { get; set; }

        #endregion

        public DbSet<Role> Roles { get; set; }
        public DbSet<QuickLink> QuickLinks { get; set; }
        public DbSet<QuickLinkAccessRights> QuickLinkAccessRights { get; set; }
        public DbSet<UserQuickLink> UserQuickLinks { get; set; }
        public DbSet<DefaultRoleQuickLink> DefaultRoleQuickLink { get; set; }
        public DbSet<UserGridFilterInfo> UserGridFilterInfos { get; set; }
        public DbSet<UserGridInfo> UserGridInfos { get; set; }
        public DbSet<UserGridColumnInfo> UserGridColumnInfos { get; set; }

        #region Tasks
        public DbSet<SubTask> SubTask { get; set; }
        public DbSet<InterimStore> InterimStores { get; set; }
        public DbSet<RequestAttachment> RequestAttachments { get; set; }
        public DbSet<TaskDelegation> TaskDelegations { get; set; }
        public DbSet<DI.Chakra.EntityModel.Task> Tasks { get; set; }
        #endregion

        #region Notifications
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationAttachment> NotificationAttachments { get; set; }
        #endregion

        #region Attachments
        public DbSet<Attachment> Attachments { get; set; }
        #endregion

        #region Common
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupHistory> GroupHistory { get; set; }
        #endregion

        #region Configuration
        public DbSet<ConfigurationMaster> ConfigurationMastersIACM { get; set; }
        public DbSet<ConfigurationMasterPIM> ConfigurationMastersPIM { get; set; }
        public DbSet<ConfigurationMasterPasswordManagement> ConfigurationMastersPasswordManagement { get; set; }
        public DbSet<ConfigurationMasterIM> ConfigurationMastersIM { get; set; }
        public DbSet<ConfigurationMasterDGM> ConfigurationMastersDGM { get; set; }
        public DbSet<ConfigurationMasterCompliance> ConfigurationMastersCompliance { get; set; }

        public DbSet<ConfigurationMasterHistory> ConfigurationMasterHistories { get; set; }
        public DbSet<PasswordSyncConfigurationDetail> PasswordSyncConfigurationDetail { get; set; }
        public DbSet<ConfigGeneralTrustedSource> TrustedSources { get; set; }
        public DbSet<PasswordPolicy> PasswordPolicy { get; set; }
        public DbSet<PasswordPolicyEndpoint> PasswordPolicyEndpoint { get; set; }
        public DbSet<PasswordSyncConfiguration> PasswordSyncConfiguration { get; set; }
        public DbSet<RequestApprovalConfiguration> RequestApprovalConfiguration { get; set; }
        public DbSet<AuditConfiguration> AuditLog { get; set; }
        public DbSet<AuditConfigurationDetail> AuditLogDetail { get; set; }
        #endregion

        #region Database

        public DbSet<SqlConfiguration> SqlConfiguration { get; set; }

        #endregion

        #region WorkFlow

        public DbSet<WorkflowConfigurationDetail> WorkflowConfigurationDetails { get; set; }
        public DbSet<WorkflowConfiguration> WorkflowConfigurations { get; set; }

        #endregion

        #region Discovery

        public DbSet<DiscoveryProcessHistory> DiscoveryProcessHistorys { get; set; }
        public DbSet<DiscoveryProcess> DiscoveryProcess { get; set; }

        #endregion
        #region Reconciliation
        public DbSet<ExceptionAttribute> ExceptionAttributes { get; set; }
        public DbSet<ExceptionAttributeChange> ExceptionAttributeChanges { get; set; }
        #endregion

        #region Provisioning

        public DbSet<EntitlementReconcilation> EntitlementReconcilations { get; set; }
        public DbSet<AccountReconcilation> AccountReconcilations { get; set; }
        public DbSet<EmployeeEventTrigger> EmployeeEventTriggers { get; set; }

        public DbSet<OUChangeSnapshot> OUChangeSnapshots { get; set; }

        public DbSet<AccessRequest> AccessRequest { get; set; }

        #endregion

        #region AdhocAccessRequest
        public DbSet<AdHocAccessRequest> AdHocAccessRequests { get; set; }
        public DbSet<AccessRequestConfiguration> AccessRequestConfiguration { get; set; }
        #endregion

        #region EndpointConfiguration
        public DbSet<EndpointConfiguration> EndpointConfigurations { get; set; }
        public DbSet<EndpointConfigurationDetail> EndpointConfigurationDetails { get; set; }
        public DbSet<EndpointConfigurationKey> EndpointConfigurationKeys { get; set; }
        public DbSet<EndpointKeyConfiguration> EndpointKeyConfigurations { get; set; }
        public DbSet<EndpointUserViewConfiguration> EndpointUserViewConfigurations { get; set; }
        #endregion

        #region "IACM"
        public DbSet<IACMGroupInfo> IACMGroupInfos { get; set; }
        public DbSet<IACMRoleInfo> IACMRoleInfos { get; set; }
        public DbSet<IACMUserInfo> IACMUserInfos { get; set; }
        #endregion

        #region "Active Directory"

        public DbSet<ADSUserInfo> ADSUserInfos { get; set; }
        public DbSet<ADSGroupInfo> ADSGroupInfos { get; set; }
        public DbSet<ADSComputerGroup> ADSComputerGroup { get; set; }
        public DbSet<ADSComputerInfo> ADSComputerInfo { get; set; }
        public DbSet<ADSGroupInfoHistory> ADSGroupInfoHistorys { get; set; }
        public DbSet<ADSOuInfo> ADSOuInfos { get; set; }
        public DbSet<ADSOuInfoHistory> ADSOuInfoHistorys { get; set; }
        public DbSet<ADSPrinterInfo> ADSPrinterInfos { get; set; }
        public DbSet<ADSSharedFolderInfo> ADSSharedFolderInfos { get; set; }
        public DbSet<ADSUserInfoHistory> ADSUserInfoHistorys { get; set; }
        public DbSet<ADSNestedGroup> ADSNestedGroups { get; set; }
        #endregion

        #region "SAP"
        public DbSet<SAPUserinfo> SAPUserinfos { get; set; }
        public DbSet<SAPActivityInfo> SAPActivityInfo { get; set; }
        public DbSet<SAPAuthorizationProfile> SAPAuthorizationProfile { get; set; }
        public DbSet<SAPAuthorizationToObject> SAPAuthorizationToObject { get; set; }
        public DbSet<SAPCompositeProfileInfo> SAPCompositeProfileInfo { get; set; }
        public DbSet<SAPCompositeRoleInfo> SAPCompositeRoleInfo { get; set; }
        public DbSet<SAPSingleRoleInfo> SAPSingleRoleInfo { get; set; }
        public DbSet<SAPTCodeInfo> SAPTCodeInfo { get; set; }
        public DbSet<SAPObjectInfo> SAPObjectInfo { get; set; }
        public DbSet<SAPObjectValues> SAPObjectValues { get; set; }
        public DbSet<SAPParameterHistoryInfo> SAPParameterHistoryInfo { get; set; }
        public DbSet<SAPParameterInfo> SAPParameterInfo { get; set; }
        public DbSet<SAPPosition> SAPPosition { get; set; }
        public DbSet<SAPProfileInfo> SAPProfileInfo { get; set; }
        public DbSet<SAPFieldValueMapping> SAPFieldValueMap { get; set; }
        public DbSet<SAPGroupHistoryInfo> SAPGroupHistoryInfos { get; set; }
        public DbSet<SAPGroupInfo> SAPGroupInfos { get; set; }
        public DbSet<SAPHRP1001> SAPHRP1001Entries { get; set; }
        public DbSet<SAPRoleInfo> SAPRoleInfos { get; set; }
        public DbSet<SapRoleProfileMapping> SapRoleProfileMappings { get; set; }
        public DbSet<SapRoleTCodeMapping> SapRoleTCodeMap { get; set; }
        public DbSet<SAPRoleToAuthMap> SapRoleToAuthMap { get; set; }
        public DbSet<SAPTCodeObjectMap> SapTCodeObjectMap { get; set; }
        public DbSet<SAPUserHistoryInfo> SAPUserHistoryinfos { get; set; }
        public DbSet<SapUserProfile> SapUserProfile { get; set; }
        public DbSet<SAPUserToObject> SAPUserToObjects { get; set; }
        public DbSet<SAPEPGroupInfoHistory> SAPEPGroupInfoHistorys { get; set; }
        public DbSet<SAPEPRoleInfoHistory> SAPEPRoleInfoHistorys { get; set; }
        public DbSet<SAPEPRoleInfoTemp> SAPEPRoleInfoTemps { get; set; }
        public DbSet<SAPEPUserInfoHistory> SAPEPUserInfoHistorys { get; set; }
        public DbSet<SAPRoleToTCodes> SAPRoleToTCodes { get; set; }
        #endregion

        #region "SAP FLINS"
        public DbSet<ProvisioningConfigMatrixSapEEGroup> ProvisioningConfigMatrixSapEEGroup { get; set; }
        public DbSet<ProvisioningConfigMatrixSAPOrg> ProvisioningConfigMatrixSAPOrg { get; set; }
        public DbSet<ProvisioningConfigMatrixSAPPosition> ProvisioningConfigMatrixSAPPosition { get; set; }
        public DbSet<SAPBIRoleCategory> SAPBIRoleCategory { get; set; }
        public DbSet<SAPHRFLINSConflictingRole> SAPHRFLINSConflictingRole { get; set; }
        public DbSet<SAPHRFLINSRoleMatrix> SAPHRFLINSRoleMatrix { get; set; }
        public DbSet<SAPHRFLINSSuppRoleMatrix> SAPHRFLINSSuppRoleMatrix { get; set; }
        public DbSet<SAPRoleMandatoryField> SAPRoleMandatoryField { get; set; }
        public DbSet<SAPRoleMatrix> SAPRoleMatrix { get; set; }

        #endregion "SAP FLINS"

        #region SAP GRC

        public DbSet<SAPGRCRole> SAPGRCRole { get; set; }

        #endregion

        #region SAP MSSL
        public DbSet<SAPXLMSSLCriticalTCodeMatrix> SAPXLMSSLCriticalTCodeMatrices { get; set; }
        public DbSet<SAPXLMSSLPositionsMatrix> SAPXLMSSLPositionsMatrices { get; set; }
        public DbSet<SAPXLMSSLRolesMatrix> SAPXLMSSLRolesMatrices { get; set; }

        #endregion

        #region SAP EBS
        public DbSet<SAPEBSXLCriticalTCodesMatrix> SAPEBSXLCriticalTCodeMatrices { get; set; }
        public DbSet<SAPEBSXLPositionsMatrix> SAPEBSXLPositionsMatrices { get; set; }
        public DbSet<SAPEBSXLRoleMatrix> SAPEBSXLRolesMatrices { get; set; }

        #endregion

        #region SAP SRM
        public DbSet<SAPSRMMainRoleMatrix> SAPSRMMainRoleMatrices { get; set; }
        public DbSet<SAPSRMEnablerRoleMatrix> SAPSRMEnablerRoleMatrices { get; set; }
        public DbSet<SAPSRMBusinessRoleMapping> SAPSRMBusinessRoleMappings { get; set; }
        #endregion

        #region O365
        public DbSet<Office365LicenseConfig> Office365LicenseConfigs { get; set; }
        #endregion
        public DbSet<ADSXLProvisioningMatrix> ADSXLProvisioningMatrices { get; set; }

        public DbSet<ApproverException> ApproverException { get; set; }

        #region "SAPEP"
        public DbSet<SAPEPUserInfo> SAPEPUserInfos { get; set; }
        public DbSet<SAPEPGroupInfo> SAPEPGroupInfos { get; set; }
        public DbSet<SAPEPRoleInfo> SAPEPRoleInfos { get; set; }
        #endregion

        #region "Inhouse Systems"
        public DbSet<InhouseUserInfo> InhouseUserInfos { get; set; }
        public DbSet<InhousePermission> InhousePermissions { get; set; }
        public DbSet<InhouseFunction> InhouseFunctions { get; set; }
        public DbSet<InhUserPermissionFunction> InhUserPermissionFunctions { get; set; }
        #endregion


        #region Endpoint
        public DbSet<EndPoint> EndPoint { get; set; }
        public DbSet<EndPointSystemType> EndpointSystemTypes { get; set; }
        public DbSet<EndpointObjects> EndpointObjects { get; set; }
        public DbSet<AttributeMappingColumns> AttributeMappingColumnss { get; set; }
        public DbSet<EndPointAutoExpiry> EndPointAutoExpirys { get; set; }
        #endregion

        #region Complaince

        public DbSet<ComplianceProcess> ComplianceProcess { get; set; }

        #endregion

        #region Scheduler

        public DbSet<Process> Process { get; set; }
        public DbSet<Scheduler> Scheduler { get; set; }
        public DbSet<ProcessHistory> ProcessHistory { get; set; }

        #endregion

        #region  Dashboard

        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<DashboardAccessRights> DashboardAccessRights { get; set; }
        public DbSet<DefaultRoleDashboardPreference> DefaultRoleDashboardPreferences { get; set; }
        public DbSet<UserDashboardPreference> UserDashboardPreferences { get; set; }


        #endregion

        #region Attestation
        public DbSet<Attestation> Attestations { get; set; }
        public DbSet<AttestationScope> AttestationScopes { get; set; }
        public DbSet<AttestationEndpoint> AttestationEndpoints { get; set; }
        public DbSet<AttestationProcess> AttestationProcesses { get; set; }
        public DbSet<AttestationProcessHistory> AttestationProcessHistories { get; set; }
        public DbSet<AttestationCampaign> AttestationCampaigns { get; set; }
        public DbSet<AttestationCampaignAttachment> AttestationCampaignAttachments { get; set; }
        public DbSet<AttestationCampaignReport> AttestationCampaignReports { get; set; }
        #endregion

        #region GridSettings
        public DbSet<UserGridSettings> UserGridSettings { get; set; }
        public DbSet<UserGridFilterSettings> UserGridFilterSettings { get; set; }
        #endregion

        #region Attestation Revamp
        public DbSet<AttestationHierarchy> AttestationHierarchies { get; set; }
        public DbSet<AttestationSnapshot> AttestationSnapshots { get; set; }
        public DbSet<AttestationSnapshotAction> AttestationSnapshotActions { get; set; }
        public DbSet<AttestationSnapshotActionInterim> AttestationSnapshotActionInterims { get; set; }
        public DbSet<AttestationSnapshotTaskStatus> AttestationSnapshotTaskStatus { get; set; }
        public DbSet<AttestationSnapshotActionHistory> AttestationSnapshotActionHistories { get; set; }
        public DbSet<EndpointHierarchy> EndpointHierarchies { get; set; }
        #endregion

        #region NOrg Revamp
        public DbSet<NDesignation> NDesignations { get; set; }
        public DbSet<NCostCenter> NCostCenters { get; set; }
        public DbSet<NOrganisation> NOrganisations { get; set; }

        public DbSet<NOrganisationHead> NOrganisationHeads { get; set; }
        public DbSet<NOrganisationScope> NOrganisationScopes { get; set; }
        public DbSet<NRulesetType> NRulesetTypes { get; set; }
        public DbSet<NScopeFor> NScopeFor { get; set; }
        public DbSet<NProvisioningField> NProvisioningFields { get; set; }
        public DbSet<NOrganisationRelationship> NOrganisationRelationships { get; set; }
        public DbSet<NRuleset> NRulesets { get; set; }
        public DbSet<NRulesetForApplication> NRulesetForApplications { get; set; }
        public DbSet<NUserOrganisation> NUserOrganisations { get; set; }
        public DbSet<NUserDesignation> NUserDesignations { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<PersonnelArea> PersonnelAreas { get; set; }
        public DbSet<PersonnelSubArea> PersonnelSubAreas { get; set; }

        #endregion

        public DbSet<ApproverDelegationHistory> ApproverDelegationHistory { get; set; }

        public DbSet<MailNotificationSettings> MailNotificationSettings { get; set; }

        public override int SaveChanges()
        {
            string userName = HttpContext.Current == null
                      ? "System"
                      : HttpContext.Current.User.Identity.Name;

            return base.SaveChanges(userName);
        }

        #region Data Audit

        public class Auditing
        {
            public void TrackProperties<T>(List<string> disableProperties)
            {
                EntityTracker.TrackAllProperties<T>();

                foreach (string disableProperty in disableProperties)
                {
                    Expression<Func<T, object>> property = GetPropertySelector<T>(disableProperty);

                    EntityTracker.OverrideTracking<T>().Disable(property);
                }
            }

            public static Expression<Func<T, object>> GetPropertySelector<T>(string propertyName)
            {
                var arg = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(arg, propertyName);
                //return the property as object
                var conv = Expression.Convert(property, typeof(object));
                var exp = Expression.Lambda<Func<T, object>>(conv, new ParameterExpression[] { arg });
                return exp;
            }
        }

        #endregion

        #region Reporting  Schedule

        public DbSet<Report> Report { get; set; }
        public DbSet<ReportAttributes> ReportAttributes { get; set; }
        public DbSet<ReportHistory> ReportHistory { get; set; }
        public DbSet<ServiceAccountQuestion> ServiceAccountQuestions { get; set; }


        #endregion

        public DbSet<EventUndoRequest> EventUndoRequests { get; set; }

        #region Bulk Request

        public DbSet<BulkRequest> BulkRequests { get; set; }
        public DbSet<BulkRequestDetail> BulkRequestDetails { get; set; }

        #endregion

        #region O365
        public DbSet<Office365UserInfo> Office365UserInfo { get; set; }
        public DbSet<Office365GroupInfo> Office365GroupInfo { get; set; }
        public DbSet<Office365License> Office365License { get; set; }
        public DbSet<Office365MassLicense> Office365MassLicense { get; set; }
        public DbSet<Office365RetentionPolicy> Office365RetentionPolicy { get; set; }
        public DbSet<Office365RoleAssignmentPolicy> Office365RoleAssignmentPolicy { get; set; }
        public DbSet<Office365ServicePlan> Office365ServicePlan { get; set; }
        public DbSet<Office365SharingPolicy> Office365SharingPolicy { get; set; }
        #endregion

        #region SMS
        public DbSet<SMSUserInfo> SMSUserInfo { get; set; }
        public DbSet<SMSCompany> SMSCompany { get; set; }
        public DbSet<SMSBadgeInfo> SMSBadgeInfo { get; set; }
        public DbSet<SMSAccessLevel> SMSAccessLevels { get; set; }
        public DbSet<SMSXLProvisioningMatrix> SMSXLProvisioningMatrices { get; set; }
        public DbSet<SMSXLRoleCatalogue> SMSXLRoleCatalogues { get; set; }

        #endregion

        #region GEMS
        public DbSet<GEMSRoleMatrix> GEMSRoleMatrix { get; set; }
        public DbSet<GEMSRoleInfo> GEMSRoleInfo { get; set; }
        public DbSet<GEMSUserInfo> GEMSUserInfo { get; set; }
        public DbSet<UserBusinessRoleMapping> UserBusinessRoleMapping { get; set; }
        #endregion

        #region NPAC
        public DbSet<NPACDesignationInfo> NPACDesignationInfo { get; set; }
        public DbSet<NPACLevelInfo> NPACLevelInfo { get; set; }
        public DbSet<NPACSectionInfo> NPACSectionInfo { get; set; }
        public DbSet<NPACUserInfo> NPACUserInfo { get; set; }
        #endregion

        #region SSIDS     
        public DbSet<SSIDSRolelnfo> SSIDSRolelnfo { get; set; }
        public DbSet<SSIDSUserInfo> SSIDSUserInfo { get; set; }
        #endregion
        #region SAP Others
        public DbSet<SAPPIRole> SAPPIRoles { get; set; }
        #endregion

        #region NOVA
        public DbSet<NOVAUserRoleMatrix> NOVAUserRoleMatrices { get; set; }
        public DbSet<NOVAManagementRoleMatrix> NOVAManagementRoleMatrices { get; set; }
        public DbSet<NOVAExceptionalRoleMatrix> NOVAExceptionalRoleMatrices { get; set; }
        public DbSet<NOVASoDExceptionsMatrix> NOVASoDExceptionsMatrices { get; set; }

        public DbSet<SAPSectionMappingInfo> SAPSectionMappingInfo { get; set; } 
        public DbSet<SAPEamNovaUserRoleInfo> SAPEamNovaUserRoleInfo { get; set; }

        public DbSet<SAPEamNovaUserInfo> SAPEamNovaUserInfo { get; set; }

        public DbSet<NOVACompositeRoleMatrix> NOVACompositeRoleMatrices { get; set; }
        #endregion

        #region MISC
        public DbSet<ApproverExceptionInfo> ApproverExceptionInfos { get; set; }
        #endregion

        #region CustomReport

        public DbSet<CustomReport> CustomReport { get; set; }
        public DbSet<CustomReportParameter> CustomReportParameter { get; set; }

        #endregion
        #region VPN
        public DbSet<VPNUserInfo> VPNUserInfo { get; set; }
        public DbSet<VPNMatrix> VPNMatrix { get; set; }
        public DbSet<VPNApplicationEnvironment> VPNApplicationEnvironment { get; set; }
        public DbSet<VPNUserApplicationEnviornment> VPNUserApplicationEnviornment { get; set; }
        #endregion
        #region Skype for business
        public DbSet<SkypeForBusinessUserInfo> SkypeForBusinessUserInfo { get; set; }
        public DbSet<SkypeForBusinessResponseGroupInfo> SkypeForBusinessResponseGroup { get; set; }
        public DbSet<SkypeForBusinessUnAssignedNumberMatrix> SkypeForBusinessUnAssignedNumberMatrix { get; set; }
        public DbSet<SkypeForBusinessUnAssignedNumber> SkypeForBusinessUnAssignedNumber { get; set; }
        public DbSet<SkypeForBusinessProvisioningMatrix> SkypeForBusinessProvisioningMatrix { get; set; }
        
        #endregion

        public DbSet<UserTableInfo> UserTableInfos { get; set; }
        public DbSet<SRMHangingRoleInfo> SRMHangingRoleInfo { get; set; }

        public DbSet<EAMNovaHangingRoleInfo> EAMNovaHangingRoleInfo { get; set; }

        #region PUB

        public DbSet<PUBFile> PUBFile { get; set; }

        public DbSet<PUBEmployees> PUBEmployees { get; set; }

        #endregion 

        public DbSet<TaskReminder> TaskReminder { get; set; }
        #region "SAP SOD Confilct Role"
        public DbSet<SAPSODConflictingRole> SAPSODConfilictingRole { get; set; }

        public DbSet<SAPCriticalTcodeReport> SAPCriticalTcodeReport { get; set; }

        public DbSet<SAPConflictingRoleReport> SAPConflictingRoleReport { get; set; }
        #endregion

        public DbSet<AccountEntitlementAuditTrial> AccountEntitlementAuditTrials { get; set; }
        public DbSet<AttestationTaskEntitlementUserComparision> AttestationTaskEntitlementUserComparisions { get; set; }
        public DbSet<AttestationTaskHistoryEntitlementUserComparision> AttestationTaskHistoryEntitlementUserComparisions { get; set; }

        #region SAPHANA
        public DbSet<SAPHANAUserinfo> SAPHANAUserinfos { get; set; }
        public DbSet<SAPHANAAuthorizationProfile> SAPHANAAuthorizationProfile { get; set; }
        public DbSet<SAPHANAAuthorizationToObject> SAPHANAAuthorizationToObject { get; set; }
        public DbSet<SAPHANACompositeProfileInfo> SAPHANACompositeProfileInfo { get; set; }
        public DbSet<SAPHANACompositeRoleInfo> SAPHANACompositeRoleInfo { get; set; }
        public DbSet<SAPHANASingleRoleInfo> SAPHANASingleRoleInfo { get; set; }
        public DbSet<SAPHANATCodeInfo> SAPHANATCodeInfo { get; set; }
        public DbSet<SAPHANAObjectInfo> SAPHANAObjectInfo { get; set; }
        public DbSet<SAPHANAObjectValues> SAPHANAObjectValues { get; set; }
        public DbSet<SAPHANAParameterHistoryInfo> SAPHANAParameterHistoryInfo { get; set; }
        public DbSet<SAPHANAParameterInfo> SAPHANAParameterInfo { get; set; }
        public DbSet<SAPHANAPosition> SAPHANAPosition { get; set; }
        public DbSet<SAPHANAProfileInfo> SAPHANAProfileInfo { get; set; }
        public DbSet<SAPHANAFieldValueMapping> SAPHANAFieldValueMap { get; set; }
        public DbSet<SAPHANAGroupInfo> SAPHANAGroupInfos { get; set; }
        public DbSet<SAPHANAHRP1001> SAPHANAHRP1001Entries { get; set; }
        public DbSet<SAPHANARoleInfo> SAPHANARoleInfos { get; set; }
        public DbSet<SAPHANARoleTCodeMapping> SAPHANARoleTCodeMap { get; set; }
        public DbSet<SAPHANARoleToAuthMap> SAPHANARoleToAuthMap { get; set; }
        public DbSet<SapHANAUserProfile> SAPHANAUserProfile { get; set; }
        public DbSet<SAPHANARoleToTCodes> SAPHANARoleToTCodes { get; set; }

        public DbSet<SAPHANAXLCriticalTCodeMatrix> SAPHANACriticalTCodeMatrices { get; set; }
        public DbSet<SAPHANAXLPositionsMatrix> SAPHANAXLPositionsMatrices { get; set; }
        public DbSet<SAPHANAXLRolesMatrix> SAPHANAXLRolesMatrices { get; set; }
        public DbSet<SAPHANACriticalTcodeReport> SAPHANACriticalTcodeReport { get; set; }
        public DbSet<SAPHANAConflictingRoleReport> SAPHANAConflictingRoleReport { get; set; }
        public DbSet<SAPHANARoleProfileMapping> SAPHANARoleProfileMappings { get; set; }

        public DbSet<SAPHANATCodeObjectMap> SAPHANATCodeObjectMap { get; set; }
        public DbSet<SAPHANAGroupHistoryInfo> SAPHANAGroupHistoryInfo { get; set; }
        public DbSet<SAPHANASectionMappingInfo> SAPHANASectionMappingInfo { get; set; }
        public DbSet<SAPHANAUserHistoryInfo> SAPHANAUserHistoryInfo { get; set; }

        #endregion
    }
}
