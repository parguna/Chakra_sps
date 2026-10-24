
namespace DI.Chakra.Infrastructure.Enums
{
    public enum EmployeeType : byte
    {
        //[Description("None")]
        //None = 0,

        [Description("Permanent")]
        Permanent = 1,

        [Description("Vendor")]
        Vendor = 2,

        [Description("Third Party Contractor")]
        Contractor = 3,

        //[Description("Intern")]
        //Intern = 4,

        [Description("Temp or Contact Staff")]
        TempStaff = 4,

        //[Description("SDC User")]
        //SDCUser = 6,

        //[Description("SP Club")]
        //SPClub = 7,
    }

}
