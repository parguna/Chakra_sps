
namespace DI.Chakra.Infrastructure.Enums
{
    public enum RequestType : byte
    {
        [Description("None")]
        None = 0,
        [Description("Access Review")]
        Attestation = 1,

        [Description("Provisioning")]
        Provisioning = 2,

        [Description("Password Sync")]
        PasswordSync = 3,

        [Description("Compliance")]
        Compliance = 4,

        [Description("Non Employee")]
        EmployeeEvent = 5,

        [Description("Bulk Request")]
        BulkRequest = 6,

    }
}
