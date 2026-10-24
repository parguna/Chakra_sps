
namespace DI.Chakra.Infrastructure.Enums
{
    public enum ACLAction
    {
        View,
        Add,
        Edit,
        Delete,
        Save,
        Detail,
        AccessRights,
        RunNow,
        Approve,
        Reject,
        PagePermission,
        Clone,
        Export,
        GeneratePair,
        Delegate,
        Upload,
        DefaultRoleSettings,
        ReSubmit,
        CSV,
        Excel,
        ExcelAll,
        CSVAll,
        Withdraw,
        Download,
        MarkAsRead,
        Reply,
        Schedule,
        ScheduleHistory,
        ViewSchedule,
        EditSchedule,
        Submit,
        Revert,
        Remark,
        AttestationView,
        AttestationReport,
        UserView,
        AccessView,
        ExportFlatReport,
        ExportHierarchyReport,
        AttestationDetail,
        AlertActions,
        RequestExtend,
        ExtendNonEmployee,
        RehireNonEmployee,
        ManuallyResolved
    }

    public enum UIMode
    {
        View,
        Add,
        Edit,
        Clone
    }

    public enum Product
    {
        IACM,
        IM,
        PWDManagement,
        Compliance,
        PIM,
        DGM
    }

    public enum IsAdminMode
    {
        Yes,
        No
    }


    public enum DisplayControl
    {
        ApplicationType,
        Application,
        Both,
    }

    public enum EndpointKey
    {
        IPAddress = 1,
        DomainName = 2,
        UserName = 3,
        Password = 4,
        Ou = 5,
        MailDomain = 6,
        SystemNumber = 7,
        ClientNumber = 8,
        CanRequestMultipleRoles = 9,
        HostName = 10,
        InstanceName = 11,
        PortNo = 12,
        ServerName = 13,
        DatabaseName = 14,
        WSDLURL = 15,
        IsWindowsAuthendication = 16,
        DirectorySyncServerName = 17,
        RootConnectionString = 18,
        SSL = 19,
        IsWOG = 20,
        IsOneWayTrust = 21,
        TrustSource = 22,
        WebAPI = 27,
        Instruction = 28,
        SecondaryIPAddress = 29,
        UserFilePath = 30,
        RoleFilePath = 31,
        FunctionFilePath = 32,
        PrivillegeAcctTransFilePath = 33,
        BizServiceUserName = 34,
        BizServicePassword = 35,
        Finance = 36,
        Procurement = 37,
        UserRoleFilePath = 38,
        FinanceFilePath = 39,
        ProcurementFilePath = 40,
        CostCenterGroupFilePath = 41,
        Role2FunctionFilePath = 42,
        IMUserName = 43,
        IMUserPassword = 44,
        OneTimeUserListFilePath = 45,
        // DirectorySyncServerName= 46,
        CanRunDiscovery = 47,
        ConnectionUri = 50,
        AzueADUserName = 51,
        AzureADUserPassword = 52,
        ////IsWindowsAuthendication = 48,
        // RootConnectionString = 49
        SNCMode = 53,
        SNCQop = 54,
        SNCLib = 55,
        SNCPartnerName = 56,
        SF_Company = 57,

        EnableSSO=70,
        SNCNamePrefix = 71,
        SNCNameSuffix = 72,
        SNCParameterName = 73,
        IsPaginationEnabled = 74, //Added
    }
}