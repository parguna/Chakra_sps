using System.Linq;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.ProvisioningProcessor;
using Xunit;
using Xunit.Abstractions;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// SAP HRMS/FLINS (endpoint 5, the most used SAP executor: User.FLINS / FLINSBase) with the mock standing in
    /// for SAP. FLINS takes the account name from the employee's AD account, so each test first onboards the
    /// synthetic full-time employee in AD (also mocked). Roles are requested as a test business role that the
    /// role matrix maps to a real SAP technical role.
    /// </summary>
    [Collection(MockedCollection.Name)]
    public class SapFlinsTests
    {
        private readonly ITestOutputHelper output;
        public SapFlinsTests(ITestOutputHelper output) { this.output = output; }

        private const int FlinsApplicationType = (int)ApplicationType.HRMS_FLINS;

        private static void Run(long requestId) => new ProvisioningEngine().StartProvisioningTest(requestId);

        private ConnectorCall[] CallsFor(long requestId)
        {
            var calls = MockConnector.Calls.Where(c => c.RequestId == requestId).ToArray();
            foreach (var c in calls) output.WriteLine($"req {requestId}: {c}");
            return calls;
        }

        /// <summary>The product's own provisioning log for a request (written to the test output to explain failures).</summary>
        private void LogFor(long requestId)
        {
            using (var cn = new System.Data.SqlClient.SqlConnection(TestDb.ConnectionString))
            using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT TOP 40 LogLevel, Message, ExceptionMessage FROM DI_Log_Provisioning WHERE AccessRequestId = @r ORDER BY Id", cn))
            {
                cmd.Parameters.AddWithValue("@r", requestId);
                cn.Open();
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) output.WriteLine($"log {requestId}: [{r[0]}] {r[1]} {r[2]}");
            }
        }

        /// <summary>Synthetic full-time employee with an AD account (FLINS account names come from AD).</summary>
        private static void EmployeeWithAdAccount(TestWorld w)
        {
            w.CreateSyntheticEmployee(fullTime: true);
            long trigger = w.CreateEventTrigger(EmployeeEvents.NewJoinee);
            long onboard = w.CreateLifecycleRequest(TestWorld.AdEndpointId, AccessRequestSource.OnBoarding, trigger, (int)ApplicationType.ADSingaporePowerLocal);
            Run(onboard);
            Assert.NotEqual(0, w.AdAccountId());
        }

        private long CreateSapAccount(TestWorld w, string businessRole, out long request)
        {
            request = w.CreateFlinsRequest(AdHocActionType.NewAccount, 0, businessRole);
            Run(request);
            return w.SapAccountId();
        }

        private static bool SapLocked(long account) =>
            (bool)(TestDb.Scalar("SELECT ISNULL(IsUserLocked, 0) FROM DI_SAP_UserInfo WHERE Id = @a", ("@a", account)) ?? false);

        [Fact]
        public void New_SAP_account_is_created_with_the_requested_role()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                string role = w.CreateFlinsBusinessRole(TestWorld.FlinsTechnicalRole);

                long account = CreateSapAccount(w, role, out long request);

                var calls = CallsFor(request);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(request));
                Assert.Contains(calls, c => c.Method == "CreateAccount" && c.EndpointId == TestWorld.FlinsEndpointId);
                // The business role was resolved to its technical role and that exact role was sent to SAP.
                Assert.Contains(calls, c => c.Method == "AddEntitlement" && c.EndpointId == TestWorld.FlinsEndpointId
                    && (c.Detail ?? "").StartsWith("Role") && c.Detail.Contains(TestWorld.FlinsTechnicalRole));
                Assert.NotEqual(0, account);
            }
        }

        [Fact]
        public void New_SAP_account_is_rejected_when_the_role_is_not_in_the_SAP_role_catalogue()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                string role = w.CreateFlinsBusinessRole("MOCKTEST_NO_SUCH_ROLE");

                long account = CreateSapAccount(w, role, out long request);

                var calls = CallsFor(request);
                Assert.NotEqual(AccessRequestProgress.Completed, w.RequestProgress(request));
                Assert.DoesNotContain(calls, c => c.Method == "CreateAccount");
                Assert.Equal(0, account);
            }
        }

        [Fact]
        public void Failed_SAP_create_fails_the_request_and_records_no_account()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                string role = w.CreateFlinsBusinessRole(TestWorld.FlinsTechnicalRole);
                MockConnector.FailOn.Add("CreateAccount");

                long account = CreateSapAccount(w, role, out long request);

                CallsFor(request);
                Assert.Equal(AccessRequestProgress.ExecutionFailed, w.RequestProgress(request));
                Assert.Equal(0, account);
            }
        }

        [Fact]
        public void SAP_role_add_then_remove_reaches_SAP()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                // An ordinary role: FLINS deliberately keeps its default/general roles when a remove is requested.
                string role = w.CreateFlinsBusinessRole(TestWorld.FlinsOrdinaryRole);
                long account = CreateSapAccount(w, null, out _);
                Assert.NotEqual(0, account);

                long add = w.CreateFlinsRequest(AdHocActionType.AmendmentAdd, account, role);
                Run(add);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(add));
                Assert.Contains(CallsFor(add), c => c.Method == "AddEntitlement" && (c.Detail ?? "").StartsWith("Role") && c.Detail.Contains(TestWorld.FlinsOrdinaryRole));

                long remove = w.CreateFlinsRequest(AdHocActionType.AmendmentRemove, account, role);
                Run(remove);
                output.WriteLine("remove status: " + w.RequestProgress(remove));
                LogFor(remove);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(remove));
                Assert.Contains(CallsFor(remove), c => c.Method == "RemoveEntitlement" && (c.Detail ?? "").Contains(TestWorld.FlinsOrdinaryRole));
            }
        }

        [Fact]
        public void SAP_remove_keeps_default_general_roles()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                string generalRole = w.CreateFlinsBusinessRole(TestWorld.FlinsTechnicalRole); // in FLINS's default "Roles" field
                long account = CreateSapAccount(w, generalRole, out _);
                Assert.NotEqual(0, account);

                long remove = w.CreateFlinsRequest(AdHocActionType.AmendmentRemove, account, generalRole);
                Run(remove);

                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(remove));
                Assert.DoesNotContain(CallsFor(remove), c => c.Method == "RemoveEntitlement" && (c.Detail ?? "").Contains(TestWorld.FlinsTechnicalRole));
            }
        }

        [Fact]
        public void SAP_account_delete_locks_the_account_and_removes_its_roles()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                long account = CreateSapAccount(w, null, out _);
                Assert.NotEqual(0, account);

                long delete = w.CreateFlinsRequest(AdHocActionType.DeleteAccount, account, null);
                Run(delete);

                var calls = CallsFor(delete).Where(c => c.EndpointId == TestWorld.FlinsEndpointId).Select(c => c.Method).ToList();
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(delete));
                Assert.Contains("UpdateAccount", calls);     // valid-to set to today
                Assert.Contains("LockAccount", calls);
                Assert.Contains("RemoveAllEntitlements", calls);
                Assert.True(SapLocked(account));
            }
        }

        [Fact]
        public void SAP_offboarding_locks_the_account_and_delimits_the_logon_id_in_HR()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                long account = CreateSapAccount(w, null, out _);
                Assert.NotEqual(0, account);

                long trigger = w.CreateEventTrigger(EmployeeEvents.Resignation);
                long offboard = w.CreateLifecycleRequest(TestWorld.FlinsEndpointId, AccessRequestSource.OffBoarding, trigger, FlinsApplicationType);
                Run(offboard);

                var calls = CallsFor(offboard);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(offboard));
                Assert.Contains(calls, c => c.Method == "LockAccount");
                Assert.True(SapLocked(account));
                // The HR write-back goes to the trusted-source (HR) endpoint with no request attached; the mock caught it.
                Assert.Contains(MockConnector.Calls, c => c.Method == "DelimitFLINSLogonIdInHR" && c.Detail == w.SyntheticEmployeeId);
            }
        }

        [Fact]
        public void SAP_mover_changes_the_user_groups_for_the_new_department()
        {
            using (var w = new TestWorld())
            {
                EmployeeWithAdAccount(w);
                long account = CreateSapAccount(w, null, out _);
                Assert.NotEqual(0, account);

                w.MoveSyntheticEmployeeToNewOrg();
                long trigger = w.CreateEventTrigger(EmployeeEvents.OUChange);
                long mover = w.CreateLifecycleRequest(TestWorld.FlinsEndpointId, AccessRequestSource.OUChange, trigger, FlinsApplicationType);
                Run(mover);

                var calls = CallsFor(mover);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(mover));
                Assert.Contains(calls, c => c.Method == "ChangeContainer" && c.EndpointId == TestWorld.FlinsEndpointId);
            }
        }
    }
}
