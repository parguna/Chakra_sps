
namespace DI.Chakra.Infrastructure.Enums
{
    public enum EmployeeEvents : byte
    {
        [Description("None")]
        None = 0,

        [Description("New Joinee")]
        NewJoinee = 1,

        [Description("Re-Hire")]
        Rehire = 2,

        [Description("Transfer")]
        OUChange = 3,

        [Description("Resignation")]
        Resignation = 6,

        [Description("Adhoc Request")]
        AdhocRequest = 7,

        [Description("Re-Organisation")]
        ReOrganisation = 8,

        [Description("Propagation")]
        Propagation = 9,

        [Description("Post Resignation")]
        PostResignation = 10,

        [Description("Non Employee Creation")]
        NonEmployeeCreation = 11,

        [Description("Non Employee Extension / Shortening")]
        NonEmployeeExtension = 12,

        [Description("EmployeeConversion")]
        EmployeeConversion = 17,

        [Description("Promotion")]
        Promotion = 18
    }
}
