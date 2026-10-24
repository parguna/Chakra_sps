
namespace DI.Chakra.Infrastructure.Enums
{
    public enum AccessRequestSource : long
    {

        [Description("None")]
        None = 0,

        [Description("On-Boarding")]
        OnBoarding = 1,

        [Description("Off-Boarding")]
        OffBoarding = 2,

        [Description("OU Change")]
        OUChange = 3,

        [Description("Adhoc Request")]
        AdhocRequest = 4,

        [Description("Re-Organisation")]
        ReOrganisation = 5,

        [Description("Propagation")]
        Propagation = 6,

        [Description("Employee Extension")]
        EmployeeExtension = 7,

        [Description("Attestation Remediation")]
        AttestationRemediation = 8,

        [Description("Employee Creation")]
        EmployeeCreation = 9,


        [Description("Re-Hire")]
        Rehire = 10,

        [Description("Promotion")]
        Promotion = 11,

        [Description("ExtendedRequest")]
        ExtendedRequest = 12,

        [Description("GamingApplicationRemoval")]
        GamingApplicationRemoval = 13,

        [Description("Off-BoardingDay2")]
        OffBoardingDay2 = 14,

        [Description("Off-BoardingDay3")]
        OffBoardingDay3 = 15,

        [Description("Demotion")]
        Demotion = 16,

        [Description("UpdateADAccount")]
        UpdateADAccount = 17,

        [Description("EmployeeConversion")]
        EmployeeConversion = 18,

        [Description("Undo Event")]
        UndoEvent = 19,

        [Description("Non Employee Extension/Shortening")]
        NonEmployeeExtension = 20,
    }
}
