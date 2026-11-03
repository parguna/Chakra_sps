using System.Linq;
using DI.Chakra.DomainModel;
using DI.Chakra.Services.DiscoveryProcessor;
using Xunit;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// Runs the real DiscoveryService for a test-owned In-house endpoint with the mock connector.
    /// </summary>
    [Collection(MockedCollection.Name)]
    public class DiscoveryTests
    {
        private static Process LoadProcess(long id) => Process.GetById(id);

        private static (string Status, string Error, int History, string HistoryStatus) Outcome(TestWorld w)
        {
            var p = TestDb.Row("SELECT Status, ErrorMessage FROM DI_M_Process WHERE Id = @p", ("@p", w.ProcessId));
            var h = TestDb.Row("SELECT COUNT(*) n, MAX(Status) s FROM DI_M_ProcessHistroy WHERE ProcessId = @p", ("@p", w.ProcessId));
            return (p["Status"] as string, p["ErrorMessage"] as string, System.Convert.ToInt32(h["n"]), h["s"] as string);
        }

        [Fact]
        public void Successful_discovery_connects_pulls_data_and_completes()
        {
            using (var w = new TestWorld())
            {
                w.CreateDiscoveryJob();

                new DiscoveryService(LoadProcess(w.ProcessId)).Start();

                var calls = MockConnector.CallsFor(w.EndpointId).Select(c => c.Method).ToList();
                Assert.Equal(new[] { "Connect", "GetData" }, calls.Where(m => m == "Connect" || m == "GetData").ToArray());

                var o = Outcome(w);
                Assert.Equal("Completed", o.Status);
                Assert.Equal(1, o.History);
                Assert.Equal("Success", o.HistoryStatus);
            }
        }

        [Fact]
        public void Discovery_fails_cleanly_when_the_target_cannot_be_reached()
        {
            using (var w = new TestWorld())
            {
                w.CreateDiscoveryJob();
                MockConnector.FailOn.Add("Connect");

                new DiscoveryService(LoadProcess(w.ProcessId)).Start();

                Assert.DoesNotContain(MockConnector.CallsFor(w.EndpointId), c => c.Method == "GetData");
                var o = Outcome(w);
                Assert.Equal("Failed", o.Status);
                Assert.Contains("Failed to connect", o.Error ?? "");
                Assert.Equal(1, o.History);
            }
        }

        [Fact]
        public void Discovery_records_failure_when_the_connector_crashes_while_reading()
        {
            using (var w = new TestWorld())
            {
                w.CreateDiscoveryJob();
                MockConnector.ThrowOn.Add("GetData");

                new DiscoveryService(LoadProcess(w.ProcessId)).Start();

                var o = Outcome(w);
                Assert.Equal("Failed", o.Status);
                Assert.Contains("simulated failure in GetData", o.Error ?? "");
                Assert.Equal(1, o.History);
            }
        }

        [Fact]
        public void Discovery_skips_inactive_endpoints_without_calling_the_connector()
        {
            using (var w = new TestWorld())
            {
                w.CreateDiscoveryJob();
                TestDb.Execute("UPDATE DI_M_EndpointSystem SET IsActive = 0 WHERE Id = @ep", ("@ep", w.EndpointId));

                new DiscoveryService(LoadProcess(w.ProcessId)).Start();

                Assert.Empty(MockConnector.CallsFor(w.EndpointId));
            }
        }
    }
}
