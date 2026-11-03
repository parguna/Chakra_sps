using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.ProvisioningProcessor;
using Xunit;
using Xunit.Abstractions;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// O365 (endpoint 19, O365Base) with the mock standing in for both Office 365 and AD (user licences are
    /// driven through AD attributes/groups). Notes on scope:
    ///  - A user's new O365 account stops at WaitingForLicenseAssignment: the next stages run in batch jobs
    ///    (RequestExecutor.AssignO365License / CreateO365MailBox) that pick up every waiting O365 request in
    ///    the database, so they are not run from tests.
    ///  - Delete / licence / offboarding tests seed the O365 account row that those jobs would normally create.
    ///  - The product waits TimeDelayForDownloadO365User (60 s in this database) before DownloadUser calls.
    /// </summary>
    [Collection(MockedCollection.Name)]
    public class O365Tests
    {
        private readonly ITestOutputHelper output;
        public O365Tests(ITestOutputHelper output) { this.output = output; }

        private const int O365ApplicationType = (int)ApplicationType.Office365;

        private static void Run(long requestId) => new ProvisioningEngine().StartProvisioningTest(requestId);

        private ConnectorCall[] CallsFor(long requestId)
        {
            var calls = MockConnector.Calls.Where(c => c.RequestId == requestId).ToArray();
            foreach (var c in calls) output.WriteLine($"req {requestId}: {c}");
            foreach (var l in TestDb.ProvisioningLog(requestId)) output.WriteLine($"log {requestId}: {l}");
            return calls;
        }

        private static void EmployeeWithAdAccount(TestWorld w)
        {
            w.CreateSyntheticEmployee(fullTime: true);
            long trigger = w.CreateEventTrigger(EmployeeEvents.NewJoinee);
            long onboard = w.CreateLifecycleRequest(TestWorld.AdEndpointId, AccessRequestSource.OnBoarding, trigger, (int)ApplicationType.ADSingaporePowerLocal);
            Run(onboard);
            Assert.NotEqual(0, w.AdAccountId());
        }

        private static string AdAccountName(TestWorld w) =>
            TestDb.Scalar("SELECT sAMAccountName FROM DI_ADS_UserInfo WHERE Id = @a", ("@a", w.AdAccountId())) as string;

        /// <summary>
        /// A licence (SKU and its service plans) from the O365 endpoint's own provisioning defaults, the same pair
        /// the web UI sends when a licence is requested.
        /// </summary>
        private static Dto.AdHocLicenseInfo License(bool remove = false)
        {
            string xml = TestDb.Scalar("SELECT TOP 1 Value FROM DI_M_NProvisioningField WHERE EndPointSystemId = @ep AND AccountType = 1 AND FieldName = 'Licenses'",
                ("@ep", TestWorld.O365EndpointId)) as string ?? "";
            // ProvLicenseInfo XML: <ProvLicenseInfoDetail><License>TENANT:SKU</License><ServiceName>PLAN,PLAN,...</ServiceName>
            var m = Regex.Match(xml, "<License>([^<]+)</License>\\s*<ServiceName>([^<]*)</ServiceName>");
            Assert.True(m.Success, "O365 'Licenses' provisioning field has no License/ServiceName entry");
            return new Dto.AdHocLicenseInfo { AccountSkuId = m.Groups[1].Value, ServiceName = m.Groups[2].Value, IsRemoveLicense = remove };
        }

        private static (bool Locked, bool Deleted) O365Recon(TestWorld w, long account)
        {
            var r = TestDb.Row("SELECT IsLocked, IsDeleted FROM DI_M_AccountReconcilation WHERE UserId = @u AND EndpointSystemId = @ep AND EndpointUserId = @a",
                ("@u", w.SyntheticUserId), ("@ep", TestWorld.O365EndpointId), ("@a", account));
            return ((bool)r["IsLocked"], (bool)r["IsDeleted"]);
        }

        [Fact]
        public void O365_new_user_account_prepares_AD_and_waits_for_licence_assignment()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                long request = w.CreateO365Request(AdHocActionType.NewAccount, 0);
                Run(request);

                CallsFor(request);
                Assert.Equal(AccessRequestProgress.WaitingForLicenseAssignment, w.RequestProgress(request));
                Assert.Equal(AdAccountName(w), TestDb.Scalar("SELECT Remarks FROM DI_M_AccessRequest WHERE Id = @r", ("@r", request)) as string);
                Assert.NotNull(TestDb.Scalar("SELECT AdditionalInfo FROM DI_M_AccessRequest WHERE Id = @r", ("@r", request)));
            }
        }

        [Fact]
        public void O365_new_user_account_fails_when_the_employee_has_no_AD_account()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee(fullTime: true);
                long request = w.CreateO365Request(AdHocActionType.NewAccount, 0);
                Run(request);

                CallsFor(request);
                Assert.Equal(AccessRequestProgress.ExecutionFailed, w.RequestProgress(request));
            }
        }

        [Fact]
        public void O365_licence_add_assigns_the_licence_in_Office_365()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                long account = w.SeedO365Account(AdAccountName(w));
                                long request = w.CreateO365Request(AdHocActionType.AmendmentAdd, account, AccountType.User, d =>
                {
                    d.LicenseAction = LicenseActionType.Add;
                    d.LicenseInfo = new List<Dto.AdHocLicenseInfo> { License() };
                });
                Run(request);

                var calls = CallsFor(request);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(request));
                Assert.Contains(calls, c => c.Method == "AssignLicense" && c.EndpointId == TestWorld.O365EndpointId);
            }
        }

        [Fact]
        public void O365_licence_remove_removes_the_licence_in_Office_365()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                long account = w.SeedO365Account(AdAccountName(w));
                                long request = w.CreateO365Request(AdHocActionType.AmendmentRemove, account, AccountType.User, d =>
                {
                    d.LicenseAction = LicenseActionType.Remove;
                    d.LicenseInfo = new List<Dto.AdHocLicenseInfo> { License(remove: true) };
                });
                Run(request);

                var calls = CallsFor(request);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(request));
                Assert.Contains(calls, c => c.Method == "RemoveLicense" && c.EndpointId == TestWorld.O365EndpointId);
            }
        }

        [Fact]
        public void O365_delete_disables_the_user_through_AD_and_closes_the_account()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                long account = w.SeedO365Account(AdAccountName(w));

                long request = w.CreateO365Request(AdHocActionType.DeleteAccount, account);
                Run(request);

                var calls = CallsFor(request);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(request));
                // User licences hang off AD: the post-office-box attribute is removed on the AD account.
                Assert.Contains(calls, c => c.Method == "UpdatePOBoxAttribute" && c.Detail == "remove" && c.EndpointId == TestWorld.AdEndpointId);
                var recon = O365Recon(w, account);
                Assert.True(recon.Locked && recon.Deleted);
            }
        }

        [Fact]
        public void O365_offboarding_disables_the_user_and_closes_the_account()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                long account = w.SeedO365Account(AdAccountName(w));

                long trigger = w.CreateEventTrigger(EmployeeEvents.Resignation);
                long offboard = w.CreateLifecycleRequest(TestWorld.O365EndpointId, AccessRequestSource.OffBoarding, trigger, O365ApplicationType);
                Run(offboard);

                var calls = CallsFor(offboard);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(offboard));
                // Offboarding always reports success, so assert on what was actually done.
                Assert.Contains(calls, c => c.Method == "UpdatePOBoxAttribute" && c.Detail == "remove");
                var recon = O365Recon(w, account);
                Assert.True(recon.Locked && recon.Deleted);
            }
        }

        /// <summary>
        /// A new shared mailbox is created and downloaded; members are added only when the request asks for it
        /// (SharedMailBoxAction = MemberAddition). Licences are assigned only when converting a user mailbox to
        /// a shared one (MailBoxConversionType.UserToShared), which is not covered here.
        /// </summary>
        [Fact]
        public void O365_shared_mailbox_is_created_with_its_members()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                string mailbox = "MT" + w.Tag.Substring(w.Tag.Length - 6) + "SHR";
                long request = w.CreateO365Request(AdHocActionType.NewAccount, 0, AccountType.Shared, d =>
                {
                    d.AdHocO365SharedAccount = new Dto.AdHocO365SharedAccount { AccountName = mailbox, DisplayName = "Mocktest Shared " + w.Tag };
                    d.SharedMailBoxAction = SharedMailBoxActionType.MemberAddition;
                    d.Members = new Dictionary<string, string> { [AdAccountName(w)] = "FullAccess" };
                });
                Run(request);

                var calls = CallsFor(request).Where(c => c.EndpointId == TestWorld.O365EndpointId).Select(c => c.Method).ToList();
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(request));
                Assert.Contains("CreateAccount", calls);
                Assert.Contains("DownloadUser", calls);
                Assert.Contains("AssignMembers", calls);
                Assert.True(calls.IndexOf("CreateAccount") < calls.IndexOf("AssignMembers"), "members are added after the mailbox exists");
                Assert.DoesNotContain("AssignLicense", calls);
            }
        }

        [Fact]
        public void O365_shared_mailbox_create_failure_fails_the_request()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                MockConnector.FailOn.Add("CreateAccount");
                string mailbox = "MT" + w.Tag.Substring(w.Tag.Length - 6) + "SHR";
                long request = w.CreateO365Request(AdHocActionType.NewAccount, 0, AccountType.Shared, d =>
                    d.AdHocO365SharedAccount = new Dto.AdHocO365SharedAccount { AccountName = mailbox, DisplayName = "Mocktest Shared " + w.Tag });
                Run(request);

                var calls = CallsFor(request);
                Assert.Equal(AccessRequestProgress.ExecutionFailed, w.RequestProgress(request));
                Assert.DoesNotContain(calls, c => c.Method == "AssignLicense");
            }
        }
    }
}
