using System.Linq;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.ProvisioningProcessor;
using Xunit;
using Xunit.Abstractions;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// Runs approved requests through the real ProvisioningEngine / RequestExecutor / form executor for a
    /// test-owned In-house endpoint, with the mock connector. In-house systems are "non-connected": the
    /// executor records the account in the database instead of calling a target, so these tests assert on
    /// request status and the account/permission rows, plus the connector's Connect check.
    /// </summary>
    [Collection(MockedCollection.Name)]
    public class ProvisioningTests
    {
        private readonly ITestOutputHelper output;
        public ProvisioningTests(ITestOutputHelper output) { this.output = output; }

        private static void Run(long requestId) => new ProvisioningEngine().StartProvisioningTest(requestId);

        private void Dump(TestWorld w)
        {
            foreach (var c in MockConnector.CallsFor(w.EndpointId)) output.WriteLine("connector: " + c);
        }

        private static (bool Deleted, bool Disabled, bool Locked) AccountState(long accountId)
        {
            var r = TestDb.Row("SELECT IsDeleted, IsDisabled, IsLocked FROM DI_INH_UserInfo WHERE Id = @id", ("@id", accountId));
            return ((bool)r["IsDeleted"], (bool)r["IsDisabled"], (bool)r["IsLocked"]);
        }

        private static int PermissionCount(long accountId, long endpointId) =>
            (int)TestDb.ScalarLong("SELECT COUNT(*) FROM DI_INH_UserPermissionFunction WHERE EndpointUserId = @a AND EndPointSystemId = @ep", ("@a", accountId), ("@ep", endpointId));

        [Fact]
        public void Adhoc_lifecycle_create_add_remove_delete()
        {
            using (var w = new TestWorld())
            {
                w.CreateInhouseEndpoint();
                string permission = w.CreatePermission();

                // 1. New account
                long create = w.CreateRequest(AccessRequestSource.AdhocRequest, AdHocActionType.NewAccount, 0, permission);
                Run(create);
                Dump(w);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(create));
                long account = w.AccountId();
                Assert.NotEqual(0, account);
                Assert.False(AccountState(account).Deleted);
                Assert.Contains(MockConnector.CallsFor(w.EndpointId), c => c.Method == "Connect" && c.RequestId == create);

                // 2. Add a permission
                long add = w.CreateRequest(AccessRequestSource.AdhocRequest, AdHocActionType.AmendmentAdd, account, permission);
                Run(add);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(add));
                Assert.Equal(1, PermissionCount(account, w.EndpointId));

                // 3. Remove it again
                long remove = w.CreateRequest(AccessRequestSource.AdhocRequest, AdHocActionType.AmendmentRemove, account, permission);
                Run(remove);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(remove));
                Assert.Equal(0, PermissionCount(account, w.EndpointId));

                // 4. Delete the account
                long delete = w.CreateRequest(AccessRequestSource.AdhocRequest, AdHocActionType.DeleteAccount, account, permission);
                Run(delete);
                Dump(w);
                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(delete));
                Assert.True(AccountState(account).Deleted);
            }
        }

        [Fact]
        public void Offboarding_removes_the_account()
        {
            using (var w = new TestWorld())
            {
                w.CreateInhouseEndpoint();
                string permission = w.CreatePermission();
                long create = w.CreateRequest(AccessRequestSource.AdhocRequest, AdHocActionType.NewAccount, 0, permission);
                Run(create);
                long account = w.AccountId();
                Assert.NotEqual(0, account);

                long offboard = w.CreateRequest(AccessRequestSource.OffBoarding, AdHocActionType.DeleteAccount, account, permission, isOffBoardRequest: true);
                Run(offboard);
                Dump(w);

                Assert.Equal(AccessRequestProgress.Completed, w.RequestProgress(offboard));
                Assert.True(AccountState(account).Deleted);
            }
        }

        public static TheoryData<AccessRequestSource> EventSources() => new TheoryData<AccessRequestSource>
        {
            AccessRequestSource.OnBoarding,
            // Rehire: see Rehire_request_is_dispatched (skipped) below.
            AccessRequestSource.OUChange,
            AccessRequestSource.ReOrganisation,
            AccessRequestSource.Propagation,
            AccessRequestSource.NonEmployeeExtension,
            AccessRequestSource.UndoEvent,
            AccessRequestSource.AttestationRemediation,
        };

        /// <summary>
        /// Every HR / lifecycle event is dispatched and finishes (no request left Ready or Executing),
        /// with nothing sent to a real target. The resulting status is written to the test output.
        /// </summary>
        [Theory]
        [MemberData(nameof(EventSources))]
        public void Lifecycle_event_is_dispatched_and_finishes(AccessRequestSource source)
        {
            using (var w = new TestWorld())
            {
                w.CreateInhouseEndpoint();
                string permission = w.CreatePermission();
                long create = w.CreateRequest(AccessRequestSource.AdhocRequest, AdHocActionType.NewAccount, 0, permission);
                Run(create);
                long account = w.AccountId();
                Assert.NotEqual(0, account);

                long request = w.CreateRequest(source, AdHocActionType.AmendmentAdd, account, permission);
                Run(request);

                var status = w.RequestProgress(request);
                output.WriteLine($"{source}: {status}");
                Dump(w);
                Assert.NotEqual(AccessRequestProgress.ReadytoExecute, status);
                Assert.NotEqual(AccessRequestProgress.Executing, status);
            }
        }

        /// <summary>
        /// Known gap, skipped on request: RequestExecutor (RequestExecutor.cs:45-100) has no branch for
        /// RequestSource.Rehire, so a Rehire request is set to Executing and never processed. Production
        /// currently creates rehire requests with RequestSource.OnBoarding (ProvisioningFlowHelper.cs:1170),
        /// so whether Rehire requests should exist at all is an open decision. Remove Skip once decided.
        /// </summary>
        [Fact(Skip = "Known gap: RequestExecutor has no Rehire branch (request stays Executing). Skipped pending a decision on Rehire handling.")]
        public void Rehire_request_is_dispatched_and_finishes() => Lifecycle_event_is_dispatched_and_finishes(AccessRequestSource.Rehire);

        [Fact]
        public void Request_fails_and_creates_nothing_when_the_target_is_unreachable()
        {
            using (var w = new TestWorld())
            {
                w.CreateInhouseEndpoint();
                string permission = w.CreatePermission();
                MockConnector.FailOn.Add("Connect");

                long create = w.CreateRequest(AccessRequestSource.AdhocRequest, AdHocActionType.NewAccount, 0, permission);
                Run(create);

                Assert.Equal(AccessRequestProgress.ExecutionFailed, w.RequestProgress(create));
                Assert.Equal(0, w.AccountId());
            }
        }

        [Fact]
        public void Request_does_not_stay_executing_when_the_connector_crashes()
        {
            using (var w = new TestWorld())
            {
                w.CreateInhouseEndpoint();
                string permission = w.CreatePermission();
                MockConnector.ThrowOn.Add("Connect");

                long create = w.CreateRequest(AccessRequestSource.AdhocRequest, AdHocActionType.NewAccount, 0, permission);
                Run(create);

                var status = w.RequestProgress(create);
                output.WriteLine("status after crash: " + status);
                Assert.NotEqual(AccessRequestProgress.Completed, status);
                Assert.NotEqual(AccessRequestProgress.ReadytoExecute, status);
                Assert.NotEqual(AccessRequestProgress.Executing, status); // would never be retried or failed
                Assert.Equal(0, w.AccountId());
            }
        }
    }
}
