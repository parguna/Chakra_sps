
namespace DI.Chakra.Infrastructure.Enums
{
    public enum LockScenario : byte
    {
        [Description("None")]
        None = 0,

        [Description("Off Board")]
        OffBoard = 1,

        [Description("OU Change")]
        OUChange = 2,

        [Description("Adhoc Request")]
        AdhocRequest = 3,

        //[Description("Position Change")]
        //PositionChange = 4,

        [Description("Attestation Remediation")]
        AttestationRemediation = 5
    }
}
