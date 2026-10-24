namespace DI.Chakra.Infrastructure.Enums
{
    public enum SfBActionType : byte
    {
        None = 0,
        OnPremToOnline = 1,
        OnlineToOnPrem = 2,
    }

    public enum SharedMailBoxActionType : byte
    {
        None = 0,
        MemberAddition = 1,
        MemberRemoval = 2,
    }

    public enum MailBoxConversionType : byte
    {
        None = 0,
        [Description("Convert to Regular")]
        SharedToUser = 1,
        [Description("Convert to Shared")]
        UserToShared = 2,
    }

    public enum ExternalEmailActionType : byte
    {
        None = 0,
        Enable = 1,
        Disable = 2,
    }

    public enum LicenseActionType : byte
    {
        None = 0,
        Add = 1,
        Remove = 2,
    }

}
