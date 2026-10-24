namespace DI.Chakra.Infrastructure.Enums
{
    public enum StatusType : byte
    {
        None = 0,
        [Description("Pending")]
        Pending = 1,

        [Description("Approved")]
        Approved,

        [Description("Rejected")]
        Rejected,

        [Description("Auto Completed")]
        AutoCompleted,

        [Description("Not Initiated")]
        NotInitiated,

        [Description("Withdrawn")]
        Withdrawn,
    }
}
