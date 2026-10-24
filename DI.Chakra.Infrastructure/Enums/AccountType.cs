
namespace DI.Chakra.Infrastructure.Enums
{
    public enum AccountType : byte
    {
        [Description("None")]
        None = 0,

        [Description("User Account")]
        User = 1,

        [Description("Service Account")]
        Service = 2,

        [Description("Shared Account")]
        Shared = 3,

        [Description("Secondary Account")]
        Secondary = 4 
    }

    public enum ActionType : byte
    {
        [Description("New Account")]
        NewAccount = 1,

        [Description("Amendment Add")]
        AmendmentAdd = 2,

        [Description("Amendment Remove")]
        AmendmentRemove = 3,

        [Description("Delete Account")]
        DeleteAccount = 4,

        [Description("RenewalAccess")]
        RenewalAccess = 5,

    }
}
