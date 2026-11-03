using System;
using System.Linq;
using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using Xunit;

namespace DI.Chakra.MockedTests
{
    [Collection(MockedCollection.Name)]
    public class ConnectorFactoryHookTests : IDisposable
    {
        public ConnectorFactoryHookTests() { MockConnector.Reset(); MockConnector.Install(); }
        public void Dispose() { MockConnector.Uninstall(); MockConnector.Reset(); }

        public static TheoryData<EndpointType> AllEndpointTypes()
        {
            var data = new TheoryData<EndpointType>();
            foreach (EndpointType t in Enum.GetValues(typeof(EndpointType)))
                data.Add(t);
            return data;
        }

        [Theory]
        [MemberData(nameof(AllEndpointTypes))]
        public void Every_endpoint_type_gets_the_mock_instead_of_a_real_connector(EndpointType type)
        {
            // No DB lookup happens: the override answers before the factory touches anything.
            var endpoint = new EndPoint { Id = -1, DisplayName = "hook-test", EndPointSystemType = new EndPointSystemType { EndpointSystemTypeName = type } };

            ConnectorBase connector = ConnectorFactory.GetConnector(endpoint, null, new DI.Chakra.Logger.ProvisioningLogger(null, -1, (long)type, "hook-test"));

            Assert.IsType<MockConnector>(connector);
            Assert.True(connector.Connect());
            Assert.Contains(MockConnector.Calls, c => c.Method == "Connect" && c.EndpointId == -1);
        }

        [Fact]
        public void Without_the_hook_the_factory_is_unchanged()
        {
            MockConnector.Uninstall();
            Assert.Null(ConnectorFactory.TestConnectorOverride);
        }
    }
}
