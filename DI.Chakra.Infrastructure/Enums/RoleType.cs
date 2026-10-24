
namespace DI.Chakra.Infrastructure.Enums
{
    public enum RoleType : byte
    {
        [Description("None")]
        None = 0,

        [Description("User")]
        User = 1,

        [Description("Manager")]
        Manager = 2,

        [Description("Supervisor")]
        RO = 3,

        [Description("Countersigning Officer")]
        CO = 4,

        [Description("Application Owner")]
        ApplicationOwner = 5,

        [Description("Role Owner - Level 1")]
        Level1Approver = 6,

        [Description("Role Owner - Level 2")]
        Level2Approver = 7,

        [Description("Role Owner - Level 3")]
        Level3Approver = 8,

        [Description("Critical T-Code Owner")]
        TCodeOwner = 9,

        [Description("Group Owner")]
        GroupOwner = 10,

        [Description("Chief Information Officer / Digital Officer")]
        CIO = 11,

        [Description("Head of Section")]
        HeadOfSection = 12,

        [Description("Head of Branch")]
        HeadOfBranch = 13,

        [Description("Head of Department")]
        HeadOfDepartment = 14,

    }
}
