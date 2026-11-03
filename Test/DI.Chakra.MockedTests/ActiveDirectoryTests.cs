using System.Linq;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.ProvisioningProcessor;
using Xunit;
using Xunit.Abstractions;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// Runs AD requests through the real AD form executor (SPLocal) for a synthetic employee, with the mock
    /// connector standing in for Active Directory. Unlike In-house, AD is a connected system, so these tests
    /// assert the exact operations sent to the directory (create, add/remove group, disable, move OU).
    /// </summary>
    [Collection(MockedCollection.Name)]
    public class ActiveDirectoryTests
    {
        private readonly ITestOutputHelper output;
        public ActiveDirectoryTests(ITestOutputHelper output) { this.output = output; }

        /// <summary>An existing AD group in the directory data (only the mock "adds" the synthetic user to it).</summary>
        private const string Group = "View-Only Organization Management";

        private static void Run(long requestId) => new ProvisioningEngine().StartProvisioningTest(requestId);

        private string[] CallsFor(long requestId)
        {
            var calls = MockConnector.Calls.Where(c => c.RequestId == requestId).ToList();
            foreach (var c in calls) output.WriteLine($"req {requestId}: {c}");
            return calls.Select(c => c.Method).ToArray();
        }

        private static long CreateAccount(TestWorld w, out long request)
        {
            request = w.CreateAdRequest(AccessRequestSource.AdhocRequest, AdHocActionType.NewAccount, 0, "");
            Run(request);
            return w.AdAccountId();
        }

        [Fact]
        public void New_AD_account_is_created_in_the_directory_with_an_existing_OU()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee();
                long account = CreateAccount(w, out long request);

                var calls = CallsFor(request);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(request));
                Assert.Contains("Connect", calls);
                Assert.Contains("IsUserPropertyExists", calls);   // OU checked before creating
                Assert.Contains("CreateAccount", calls);
                Assert.True(System.Array.IndexOf(calls, "IsUserPropertyExists") < System.Array.IndexOf(calls, "CreateAccount"));
                Assert.NotEqual(0, account);
            }
        }

        [Fact]
        public void New_AD_account_is_not_created_when_the_target_OU_is_missing()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee();
                MockConnector.FailOn.Add("IsUserPropertyExists");
                long account = CreateAccount(w, out long request);

                var calls = CallsFor(request);
                Assert.Equal(AccessRequestProgress.ExecutionFailed, w.RequestProgress(request));
                Assert.DoesNotContain("CreateAccount", calls);
                Assert.Equal(0, account);
            }
        }

        [Fact]
        public void Failed_directory_create_fails_the_request_and_records_no_account()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee();
                MockConnector.FailOn.Add("CreateAccount");
                long account = CreateAccount(w, out long request);

                CallsFor(request);
                Assert.Equal(AccessRequestProgress.ExecutionFailed, w.RequestProgress(request));
                Assert.Equal(0, account);
            }
        }

        [Fact]
        public void AD_group_add_then_remove_reaches_the_directory()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee();
                long account = CreateAccount(w, out _);
                Assert.NotEqual(0, account);

                long add = w.CreateAdRequest(AccessRequestSource.AdhocRequest, AdHocActionType.AmendmentAdd, account, Group);
                Run(add);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(add));
                Assert.Contains("AddEntitlement", CallsFor(add));

                long remove = w.CreateAdRequest(AccessRequestSource.AdhocRequest, AdHocActionType.AmendmentRemove, account, Group);
                Run(remove);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(remove));
                Assert.Contains("RemoveEntitlement", CallsFor(remove));
            }
        }

        [Fact]
        public void AD_account_delete_disables_and_moves_to_the_disabled_OU()
        {
            using (var w = new TestWorld())
            {
                w.CreateSyntheticEmployee();
                long account = CreateAccount(w, out _);
                Assert.NotEqual(0, account);

                long delete = w.CreateAdRequest(AccessRequestSource.AdhocRequest, AdHocActionType.DeleteAccount, account, "");
                Run(delete);

                var calls = CallsFor(delete);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(delete));
                Assert.True(calls.Contains("DisableAccount") || calls.Contains("LockAccount"), "account should be disabled in the directory");
                Assert.Contains("MoveOU", calls);
            }
        }
    }
}
