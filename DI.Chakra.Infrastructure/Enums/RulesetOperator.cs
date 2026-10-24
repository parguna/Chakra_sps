
namespace DI.Chakra.Infrastructure.Enums
{
    public enum RulesetOperator : byte
    {
        [Description("Any")]
        Any = 0,

        [Description("Equals")]
        Equals = 1,        
    }

    public enum RulesetType : byte
    {
        [Description("Organisation")]
        Organisation = 1,

        [Description("Designation")]
        Designation = 2,

        [Description("EmployeeType")]
        EmployeeType = 3,

        [Description("EventTrigger")]
        EventTrigger = 4,

        [Description("Grade")]
        Grade = 5,

        [Description("Company")]
        Company = 6,

        [Description("BusinessUnit")]
        BusinessUnit = 7,

        [Description("CostCenter")]
        CostCenter = 8,

        [Description("Location")]
        Location = 9,
    }
}
