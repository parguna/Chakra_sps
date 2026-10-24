
namespace DI.Chakra.Infrastructure.Enums
{
    public enum EnvironmentType : byte
    {
        [Description("None")]
        None = 0,

        [Description("Production")]
        Production,

        [Description("QA")]
        QA,

        [Description("Development")]
        Development,
       
    }
}
