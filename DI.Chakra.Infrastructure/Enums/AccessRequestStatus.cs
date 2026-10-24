
namespace DI.Chakra.Infrastructure.Enums
{
    public enum AccessRequestStatus : long
    {
        [DisplayAs("None")]
        [Description("None")]
        None = 0,

        [DisplayAs("Requested")]
        [Description("Requested")]
        Requested = 1,

        [DisplayAs("Waiting for next level Approval")]
        [Description("Approved")]
        Approved = 2,

        [DisplayAs("Rejected")]
        [Description("Rejected")]
        Rejected = 3,

        [DisplayAs("Approved")]
        [Description("Granted")]
        Granted = 4,

        [DisplayAs("Withdrawn")]
        [Description("Withdrawn")]
        Withdrawn = 5,

        [DisplayAs("Submitted")]
        [Description("Submitted")]
        Submitted = 6,
    }
}
