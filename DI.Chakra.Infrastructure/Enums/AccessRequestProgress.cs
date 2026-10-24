
namespace DI.Chakra.Infrastructure.Enums
{
    public enum AccessRequestProgress : long
    {
        [Description("None")]
        None = 0,

        [Description("Waiting for Approval")]
        WaitingforApproval = 1,

        [Description("Request Rejected")]
        RequestRejected = 2,

        [Description("Ready to Execute")]
        ReadytoExecute = 3,

        [Description("Executing")]
        Executing = 4,

        [Description("Completed")]
        Completed = 5,

        [Description("Execution Failed")]
        ExecutionFailed = 6,

        [Description("Validation Failed")]
        ValidationFailed = 7,

        [Description("Withdrawn")]
        Withdrawn = 8,

        [Description("Submitted")]
        Submitted = 9,

        [Description("Manually Resolved")]
        ManuallyResolved = 10,

        [Description("Waiting For Sync")]
        WaitingForSync = 11,
        [Description("Waiting For License Assignment")]
        WaitingForLicenseAssignment = 12,
        [Description("Waiting For MailBox Creation")]
        WaitingForMailBoxCreation = 13,

        [Description("Application Workflow Not Found")]
        WorkflowNotFound = 20
    }
}
