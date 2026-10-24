
namespace DI.Chakra.Infrastructure.Enums
{
    public enum AccessRequestMode : long
    {
        [DisplayAs("Consolidate")]
        [Description("Consolidate")]
        Consolidate = 0,

        [DisplayAs("SeparateAccountAndRole")]
        [Description("SeparateAccountAndRole")]
        SeparateAccountAndRole = 1,

        [DisplayAs("SeparateEachRole")]
        [Description("SeparateEachRole")]
        SeparateEachRole = 2,
        
    }
}
