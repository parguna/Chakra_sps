
namespace DI.Chakra.Infrastructure.Enums
{
    public enum EndpointType : int
    {

        None = 0,

        [Description("Active Directory")]
        ActiveDirectory = 1,

        //[Description("Lotus Notes")]
        //LotusNotes = 2,

        [Description("SAP")]
        SAP = 3,

        [Description("Portals (EP / CE)")]
        SAPEnterprisePortal = 4,

        [Description("NPAC/NMACS/CIMS/EDMS")]
        NPAC = 6,

        [Description("GEMS")]
        GEMS = 7,

        [Description("VPN")]
        VPN = 8,

        [Description("Other Systems")]
        InhouseSystems = 9,

        [Description("Office 365")]
        Office365 = 11,

        [Description("OneCard")]
        OneCard = 12,

        [Description("SRM")]
        SRM = 13,

        [Description("IDMS")]
        IDMS = 26,

        //[Description("NOVA")]
        //NOVA = 27,


        [Description("SSIDS")]
        SSIDS = 28,

        [Description("EAM")]
        EAM_NOVA = 29,
        [Description("Microsoft Teams")]
        MicrosoftTeams = 30,

        [Description("SuccessFactor")]
        SuccessFactor = 31,
        [Description("SAP HANA")]
        SAPHANA = 40,
    }
}
