using Xunit;

namespace DI.Chakra.MockedTests
{
    /// <summary>
    /// ConnectorFactory.TestConnectorOverride and MockConnector's call log are static, so every test
    /// that uses them runs in this one collection (xUnit runs collections in parallel, tests within a
    /// collection one at a time).
    /// </summary>
    [CollectionDefinition(Name, DisableParallelization = true)]
    public class MockedCollection
    {
        public const string Name = "Mocked connector";
    }
}
