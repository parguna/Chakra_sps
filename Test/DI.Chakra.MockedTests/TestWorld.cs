using System;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// Creates the rows a test needs (its own endpoint, discovery job, ...) with a unique MOCKTEST name,
    /// installs the mock connector, and on Dispose removes every row tied to what it created plus any
    /// emails queued during the test. Nothing outside these rows is modified.
    /// </summary>
    public sealed class TestWorld : IDisposable
    {
        /// <summary>Copied from MV90 (an existing In-house system) so the endpoint is realistic.</summary>
        private const long TemplateInhouseEndpointId = 26;

        public string Tag { get; } = "MOCKTEST_" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
        public long EndpointId { get; private set; }
        public long DiscoveryProcessId { get; private set; }
        public long SchedulerId { get; private set; }
        public long ProcessId { get; private set; }

        private readonly long emailIdBefore;
        private readonly DateTime startedAt = DateTime.Now;

        private static readonly object bootLock = new object();
        private static bool booted;

        /// <summary>AutoMapper's static configuration must be set up exactly once per process, as the services do at startup.</summary>
        public static void EnsureBootstrapped()
        {
            lock (bootLock)
            {
                if (booted) return;
                // Same start-up mappings as the Provisioning Windows service
                // (Services\Win\DI.Chakra.Services.Provisioning\Program.cs, Main) — the O365 executor depends on them.
                DomainModel.AutoMapperHelper.BootStrap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365LicenseConfig, Dto.Office365LicenseConfig>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365GroupInfo, Dto.Office365GroupInfo>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365License, Dto.Office365License>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365MassLicense, Dto.Office365MassLicense>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365RetentionPolicy, Dto.Office365RetentionPolicy>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365RoleAssignmentPolicy, Dto.Office365RoleAssignmentPolicy>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365ServicePlan, Dto.Office365ServicePlan>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365SharingPolicy, Dto.Office365SharingPolicy>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.Office365UserInfo, Dto.Office365UserInfo>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.ServicePlanDetail, Dto.ServicePlanDetail>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.AdHocLicenseInfo, Dto.AdHocLicenseInfo>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365SharedAccount, Dto.AdHocO365SharedAccount>().ReverseMap();
                AutoMapper.Mapper.CreateMap<DomainModel.AdHocO365Request, Dto.AdHocO365Request>().ReverseMap();
                booted = true;
            }
        }

        public TestWorld()
        {
            EnsureBootstrapped();
            emailIdBefore = TestDb.ScalarLong("SELECT ISNULL(MAX(Id), 0) FROM DI_M_Email");
            MockConnector.Reset();
            MockConnector.Install();
        }

        /// <summary>A new, active In-house endpoint owned by this test.</summary>
        public long CreateInhouseEndpoint(bool canAutomate = false)
        {
            EndpointId = TestDb.ScalarLong(@"
INSERT INTO DI_M_EndpointSystem (Description, EndpointSystemTypeId, DisplayName, OwnerUserId, ApplicationCode, IsActive, IsTrustedSource,
    IsPasswordResetAllowed, IncludeInAttestation, CreatedBy, CreatedDate, UserObjectName, ApplicationType, IsSodApplicable,
    SupportSecondaryAccount, IsFunctionLevel, CanAutomate, IsUnlockAllowed)
OUTPUT INSERTED.Id
SELECT @tag, EndpointSystemTypeId, @tag, OwnerUserId, @tag, 1, 0,
    IsPasswordResetAllowed, 0, 'mocktest', GETDATE(), UserObjectName, ApplicationType, 0,
    SupportSecondaryAccount, IsFunctionLevel, @canAutomate, IsUnlockAllowed
FROM DI_M_EndpointSystem WHERE Id = @template",
                ("@tag", Tag), ("@canAutomate", canAutomate), ("@template", TemplateInhouseEndpointId));
            if (EndpointId == 0) throw new InvalidOperationException("Template In-house endpoint not found.");
            return EndpointId;
        }

        /// <summary>A discovery job for the test endpoint, in the same shape the scheduler UI creates.</summary>
        public long CreateDiscoveryJob()
        {
            if (EndpointId == 0) CreateInhouseEndpoint();
            DiscoveryProcessId = TestDb.ScalarLong(@"
INSERT INTO DI_M_DiscoveryProcess (ProcessName, Description, Endpointsystemid, [Default], CreatedBy, CreatedDate, DisableDeltaDiscovery)
OUTPUT INSERTED.Id VALUES (@tag, @tag, @ep, 0, 'mocktest', GETDATE(), 0)", ("@tag", Tag), ("@ep", EndpointId));

            // Far-future one-time schedule: a running scheduler would never pick it up; the tests start it directly.
            SchedulerId = TestDb.ScalarLong(@"
INSERT INTO DI_M_Scheduler (ProcessName, StartDate, StartTime, Occurence, CreatedBy, CreatedDate)
OUTPUT INSERTED.Id VALUES (@tag, '2099-01-01', '00:00', @occ, 'mocktest', GETDATE())", ("@tag", Tag), ("@occ", (int)Occurence.OneTime));

            ProcessId = TestDb.ScalarLong(@"
INSERT INTO DI_M_Process (ProcessType, ProcessTypeKeyId, Status, CreatedBy, CreatedDate, Scheduler_SchedulerId, DiscoveryInstance)
OUTPUT INSERTED.Id VALUES (@type, @key, 'Pending', 'mocktest', GETDATE(), @sch, 1)",
                ("@type", (int)ProcessType.Discovery), ("@key", DiscoveryProcessId), ("@sch", SchedulerId));
            return ProcessId;
        }

        // ------------------------------------------------------------------ provisioning

        /// <summary>Real MV90 rows used as templates for valid request rows (then re-pointed at the test endpoint).</summary>
        private const long TemplateAccessRequestId = 510487;
        private const long TemplateAdHocRequestId = 40821;
        private const long TemplatePermissionId = 1661;

        /// <summary>An existing active employee. Accounts are only ever created on the test endpoint.</summary>
        public const long TestUserId = 2;
        public const long RequesterUserId = 1;

        private readonly List<long> accessRequestIds = new List<long>();
        private readonly List<long> adHocRequestIds = new List<long>();

        /// <summary>A permission on the test endpoint (copied from an MV90 permission).</summary>
        public string CreatePermission()
        {
            if (EndpointId == 0) CreateInhouseEndpoint();
            string name = Tag + "_PERM";
            TestDb.CloneAndUpdate("DI_INH_Permission", TemplatePermissionId,
                "UPDATE DI_INH_Permission SET EndPointSystemId = @ep, EndPoint_Id = NULL, InhouseSystemCode = @tag, Name = @name, InhouseUserInfo_Id = NULL, ADGroups = NULL, EPGroups = NULL, EPRoles = NULL, CEGroups = NULL, CERoles = NULL, CreatedBy = 'mocktest' WHERE Id = @id",
                ("@ep", EndpointId), ("@tag", Tag), ("@name", name));
            return name;
        }

        /// <summary>
        /// A request already approved and ready to execute (RequestStatus=Granted, ProgressStatus=ReadytoExecute),
        /// as it would be after the workflow finished, for the test user on the test endpoint.
        /// </summary>
        public long CreateRequest(AccessRequestSource source, AdHocActionType action, long endpointUserId, string entitlement, bool isOffBoardRequest = false)
        {
            if (EndpointId == 0) CreateInhouseEndpoint();

            string detail =
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
                "<AdHocINHRequest xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n" +
                $"  <DisplayAccountName>{System.Security.SecurityElement.Escape(Tag)}</DisplayAccountName>\r\n" +
                $"  <EndpointUserId>{endpointUserId}</EndpointUserId>\r\n" +
                $"  <Entitlement>{System.Security.SecurityElement.Escape(entitlement ?? string.Empty)}</Entitlement>\r\n" +
                "  <EntitlementType>Permission</EntitlementType>\r\n" +
                $"  <IsOffBoardRequest>{(isOffBoardRequest ? "true" : "false")}</IsOffBoardRequest>\r\n" +
                "</AdHocINHRequest>";

            return CreateRequestCore(EndpointId, TestUserId, source, action, detail, TemplateAccessRequestId, TemplateAdHocRequestId);
        }

        private long CreateRequestCore(long endpointId, long userId, AccessRequestSource source, AdHocActionType action, string detail,
            long templateAccessRequestId, long templateAdHocRequestId)
        {
            long adHocId = TestDb.CloneAndUpdate("DI_M_AdHocAccessRequest", templateAdHocRequestId,
                @"UPDATE DI_M_AdHocAccessRequest SET RequesterUserId = @req, RequesteeUserId = @user, EndpointSystemId = @ep, AdhocActionType = @action,
    AccountType = @acct, EntitlementDetail = @detail, ValidFrom = GETDATE(), ValidTo = NULL, Comments = @tag, CreatedBy = 'mocktest', CreatedDate = GETDATE(),
    LastModifiedBy = NULL, LastModifiedDate = NULL WHERE Id = @id",
                ("@req", RequesterUserId), ("@user", userId), ("@ep", endpointId), ("@action", (int)action), ("@acct", (int)AccountType.User),
                ("@detail", detail), ("@tag", Tag));
            adHocRequestIds.Add(adHocId);

            long requestId = TestDb.CloneAndUpdate("DI_M_AccessRequest", templateAccessRequestId,
                @"UPDATE DI_M_AccessRequest SET RequesterId = @req, UserId = @user, Description = @tag, AdHocAccessRequestId = @adhoc, EndpointSystemId = @ep,
    EmployeeEventTriggerId = NULL, RequestStatus = @granted, ProgressStatus = @ready, RequestSource = @source, ActivationDate = GETDATE(), DeactivationDate = NULL,
    ExecutionTime = DATEADD(minute, -1, GETDATE()), Remarks = NULL, CreatedBy = 'mocktest', CreatedDate = GETDATE(), LastModifiedBy = NULL, LastModifiedDate = NULL,
    TransactionId = NEWID(), ParentRequestId = NULL, IsParentRequest = 0, IsDependentRequest = 0, IsDependentRequestCreated = 0, AccountType = @acct,
    AttestationCampaignId = NULL, BulkRequestId = NULL, EventUndoRequestId = NULL WHERE Id = @id",
                ("@req", RequesterUserId), ("@user", userId), ("@tag", Tag), ("@adhoc", adHocId), ("@ep", endpointId),
                ("@granted", (int)AccessRequestStatus.Granted), ("@ready", (int)AccessRequestProgress.ReadytoExecute), ("@source", (long)source),
                ("@acct", (int)AccountType.User));
            accessRequestIds.Add(requestId);
            return requestId;
        }

        public AccessRequestProgress RequestProgress(long requestId) =>
            (AccessRequestProgress)TestDb.ScalarLong("SELECT ProgressStatus FROM DI_M_AccessRequest WHERE Id = @id", ("@id", requestId));

        /// <summary>The test user's In-house account on the test endpoint (0 when none).</summary>
        public long AccountId() =>
            TestDb.ScalarLong("SELECT TOP 1 Id FROM DI_INH_UserInfo WHERE EndPointSystemId = @ep ORDER BY Id DESC", ("@ep", EndpointId));

        // ------------------------------------------------------------------ Active Directory

        /// <summary>The real AD endpoint (ApplicationType ADSingaporePowerLocal). Only the mock connector ever talks for it.</summary>
        public const long AdEndpointId = 3;
        /// <summary>A completed real AD ad hoc new-account request, its ad hoc row, and that person (template for the synthetic employee).</summary>
        private const long TemplateAdAccessRequestId = 779955;
        private const long TemplateAdAdHocRequestId = 191369;
        private const long TemplateAdUserId = 28063;
        private const long TemplateIacmUserInfoId = 16086;

        /// <summary>A synthetic employee owned by this test (copy of a real person's org data, new id/employee id/name).</summary>
        public long SyntheticUserId { get; private set; }
        public string SyntheticEmployeeId { get; private set; }

        /// <summary>Test-owned org codes (unique per test) so only this test's matrix rows can ever match the synthetic employee.</summary>
        public string OrgCompany { get; private set; }
        public string OrgCode { get; private set; }
        public string OrgDivision { get; private set; }
        public string OrgDepartment { get; private set; }
        public string OrgCostCenter { get; private set; }
        /// <summary>OU of the test's AD provisioning-matrix row for the current org codes.</summary>
        public string MatrixOu { get; private set; }
        public string MatrixDefaultGroups { get; private set; }

        private readonly List<string> orgCompanies = new List<string>();
        private readonly List<string> flinsBusinessRoles = new List<string>();

        /// <summary>Real AD provisioning-matrix row copied for the test org (real OU format and real default groups).</summary>
        private const long TemplateAdMatrixId = 308;

        public long CreateSyntheticEmployee() => CreateSyntheticEmployee(fullTime: false);

        /// <param name="fullTime">Full-time employees get their AD OU from the provisioning matrix (needed for mover tests);
        /// non-employees always go to the non-employee OU.</param>
        public long CreateSyntheticEmployee(bool fullTime)
        {
            SyntheticEmployeeId = "MT" + Tag.Substring(Tag.Length - 6);
            SetTestOrg(1);
            SyntheticUserId = TestDb.CloneAndUpdate("DI_M_User", TemplateAdUserId,
                @"UPDATE DI_M_User SET EmployeeId = @emp, UserName = @emp, DisplayName = 'Mocktest User', KnownAs = 'Mocktest User', FirstName = 'Mocktest',
    MiddleName = NULL, LastName = @tag, EmailId = NULL, TerminationDate = NULL, IsActive = 1, NeedEmailUpdateToHR = 0, ROEmployeeId = NULL, COEmployeeId = NULL,
    FullTimeEmployee = @ft, CompanyCode = @co, OrganisationCode = @org, BranchCode = @div, DepartmentCode = @dept, CostCenter = @cc,
    CreatedBy = 'mocktest', CreatedDate = GETDATE(), LastModifiedBy = NULL, LastModifiedDate = NULL WHERE Id = @id",
                ("@emp", SyntheticEmployeeId), ("@tag", Tag), ("@ft", fullTime),
                ("@co", OrgCompany), ("@org", OrgCode), ("@div", OrgDivision), ("@dept", OrgDepartment), ("@cc", OrgCostCenter));

            // The AD executor updates the person's IDMS account by employee id, so give the synthetic employee one.
            TestDb.CloneAndUpdate("DI_IACM_UserInfo", TemplateIacmUserInfoId,
                "UPDATE DI_IACM_UserInfo SET EmployeeId = @emp, UserName = @emp, CreatedBy = 'mocktest', CreatedDate = GETDATE() WHERE Id = @id",
                ("@emp", SyntheticEmployeeId));

            // A brand-new id must not already be referenced anywhere; otherwise cleanup by user id would not be safe.
            long preexisting = TestDb.ScalarLong(@"
DECLARE @n bigint = 0, @sql nvarchar(max) = N'';
SELECT @sql = @sql + N'SELECT @n = @n + COUNT(*) FROM ' + QUOTENAME(t.name) + N' WHERE ' + QUOTENAME(c.name) + N' = @u;'
FROM sys.columns c JOIN sys.tables t ON t.object_id = c.object_id
WHERE c.name IN ('UserId','User_Id','RequesteeUserId','WogUserId','DIUserId') AND TYPE_NAME(c.user_type_id) IN ('bigint','int') AND t.name <> 'DI_M_User';
EXEC sp_executesql @sql, N'@u bigint, @n bigint OUTPUT', @u = @u, @n = @n OUTPUT;
SELECT @n;", ("@u", SyntheticUserId));
            if (preexisting != 0)
                throw new InvalidOperationException($"New user id {SyntheticUserId} is already referenced by {preexisting} rows; refusing to run.");
            return SyntheticUserId;
        }

        /// <summary>
        /// Picks test-owned org codes (variant 1, 2, ...) and seeds the AD and SAP org matrices for them.
        /// The company code is unique per test, so the SAP matrix's LIKE-prefix matching can only hit these rows.
        /// </summary>
        private void SetTestOrg(int variant)
        {
            string suffix = Tag.Substring(Tag.Length - 4);
            OrgCompany = "M" + suffix.Substring(0, 3);                 // e.g. "MA1B" — real company codes are "SP", "SPAC", ...
            OrgCode = "9" + Math.Abs((Tag + variant).GetHashCode() % 10000000).ToString("D7");
            OrgDivision = "9" + Math.Abs((Tag + "d" + variant).GetHashCode() % 10000000).ToString("D7");
            OrgDepartment = "9" + Math.Abs((Tag + "p" + variant).GetHashCode() % 10000000).ToString("D7");
            OrgCostCenter = OrgCompany + "CC" + variant;
            if (!orgCompanies.Contains(OrgCompany)) orgCompanies.Add(OrgCompany);

            MatrixOu = $"OU=MOCKTEST {Tag} {variant},OU=SP,DC=singaporepower,DC=local";
            TestDb.CloneAndUpdate("DI_ADS_XLProvMatrix", TemplateAdMatrixId,
                "UPDATE DI_ADS_XLProvMatrix SET OrganisationId = @org, DivisionCode = @div, CompanyCode = @co, OU = @ou, CreatedBy = 'mocktest', CreatedDate = GETDATE() WHERE Id = @id",
                ("@org", OrgCode), ("@div", OrgDivision), ("@co", OrgCompany), ("@ou", MatrixOu));
            MatrixDefaultGroups = TestDb.Scalar("SELECT DefaultGroups FROM DI_ADS_XLProvMatrix WHERE Id = @id", ("@id", TemplateAdMatrixId)) as string;

            long sapOrgTemplate = TestDb.ScalarLong("SELECT TOP 1 Id FROM DI_M_ProvisioningConfigMatrixSAPORG ORDER BY Id");
            TestDb.CloneAndUpdate("DI_M_ProvisioningConfigMatrixSAPORG", sapOrgTemplate,
                @"UPDATE DI_M_ProvisioningConfigMatrixSAPORG SET CompanyCode = @co, DepartmentCode = @dept, DivisionCode = @div, CostCenterCode = @cc,
    DefaultHRMSGeneralRoles = 'Y_SAP_ESSUSER_GENERAL', CreatedBy = 'mocktest', CreatedDate = GETDATE() WHERE Id = @id",
                ("@co", OrgCompany), ("@dept", OrgDepartment), ("@div", OrgDivision), ("@cc", OrgCostCenter));
        }

        /// <summary>Moves the synthetic employee to a second test org (new matrix rows with a different OU), as an HR transfer would.</summary>
        public string MoveSyntheticEmployeeToNewOrg()
        {
            SetTestOrg(2);
            TestDb.Execute("UPDATE DI_M_User SET OrganisationCode = @org, BranchCode = @div, DepartmentCode = @dept, CostCenter = @cc, CompanyCode = @co WHERE Id = @u AND EmployeeId = @emp",
                ("@org", OrgCode), ("@div", OrgDivision), ("@dept", OrgDepartment), ("@cc", OrgCostCenter), ("@co", OrgCompany),
                ("@u", SyntheticUserId), ("@emp", SyntheticEmployeeId));
            return MatrixOu;
        }

        /// <summary>An HR event for the synthetic employee, as the HR sync writes it (request already created, no admin check).</summary>
        public long CreateEventTrigger(EmployeeEvents eventType)
        {
            return TestDb.ScalarLong(@"
INSERT INTO DI_M_EmployeeEventTrigger (UserId, EventType, EffectiveDate, IsRequestCreated, IsAdminCheckRequired, IsActive, CreatedBy, CreatedDate, IsMailSent, RequestStatus, Comments, EventSequence)
OUTPUT INSERTED.Id VALUES (@u, @type, CAST(GETDATE() AS date), 1, 0, 1, 'mocktest', GETDATE(), 0, 0, @tag, 1)",
                ("@u", SyntheticUserId), ("@type", (int)eventType), ("@tag", Tag));
        }

        /// <summary>A completed real AD offboarding request (trigger-based, no ad hoc row): template for lifecycle requests.</summary>
        private const long TemplateLifecycleRequestId = 779845;

        /// <summary>
        /// A lifecycle (HR event) request in the shape ProvisioningFlowHelper creates: Granted / ReadytoExecute,
        /// linked to the trigger, no ad hoc record; offboarding requests carry the effective date as DeactivationDate.
        /// </summary>
        public long CreateLifecycleRequest(long endpointId, AccessRequestSource source, long triggerId, int applicationType)
        {
            long requestId = TestDb.CloneAndUpdate("DI_M_AccessRequest", TemplateLifecycleRequestId,
                @"UPDATE DI_M_AccessRequest SET RequesterId = @req, UserId = @user, Description = @tag, AdHocAccessRequestId = NULL, EndpointSystemId = @ep,
    EmployeeEventTriggerId = @trig, RequestStatus = @granted, ProgressStatus = @ready, RequestSource = @source, ActivationDate = GETDATE(),
    DeactivationDate = CASE WHEN @source = 2 THEN CAST(GETDATE() AS date) ELSE NULL END,
    ExecutionTime = DATEADD(minute, -1, GETDATE()), Remarks = NULL, CreatedBy = 'mocktest', CreatedDate = GETDATE(), LastModifiedBy = NULL, LastModifiedDate = NULL,
    TransactionId = NEWID(), ParentRequestId = NULL, IsParentRequest = 0, IsDependentRequest = 0, IsDependentRequestCreated = 0, AccountType = @acct,
    ApplicationType = @app, AttestationCampaignId = NULL, BulkRequestId = NULL, EventUndoRequestId = NULL, AdditionalInfo = NULL WHERE Id = @id",
                ("@req", RequesterUserId), ("@user", SyntheticUserId), ("@tag", Tag), ("@ep", endpointId), ("@trig", triggerId),
                ("@granted", (int)AccessRequestStatus.Granted), ("@ready", (int)AccessRequestProgress.ReadytoExecute), ("@source", (long)source),
                ("@acct", (int)AccountType.User), ("@app", applicationType));
            accessRequestIds.Add(requestId);
            return requestId;
        }

        // ------------------------------------------------------------------ SAP (HRMS/FLINS)

        public const long FlinsEndpointId = 5;
        private const long TemplateFlinsAccessRequestId = 779506;
        private const long TemplateFlinsAdHocRequestId = 191154;
        /// <summary>A real SAP role on the FLINS endpoint (present in DI_SAP_ROLEINFO).</summary>
        public const string FlinsTechnicalRole = "Y_HR_SPHCO_HRA_LRN_CNTPLAYER";
        /// <summary>A real SAP role that is NOT a FLINS default/general role (general roles are deliberately kept on remove).</summary>
        public const string FlinsOrdinaryRole = "Y_NWBC_GOLDEN_TAX_ROLE_WTX";

        /// <summary>A test business role mapped (DI_SAP_HRFLINSRoleMatrix) to the given technical role.</summary>
        public string CreateFlinsBusinessRole(string technicalRole)
        {
            string businessRole = Tag + "_BR" + (flinsBusinessRoles.Count + 1);
            long template = TestDb.ScalarLong("SELECT TOP 1 Id FROM DI_SAP_HRFLINSRoleMatrix WHERE ISNULL(IsDeleted, 0) = 0 ORDER BY Id");
            TestDb.CloneAndUpdate("DI_SAP_HRFLINSRoleMatrix", template,
                @"UPDATE DI_SAP_HRFLINSRoleMatrix SET BusinessRole = @br, TechnicalRule = @tr, Remarks = @tag, IsDeleted = 0, ValidFrom = DATEADD(day, -1, GETDATE()),
    ValidTo = DATEADD(year, 1, GETDATE()), CreatedBy = 'mocktest', CreatedDate = GETDATE() WHERE Id = @id",
                ("@br", businessRole), ("@tr", technicalRole), ("@tag", Tag));
            flinsBusinessRoles.Add(businessRole);
            return businessRole;
        }

        /// <summary>An approved FLINS ad hoc request, serialized with the product's own serializer and type.</summary>
        public long CreateFlinsRequest(AdHocActionType action, long sapAccountId, string businessRole)
        {
            var template = DomainModel.AdHocAccessRequest.GetById(TemplateFlinsAdHocRequestId);
            var detail = (DomainModel.AdHocSAPHRFLINSRequest)template.GetEntitlementDetail() ?? new DomainModel.AdHocSAPHRFLINSRequest();
            detail.EndpointUserId = sapAccountId;
            detail.DisplayAccountName = Tag;
            detail.AdHocSAPHRFLINSSharedAccount = null;
            // A clean line with only the business role: RetainCompanyRoles / company / cost-centre values copied from a real
            // request would send the role lookup down a different branch and resolve to no technical roles.
            var line = new DomainModel.AdHocSAPHRFLINSRequestDetail
            {
                Entitlement = businessRole ?? string.Empty,
                EntitlementType = EntitlementType.Role,
                RetainCompanyRoles = false,
            };
            detail.AdHocSAPHRFLINSRequestDetails = string.IsNullOrEmpty(businessRole)
                ? new List<DomainModel.AdHocSAPHRFLINSRequestDetail>()
                : new List<DomainModel.AdHocSAPHRFLINSRequestDetail> { line };
            string xml = Infrastructure.UtilityMethods.Serialize(detail);
            return CreateRequestCore(FlinsEndpointId, SyntheticUserId, AccessRequestSource.AdhocRequest, action, xml, TemplateFlinsAccessRequestId, TemplateFlinsAdHocRequestId);
        }

        public long SapAccountId() =>
            TestDb.ScalarLong("SELECT TOP 1 EndpointUserId FROM DI_M_AccountReconcilation WHERE UserId = @u AND EndpointSystemId = @ep ORDER BY Id DESC",
                ("@u", SyntheticUserId), ("@ep", FlinsEndpointId));

        // ------------------------------------------------------------------ O365

        public const long O365EndpointId = 19;
        private const long TemplateO365AccessRequestId = 660010;
        private const long TemplateO365AdHocRequestId = 111003;
        /// <summary>An existing O365 account row and its reconciliation row, copied to seed an O365 account for the synthetic employee.</summary>
        private const long TemplateO365UserInfoId = 5588;
        private const long TemplateO365ReconId = 130245;

        /// <summary>An approved O365 ad hoc request (DataContract XML, as the web UI stores it).</summary>
        public long CreateO365Request(AdHocActionType action, long o365AccountId, AccountType accountType = AccountType.User, Action<Dto.AdHocO365Request> customize = null)
        {
            var detail = new Dto.AdHocO365Request { EndpointUserId = o365AccountId, DisplayAccountName = Tag };
            customize?.Invoke(detail);
            string xml = Infrastructure.UtilityMethods.DataSerialize(detail);
            long requestId = CreateRequestCore(O365EndpointId, SyntheticUserId, AccessRequestSource.AdhocRequest, action, xml, TemplateO365AccessRequestId, TemplateO365AdHocRequestId);
            if (accountType != AccountType.User)
            {
                TestDb.Execute("UPDATE DI_M_AccessRequest SET AccountType = @a WHERE Id = @id", ("@a", (int)accountType), ("@id", requestId));
                TestDb.Execute("UPDATE DI_M_AdHocAccessRequest SET AccountType = @a WHERE Id = (SELECT AdHocAccessRequestId FROM DI_M_AccessRequest WHERE Id = @id)", ("@a", (int)accountType), ("@id", requestId));
            }
            return requestId;
        }

        /// <summary>
        /// Seeds an O365 account for the synthetic employee (in production this row only appears after the
        /// licence/mailbox jobs, which these tests do not run). Returns the account's id.
        /// </summary>
        public long SeedO365Account(string samAccountName)
        {
            string upn = samAccountName + "@" + (TestDb.Scalar("SELECT ConfigValue FROM DI_M_IMConfigurationMaster WHERE ConfigKey = 'ADUPNDomainName'") as string ?? "mocktest.invalid");
            long id = TestDb.CloneAndUpdate("DI_Office365_UserInfo", TemplateO365UserInfoId,
                @"UPDATE DI_Office365_UserInfo SET UserPrincipalName = @upn, MicrosoftOnlineServicesID = @upn, WindowsEmailAddress = @upn, mail = @upn, SamAccountName = @sam,
    Name = @sam, Alias = @sam, DisplayName = 'Mocktest User', FirstName = 'Mocktest', LastName = @tag, IsShared = 0, CreatedBy = 'mocktest', CreatedDate = GETDATE(),
    LastModifiedBy = NULL, LastModifiedDate = NULL WHERE Id = @id",
                ("@upn", upn), ("@sam", samAccountName), ("@tag", Tag));
            TestDb.CloneAndUpdate("DI_M_AccountReconcilation", TemplateO365ReconId,
                @"UPDATE DI_M_AccountReconcilation SET UserId = @u, EndpointUserId = @o, AccountName = @upn, FullName = 'Mocktest User', IsDeleted = 0, DeletedDate = NULL,
    IsLocked = 0, AccountType = 1, AccessRequestId = NULL, ValidToDate = NULL, CreatedBy = 'mocktest', CreatedDate = GETDATE(), LastModifiedBy = NULL, LastModifiedDate = NULL WHERE Id = @id",
                ("@u", SyntheticUserId), ("@o", id), ("@upn", upn));
            return id;
        }

        // ------------------------------------------------------------------ cleanup of the synthetic employee

        /// <summary>Endpoint account tables and their link tables, by endpoint system type.</summary>
        private static readonly Dictionary<int, (string Table, string[] Links)> AccountTables = new Dictionary<int, (string, string[])>
        {
            [1]  = ("DI_ADS_UserInfo", new[] { "ADSUserInfoADSGroupInfo|ADSUserInfo_Id" }),
            [3]  = ("DI_SAP_UserInfo", new[] { "SAPRoleInfoSAPUserinfo|SAPUserinfo_Id", "SAPProfileInfoSAPUserinfo|SAPUserinfo_Id", "SAPParameterInfoSAPUserinfo|SAPUserinfo_Id", "SAPUserinfoSAPGroupInfo|SAPUserinfo_Id" }),
            [4]  = ("DI_SAPEP_UserInfo", new[] { "SAPEPRoleInfoSAPEPUserInfo|SAPEPUserInfo_Id", "SAPEPUserInfoSAPEPGroupInfo|SAPEPUserInfo_Id" }),
            [11] = ("DI_Office365_UserInfo", new[] { "Office365UserInfoOffice365GroupInfo|Office365UserInfo_Id" }),
        };

        private List<(long Endpoint, int Type, long Account)> SnapshotSyntheticAccounts()
        {
            var list = new List<(long, int, long)>();
            if (SyntheticUserId == 0) return list;
            using (var cn = new System.Data.SqlClient.SqlConnection(TestDb.ConnectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand(@"
SELECT r.EndpointSystemId, e.EndpointSystemTypeId, r.EndpointUserId
FROM DI_M_AccountReconcilation r JOIN DI_M_EndpointSystem e ON e.Id = r.EndpointSystemId WHERE r.UserId = @u
UNION
SELECT a.EndPointSystemId, 1, a.Id FROM DI_ADS_UserInfo a JOIN STRING_SPLIT(@reqs, ',') q ON a.info LIKE '%Request Id ' + q.value + ';%'
WHERE a.EndPointSystemId = @ad AND a.CreatedDate >= @since AND q.value <> ''
UNION
SELECT EndPointSystemId, 3, Id FROM DI_SAP_UserInfo WHERE EMPLOYEEID = @emp AND CreatedDate >= @since", cn))
            {
                cmd.Parameters.AddWithValue("@u", SyntheticUserId);
                cmd.Parameters.AddWithValue("@reqs", string.Join(",", accessRequestIds));
                cmd.Parameters.AddWithValue("@ad", AdEndpointId);
                cmd.Parameters.AddWithValue("@since", startedAt);
                cmd.Parameters.AddWithValue("@emp", SyntheticEmployeeId);
                cn.Open();
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add((Convert.ToInt64(r[0]), Convert.ToInt32(r[1] == DBNull.Value ? 0 : r[1]), Convert.ToInt64(r[2])));
            }
            return list;
        }

        private List<(long Endpoint, int Type, long Account)> accountSnapshot = new List<(long, int, long)>();

        private void CleanupSyntheticAccounts()
        {
            foreach (var (ep, type, account) in accountSnapshot.Distinct())
            {
                var args = new[] { ("@ep", (object)ep), ("@acc", (object)account), ("@since", (object)startedAt) };
                TestDb.Execute("DELETE FROM DI_M_EntitlementReconcilation WHERE EndpointSystemId = @ep AND EndpointUserId = @acc", args);
                TestDb.Execute("DELETE FROM DI_M_EntitlementReconcilation_history WHERE EndpointSystemId = @ep AND EndpointUserId = @acc", args);
                if (!AccountTables.TryGetValue(type, out var t)) continue;
                foreach (var link in t.Links)
                {
                    var parts = link.Split('|');
                    TestDb.Execute($"DELETE FROM {TestDb.Quote(parts[0])} WHERE {TestDb.Quote(parts[1])} = @acc", args);
                }
                // Only rows created during this test: a real account is never touched even if ids were mixed up.
                TestDb.Execute($"DELETE FROM {TestDb.Quote(t.Table)} WHERE Id = @acc AND EndPointSystemId = @ep AND CreatedDate >= @since", args);
            }
        }

        /// <summary>An approved AD ad hoc request for the synthetic employee on the real AD endpoint.</summary>
        public long CreateAdRequest(AccessRequestSource source, AdHocActionType action, long adAccountId, string groups)
        {
            if (SyntheticUserId == 0) CreateSyntheticEmployee();
            string detail =
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n" +
                "<AdHocADRequest xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
                $"  <EndpointUserId>{adAccountId}</EndpointUserId>\r\n" +
                "  <EntitlementType>Group</EntitlementType>\r\n" +
                $"  <Entitlement>{System.Security.SecurityElement.Escape(groups ?? string.Empty)}</Entitlement>\r\n" +
                $"  <RequesterComment>{System.Security.SecurityElement.Escape(Tag)}</RequesterComment>\r\n" +
                "</AdHocADRequest>";
            return CreateRequestCore(AdEndpointId, SyntheticUserId, source, action, detail, TemplateAdAccessRequestId, TemplateAdAdHocRequestId);
        }

        /// <summary>The synthetic employee's AD account (0 when none), via account reconciliation.</summary>
        public long AdAccountId() =>
            TestDb.ScalarLong("SELECT TOP 1 EndpointUserId FROM DI_M_AccountReconcilation WHERE UserId = @u AND EndpointSystemId = @ep ORDER BY Id DESC",
                ("@u", SyntheticUserId), ("@ep", AdEndpointId));

        private void CleanupSyntheticEmployee()
        {
            if (SyntheticUserId == 0) return;
            var p = new[] { ("@u", (object)SyntheticUserId), ("@emp", (object)SyntheticEmployeeId), ("@ad", (object)AdEndpointId), ("@since", (object)startedAt) };

            // Accounts on every endpoint (AD, SAP, SAP EP/CE, O365), from the snapshot taken before any cleanup ran.
            CleanupSyntheticAccounts();

            // Reconciliation rows (all endpoints) and anything hanging off them.
            TestDb.Execute(@"
DECLARE @rec TABLE (Id bigint);
INSERT @rec SELECT Id FROM DI_M_AccountReconcilation WHERE UserId = @u;
DELETE FROM DI_M_EntitlementReconcilation WHERE AccountReconcilationId IN (SELECT Id FROM @rec);
DELETE FROM DI_M_EntitlementReconcilation_history WHERE AccountReconcilationId IN (SELECT Id FROM @rec);
DELETE FROM DI_M_ReconcilationExceptionAttributeChanges WHERE ReconcilationId IN (SELECT Id FROM @rec);", p);

            // The synthetic employee's IDMS account.
            TestDb.Execute(@"
DECLARE @iacm TABLE (Id bigint);
INSERT @iacm SELECT Id FROM DI_IACM_UserInfo WHERE EmployeeId = @emp;
DELETE FROM IACMRoleInfoIACMUserInfo WHERE IACMUserInfo_Id IN (SELECT Id FROM @iacm);
DELETE FROM IACMUserInfoIACMGroupInfo WHERE IACMUserInfo_Id IN (SELECT Id FROM @iacm);
DELETE FROM DI_IACM_UserInfo WHERE Id IN (SELECT Id FROM @iacm);", p);

            // Everything else keyed by the (brand-new, verified unreferenced) user id, then the user.
            TestDb.DeleteEverywhere(SyntheticUserId, new[] { "UserId", "User_Id", "RequesteeUserId", "WogUserId", "DIUserId" }, new[] { "DI_M_User" });
            TestDb.Execute("DELETE FROM DI_M_User WHERE Id = @u AND EmployeeId = @emp", p);
        }

        public void Dispose()
        {
            MockConnector.Uninstall();
            var errors = new List<Exception>();
            // Every step runs even if an earlier one fails, so one problem never strands the rest of the test data.
            void Step(Action a) { try { a(); } catch (Exception ex) { errors.Add(ex); } }

            // Snapshot every account the synthetic employee holds before anything is deleted: the per-request cleanup
            // removes reconciliation rows (they carry AccessRequestId), which are the main way those accounts are found.
            Step(() => accountSnapshot = SnapshotSyntheticAccounts());

            // First: the synthetic employee's accounts, then the per-request rows.
            Step(CleanupSyntheticEmployee);

            // Rows the executor attached to the test's requests (tasks, logs, attachments, ...).
            foreach (long id in accessRequestIds)
                Step(() => TestDb.DeleteEverywhere(id, new[] { "AccessRequestId", "AccessRequest_Id", "RequestId", "ParentRequestId" }, new[] { "DI_M_AccessRequest" }));
            foreach (long id in adHocRequestIds)
                Step(() => TestDb.DeleteEverywhere(id, new[] { "AdHocAccessRequestId", "AdHocAccessRequest_Id" }, new[] { "DI_M_AdHocAccessRequest" }));
            // The test's own request rows, by Id, whichever endpoint they point at.
            foreach (long id in accessRequestIds)
                Step(() => TestDb.Execute("DELETE FROM DI_M_AccessRequest WHERE Id = @id AND CreatedBy = 'mocktest'", ("@id", id)));
            foreach (long id in adHocRequestIds)
                Step(() => TestDb.Execute("DELETE FROM DI_M_AdHocAccessRequest WHERE Id = @id AND CreatedBy = 'mocktest'", ("@id", id)));

            // Second pass: the user row itself can only go once the requests that reference it are gone.
            Step(() => accountSnapshot.AddRange(SnapshotSyntheticAccounts()));
            Step(CleanupSyntheticEmployee);

            // Test-owned matrix rows: the ones the test seeded and any the product's validators auto-inserted for the
            // test org (ProvisioningValidator inserts a blank row when none matches). Only the unique test company codes match.
            foreach (string co in orgCompanies)
            {
                Step(() => TestDb.Execute("DELETE FROM DI_ADS_XLProvMatrix WHERE CompanyCode = @co", ("@co", co)));
                Step(() => TestDb.Execute("DELETE FROM DI_M_ProvisioningConfigMatrixSAPORG WHERE CompanyCode = @co", ("@co", co)));
            }
            foreach (string br in flinsBusinessRoles)
                Step(() => TestDb.Execute("DELETE FROM DI_SAP_HRFLINSRoleMatrix WHERE BusinessRole = @br AND Remarks = @tag", ("@br", br), ("@tag", Tag)));

            // Emails queued while the test ran (none can be sent: no Emailing service runs here, but don't leave them).
            Step(() => TestDb.Execute("DELETE FROM DI_M_Email WHERE Id > @id AND CreatedDate >= @since", ("@id", emailIdBefore), ("@since", startedAt)));

            if (ProcessId != 0)
            {
                Step(() => TestDb.Execute("DELETE FROM DI_M_ProcessHistroy WHERE ProcessId = @p", ("@p", ProcessId)));
                Step(() => TestDb.Execute("UPDATE DI_M_DiscoveryProcess SET process_Id = NULL WHERE Id = @d", ("@d", DiscoveryProcessId)));
                Step(() => TestDb.Execute("DELETE FROM DI_M_Process WHERE Id = @p", ("@p", ProcessId)));
            }
            if (SchedulerId != 0) Step(() => TestDb.Execute("DELETE FROM DI_M_Scheduler WHERE Id = @s", ("@s", SchedulerId)));
            if (DiscoveryProcessId != 0) Step(() => TestDb.Execute("DELETE FROM DI_M_DiscoveryProcess WHERE Id = @d", ("@d", DiscoveryProcessId)));
            if (EndpointId != 0)
            {
                // Logs, reconciliation and any other rows the run wrote against the test endpoint (only ever a MOCKTEST endpoint).
                Step(() => TestDb.DeleteEverywhere(EndpointId,
                    new[] { "EndpointSystemId", "EndPointSystemId", "Endpointsystemid", "EndpointId", "EndPointId" },
                    new[] { "DI_M_EndpointSystem" }));
                Step(() => TestDb.Execute("DELETE FROM DI_M_EndpointSystem WHERE Id = @ep AND ApplicationCode = @tag", ("@ep", EndpointId), ("@tag", Tag)));
            }

            MockConnector.Reset();

            // A leak fails the test. (A failed step that a later pass made up for, e.g. a foreign key on the first
            // CleanupSyntheticEmployee pass, is harmless and not reported.)
            long leftover = LeftoverCount();
            if (leftover > 0)
                throw new AggregateException($"Test cleanup left {leftover} rows behind (tag {Tag}).", errors);
        }

        /// <summary>Rows this test created that still exist (should be 0 after Dispose).</summary>
        public long LeftoverCount()
        {
            long n = TestDb.ScalarLong(@"
SELECT (SELECT COUNT(*) FROM DI_M_EndpointSystem WHERE ApplicationCode = @tag)
     + (SELECT COUNT(*) FROM DI_M_AccessRequest WHERE Description = @tag OR (UserId = @u AND @u <> 0))
     + (SELECT COUNT(*) FROM DI_M_AdHocAccessRequest WHERE Comments = @tag OR (RequesteeUserId = @u AND @u <> 0))
     + (SELECT COUNT(*) FROM DI_M_User WHERE Id = @u AND EmployeeId = @emp)
     + (SELECT COUNT(*) FROM DI_IACM_UserInfo WHERE EmployeeId = @emp)
     + (SELECT COUNT(*) FROM DI_M_AccountReconcilation WHERE UserId = @u AND @u <> 0)
     + (SELECT COUNT(*) FROM DI_M_EmployeeEventTrigger WHERE UserId = @u AND @u <> 0)
     + (SELECT COUNT(*) FROM DI_M_OUChangeSnapshot WHERE UserId = @u AND @u <> 0)
     + (SELECT COUNT(*) FROM DI_SAP_HRFLINSRoleMatrix WHERE Remarks = @tag)
     + (SELECT COUNT(*) FROM DI_ADS_XLProvMatrix WHERE CompanyCode IN (SELECT value FROM STRING_SPLIT(@cos, ',') WHERE value <> ''))
     + (SELECT COUNT(*) FROM DI_M_ProvisioningConfigMatrixSAPORG WHERE CompanyCode IN (SELECT value FROM STRING_SPLIT(@cos, ',') WHERE value <> ''))
     + (SELECT COUNT(*) FROM DI_ADS_UserInfo a JOIN STRING_SPLIT(@reqs, ',') r ON a.info LIKE '%Request Id ' + r.value + ';%'
        WHERE a.EndPointSystemId = @ad AND a.CreatedDate >= @since AND r.value <> '')",
                ("@tag", Tag), ("@u", SyntheticUserId), ("@emp", (object)SyntheticEmployeeId ?? ""), ("@cos", string.Join(",", orgCompanies)),
                ("@reqs", string.Join(",", accessRequestIds)), ("@ad", AdEndpointId), ("@since", startedAt));

            // Every account row seen during the test (AD, SAP, SAP EP/CE, O365) must be gone too.
            foreach (var (ep, type, account) in accountSnapshot.Distinct())
                if (AccountTables.TryGetValue(type, out var t))
                    n += TestDb.ScalarLong($"SELECT COUNT(*) FROM {TestDb.Quote(t.Table)} WHERE Id = @acc AND EndPointSystemId = @ep AND CreatedDate >= @since",
                        ("@acc", account), ("@ep", ep), ("@since", startedAt));
            return n;
        }
    }
}
