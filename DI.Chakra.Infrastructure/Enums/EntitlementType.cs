namespace DI.Chakra.Infrastructure.Enums
{
    public enum EntitlementType
    {
        [Description("None")]
        None = 0,

        [Description("Role")]
        Role,

        [Description("Group")]
        Group,

        [Description("Profile")]
        Profile,

        [Description("Parameter")]
        Parameter,

        [Description("Position")]
        Position,

        [Description("Permission")]
        Permission,

        [Description("Level")]
        Level,

        [Description("TCodes")]
        TCodes,
        [Description("Responsibility")]
        Responsibility,

        [Description("Catalog")]
        Catalog,

        [Description("Schema")]
        Schema,

        //[Description("Roles")]
        //Roles,

        [Description("Access Level")]
        AccessLevel,

        [Description("Function")]
        Function,

        [Description("Account")]
        Account,

        [Description("Member")]
        Member,

        [Description("MemberOf")]
        MemberOf,

        [Description("Authorization")]
        Authorization,

        [Description("MailQuota")]
        MailQuota,

        [Description("License")]
        License,

        [Description("Badge")]
        Badge,

        [Description("HandPhone")]
        HandPhone,
        [Description("Application")]
        Application=23,
        [Description("Project")]
        Project=24,
        [Description("Section")]
        Section = 25,
        [Description("Default")]
        Default = 26,
        [Description("DCC")]
        DCC = 27,
        [Description("DCCS")]
        DCCS = 28,
        [Description("IDD")]
        IDD = 29,
        [Description("ResponseGroup")]
        ResponseGroup = 30,
        [Description("Move")]
        Move = 31,
       [Description("Business Role")]
       BusinessRole = 41,
       [Description("Technical Role")]
       TechnicalRole = 42,
       [Description("Indirect Role")]
       IndirectRole = 43
    }
}
