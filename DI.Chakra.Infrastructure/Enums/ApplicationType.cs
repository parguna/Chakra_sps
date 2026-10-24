
namespace DI.Chakra.Infrastructure.Enums
{
    public enum ApplicationType : byte
    {

        [Description("None")]
        None = 0,

        [Exclude("No")]
        [Description("SingaporePower.Local")]
        [EndpointType("1")]
        ADSingaporePowerLocal = 1,

        [Exclude("No")]
        [Description("SAP.Local")]
        [EndpointType("1")]
        SAPLocal = 2,

        [Exclude("No")]
        [Description("HRMS FLINS")]
        [EndpointType("3")]
        HRMS_FLINS = 3,

        [Exclude("No")]
        [Description("FLINS BI")]
        [EndpointType("3")]
        FLINS_BI = 4,

        [Exclude("No")]
        [Description("SAP PI")]
        [EndpointType("3")]
        SAP_PI = 5,

        [Exclude("No")]
        [Description("SAP GRC")]
        [EndpointType("3")]
        SAP_GRC = 6,

        [Exclude("No")]
        [Description("MSSL ISU")]
        [EndpointType("3")]
        MSSL_ISU = 7,

        [Exclude("No")]
        [Description("MSSL CRM")]
        [EndpointType("3")]
        MSSL_CRM = 8,

        [Exclude("No")]
        [Description("EBS ISU")]
        [EndpointType("3")]
        EBS_ISU = 9,

        [Exclude("No")]
        [Description("EBS CRM")]
        [EndpointType("3")]
        EBS_CRM = 10,

        [Exclude("Yes")]
        [Description("SRP")]
        [EndpointType("3")]
        SAP_SRP = 11,

        [Exclude("Yes")]
        [Description("SUS")]
        [EndpointType("3")]
        SAP_SUS = 12,


        [Exclude("No")]
        [Description("SAP EP")]
        [EndpointType("4")]
        SAP_EP = 13,

        [Exclude("No")]
        [Description("CE")]
        [EndpointType("4")]
        CE = 14,

        [Exclude("No")]
        [Description("NPAC/NMACS/CIMS/EDMS")]
        [EndpointType("6")]
        NPAC = 15,

        [Exclude("No")]
        [Description("GEMS")]
        [EndpointType("7")]
        GEMS = 16,

        [Exclude("No")]
        [Description("VPN")]
        [EndpointType("8")]
        VPN = 17,

        [Exclude("No")]
        [Description("Office 365")]
        [EndpointType("11")]
        Office365 = 18,

        [Exclude("No")]
        [Description("OneCard")]
        [EndpointType("12")]
        OneCard = 19,

        [Exclude("No")]
        [Description("SRM")]
        [EndpointType("13")]
        SRM = 20,

        [Exclude("No")]
        [Description("Other Systems")]
        [EndpointType("9")]
        InhouseSystems = 21,

        [Exclude("No")]
        [Description("SDC.Local")]
        [EndpointType("1")]
        ADSDCLocal = 24,

        [Exclude("No")]
        [Description("IDMS")]
        [EndpointType("26")]
        IDMS = 22,

        [Exclude("No")]
        [Description("SSIDS_GUI")]
        [EndpointType("28")]
        SSIDS_GUI = 23,

        [Exclude("No")]
        [Description("EAM")]
        [EndpointType("29")]
        EAM = 25,

        [Exclude("No")]
        [Description("SSIDS_SMS")]
        [EndpointType("28")]
        SSIDS_SMS = 26,
        [Exclude("No")]
        [Description("MicrosoftTeams")]
        [EndpointType("30")]
        MicrosoftTeams = 30,

        [Exclude("No")]
        [Description("SuccessFactors")]
        [EndpointType("31")]
        SuccessFactors = 31,

        [Exclude("No")]
        [Description("EBS")]
        [EndpointType("40")]
        EBS = 41,
        [Exclude("No")]
        [Description("MSSL")]
        [EndpointType("40")]
        MSSL = 42,
    }
}
