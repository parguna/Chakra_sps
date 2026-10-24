
namespace DI.Chakra.Infrastructure.Enums
{
    public enum MappingType : byte
    {
        [Description("None")]
        None = 0,

        [Description("Automatically Mapped")]
        Auto = 1,

        [Description("Manually Mapped")]
        Manual = 2,

    }
}
