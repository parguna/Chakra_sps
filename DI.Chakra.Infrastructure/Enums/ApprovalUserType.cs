
namespace DI.Chakra.Infrastructure.Enums
{
    public enum ApprovalUserType : byte
    {
        [Description("None")]
        None = 0,

        [Description("User")]
        User = 1,

        [Description("Role")]
        Role = 2,

        [Description("Group")]
        Group = 3,

        [Description("Use Application Configuration")]
        UseApplicationConfiguration = 4,
    }
}
