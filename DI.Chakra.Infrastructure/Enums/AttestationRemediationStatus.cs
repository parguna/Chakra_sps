
namespace DI.Chakra.Infrastructure.Enums
{
    public enum AttestationRemediationStatus : byte
    {

        [Description("None")]
        None = 0,

        [Description("Not Required")]
        NotRequired = 1,

        [Description("Pending")]
        Pending = 2,

        [Description("Completed")]
        Completed = 3,

    }
}
