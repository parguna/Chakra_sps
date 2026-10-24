
namespace DI.Chakra.Infrastructure.Enums
{
    public enum BusinessLogicSource : byte
    {

        [Description("None")]
        None = 0,

        [Description("Ruleset")]
        Ruleset = 1,

        [Description("External Code")]
        ExternalCode = 2,

    }
}
