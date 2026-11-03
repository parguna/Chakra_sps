using System.Linq;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.ProvisioningProcessor;
using Xunit;
using Xunit.Abstractions;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// HR lifecycle events (onboarding, offboarding, mover) on the real AD endpoint for a synthetic full-time
    /// employee with test-owned org codes. Requests are built the way ProvisioningFlowHelper builds them from an
    /// HR event trigger (no ad hoc record), and run through the real AD executor with the mock directory.
    /// </summary>
    [Collection(MockedCollection.Name)]
    public class AdLifecycleTests
    {
        private readonly ITestOutputHelper output;
        public AdLifecycleTests(ITestOutputHelper output) { this.output = output; }

        private const int AdApplicationType = (int)ApplicationType.ADSingaporePowerLocal;

        private static void Run(long requestId) => new ProvisioningEngine().StartProvisioningTest(requestId);

        private ConnectorCall[] CallsFor(long requestId)
        {
            var calls = MockConnector.Calls.Where(c => c.RequestId == requestId).ToArray();
            foreach (var c in calls) output.WriteLine($"req {requestId}: {c}");
            return calls;
        }

        private static string Config(string key) =>
            TestDb.Scalar("SELECT ConfigValue FROM DI_M_IMConfigurationMaster WHERE ConfigKey = @k", ("@k", key)) as string;

        private static long Onboard(TestWorld w)
        {
            long trigger = w.CreateEventTrigger(EmployeeEvents.NewJoinee);
            long request = w.CreateLifecycleRequest(TestWorld.AdEndpointId, AccessRequestSource.OnBoarding, trigger, AdApplicationType);
            Run(request);
            return request;
        }

        private static (bool Locked, bool Deleted) Recon(TestWorld w, long account)
        {
            var r = TestDb.Row("SELECT IsLocked, IsDeleted FROM DI_M_AccountReconcilation WHERE UserId = @u AND EndpointSystemId = @ep AND EndpointUserId = @a",
                ("@u", w.SyntheticUserId), ("@ep", TestWorld.AdEndpointId), ("@a", account));
            return ((bool)r["IsLocked"], (bool)r["IsDeleted"]);
        }

        private static long SnapshotCount(TestWorld w, long requestId, EmployeeEvents evt) =>
            TestDb.ScalarLong("SELECT COUNT(*) FROM DI_M_OUChangeSnapshot WHERE UserId = @u AND RequestId = @r AND EmployeeeEventType = @e",
                ("@u", w.SyntheticUserId), ("@r", requestId), ("@e", (int)evt));

        [Fact]
        public void Onboarding_creates_the_AD_account_in_the_matrix_OU_with_default_groups()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee(fullTime: true);
                long request = Onboard(w);

                var calls = CallsFor(request);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(request));
                Assert.Contains(calls, c => c.Method == "IsUserPropertyExists" && c.Detail == "OU=" + w.MatrixOu);
                Assert.Contains(calls, c => c.Method == "CreateAccount");
                Assert.Contains(calls, c => c.Method == "AddEntitlement" && c.Detail == "Group");
                Assert.NotEqual(0, w.AdAccountId());
                Assert.Equal(w.MatrixOu, TestDb.Scalar("SELECT ou FROM DI_ADS_UserInfo WHERE Id = @a", ("@a", w.AdAccountId())) as string);
                Assert.True((bool)TestDb.Scalar("SELECT IsActive FROM DI_M_User WHERE Id = @u", ("@u", w.SyntheticUserId)));
            }
        }

        [Fact]
        public void Onboarding_does_nothing_in_the_directory_when_the_account_already_exists()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee(fullTime: true);
                Onboard(w);
                Assert.NotEqual(0, w.AdAccountId());

                long second = Onboard(w);

                var calls = CallsFor(second);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(second));
                Assert.DoesNotContain(calls, c => c.Method == "CreateAccount");
            }
        }

        [Fact]
        public void Offboarding_disables_the_account_and_moves_it_to_the_DENY_Access_OU()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee(fullTime: true);
                Onboard(w);
                long account = w.AdAccountId();
                Assert.NotEqual(0, account);

                long trigger = w.CreateEventTrigger(EmployeeEvents.Resignation);
                long offboard = w.CreateLifecycleRequest(TestWorld.AdEndpointId, AccessRequestSource.OffBoarding, trigger, AdApplicationType);
                Run(offboard);

                var calls = CallsFor(offboard);
                string disabledOu = Config("AD_DISABLED_OU");
                Assert.False(string.IsNullOrEmpty(disabledOu));
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(offboard));
                Assert.Contains(calls, c => c.Method == "RemoveEntitlement" && c.Detail == "Group");
                Assert.Contains(calls, c => c.Method == "DisableAccount");
                Assert.Contains(calls, c => c.Method == "MoveOU" && c.Detail == disabledOu);
                var order = calls.Select(c => c.Method).ToList();
                Assert.True(order.IndexOf("DisableAccount") < order.IndexOf("MoveOU"), "account is disabled before it is moved");
                Assert.True(Recon(w, account).Locked);
                Assert.Equal(1, SnapshotCount(w, offboard, EmployeeEvents.Resignation));
            }
        }

        [Fact]
        public void Offboarding_fails_without_touching_the_account_when_the_DENY_Access_OU_is_missing()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee(fullTime: true);
                Onboard(w);
                long account = w.AdAccountId();
                Assert.NotEqual(0, account);

                MockConnector.FailOn.Add("IsUserPropertyExists");
                long trigger = w.CreateEventTrigger(EmployeeEvents.Resignation);
                long offboard = w.CreateLifecycleRequest(TestWorld.AdEndpointId, AccessRequestSource.OffBoarding, trigger, AdApplicationType);
                Run(offboard);

                var calls = CallsFor(offboard);
                Assert.Equal(AccessRequestProgress.ExecutionFailed, w.RequestProgress(offboard));
                Assert.DoesNotContain(calls, c => c.Method == "DisableAccount");
                Assert.DoesNotContain(calls, c => c.Method == "MoveOU");
                Assert.False(Recon(w, account).Locked);
            }
        }

        [Fact]
        public void Mover_puts_the_account_in_the_OU_of_the_new_department()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee(fullTime: true);
                Onboard(w);
                string oldOu = w.MatrixOu;
                Assert.NotEqual(0, w.AdAccountId());

                string newOu = w.MoveSyntheticEmployeeToNewOrg();
                Assert.NotEqual(oldOu, newOu);
                long trigger = w.CreateEventTrigger(EmployeeEvents.OUChange);
                long mover = w.CreateLifecycleRequest(TestWorld.AdEndpointId, AccessRequestSource.OUChange, trigger, AdApplicationType);
                Run(mover);

                var calls = CallsFor(mover);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(mover));
                Assert.Contains(calls, c => c.Method == "ChangeContainer" && c.Detail == newOu);
                Assert.DoesNotContain(calls, c => c.Method == "ChangeContainer" && c.Detail == oldOu);
                Assert.Contains(calls, c => c.Method == "AddEntitlement" && c.Detail == "Group");
                Assert.Equal(1, SnapshotCount(w, mover, EmployeeEvents.OUChange));
            }
        }

        [Fact]
        public void Mover_fails_and_sends_nothing_when_the_employee_has_no_AD_account()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee(fullTime: true);
                w.MoveSyntheticEmployeeToNewOrg();
                long trigger = w.CreateEventTrigger(EmployeeEvents.OUChange);
                long mover = w.CreateLifecycleRequest(TestWorld.AdEndpointId, AccessRequestSource.OUChange, trigger, AdApplicationType);
                Run(mover);

                var calls = CallsFor(mover);
                Assert.Equal(AccessRequestProgress.ExecutionFailed, w.RequestProgress(mover));
                Assert.DoesNotContain(calls, c => c.Method == "ChangeContainer");
            }
        }
    }
}
