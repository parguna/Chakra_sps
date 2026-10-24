
namespace DI.Chakra.Infrastructure.Enums
{
    public enum PasswordActionType : byte
    {
        [Description("None")]
        None = 0,

        [Description("Change Password")]
        ChangePassword,

        [Description("Reset Password")]
        ResetPassword,

        [Description("Unlock Account")]
        UnlockAccount,

        [Description("Enable Account")]
        EnableAccount
    }
}
