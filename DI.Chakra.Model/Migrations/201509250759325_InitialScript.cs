namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialScript : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_AccessRequestCategory",
                c => new
                    {
                        AccessRequestTypeId = c.Byte(nullable: false),
                        AccessRequestTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AccessRequestTypeId);
            
            CreateTable(
                "dbo.DI_M_AccessRequestHistory",
                c => new
                    {
                        AccessRequestHistoryId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Int(nullable: false),
                        TenantId = c.String(),
                        RequesterId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Description = c.String(),
                        ProvisioningRoleId = c.Int(),
                        RequestStatus = c.Byte(nullable: false),
                        ProgressStatus = c.Byte(nullable: false),
                        RequestSource = c.Byte(nullable: false),
                        ActivationDate = c.DateTime(),
                        DeactivationDate = c.DateTime(),
                        ExecutionTime = c.DateTime(),
                        Remarks = c.String(),
                        ApproverType = c.String(),
                        IsRequestedToRemove = c.Boolean(),
                        IsNewUser = c.Boolean(),
                        NewUserId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsCurrentLevelApproved = c.Boolean(nullable: false),
                        TransactionId = c.Guid(nullable: false),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.AccessRequestHistoryId);
            
            CreateTable(
                "dbo.DI_M_AccessRequest",
                c => new
                    {
                        AccessRequestId = c.Long(nullable: false, identity: true),
                        TenantId = c.String(),
                        RequesterId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Description = c.String(),
                        ProvisioningFormId = c.Long(),
                        AdHocAccessRequestId = c.Long(),
                        EndpointSystemId = c.Int(),
                        EmployeeEventTriggerId = c.Int(),
                        RequestType = c.Byte(nullable: false),
                        RequestStatus = c.Byte(nullable: false),
                        ProgressStatus = c.Byte(nullable: false),
                        RequestSource = c.Byte(nullable: false),
                        ActivationDate = c.DateTime(),
                        DeactivationDate = c.DateTime(),
                        ExecutionTime = c.DateTime(),
                        Remarks = c.String(),
                        ApproverType = c.String(),
                        IsRequestedToRemove = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        TransactionId = c.Guid(nullable: false),
                        IsCurrentLevelApproved = c.Boolean(nullable: false),
                        Comments = c.String(),
                        AdditionalInfo = c.String(),
                        ParentRequestId = c.Long(),
                        IsParentRequest = c.Boolean(nullable: false),
                        IsDependentRequest = c.Boolean(nullable: false),
                        IsDependentRequestCreated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AccessRequestId)
                .ForeignKey("dbo.DI_M_AdHocAccessRequest", t => t.AdHocAccessRequestId)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProvisioningFormId)
                .Index(t => t.AdHocAccessRequestId);
            
            CreateTable(
                "dbo.DI_M_AdHocAccessRequest",
                c => new
                    {
                        AdHocAccessRequestId = c.Long(nullable: false, identity: true),
                        TenantId = c.String(),
                        RequesterUserId = c.Int(nullable: false),
                        RequesteeUserId = c.Int(nullable: false),
                        EndpointSystemType = c.Int(nullable: false),
                        EndpointSystemId = c.Int(),
                        AdhocActionType = c.Byte(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        IsSelfRequest = c.Boolean(nullable: false),
                        IsSecondaryAccountRequest = c.Boolean(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        Comments = c.String(),
                        EntitlementDetail = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AdHocAccessRequestId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_EndpointSystem",
                c => new
                    {
                        EndpointSystemId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        TenantId = c.String(),
                        EndpointSystemTypeId = c.Int(),
                        DisplayName = c.String(),
                        OwnerUserId = c.Int(),
                        PopulateIacm = c.Boolean(),
                        MapIacm = c.Boolean(),
                        ApplicationCode = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsTrustedSource = c.Boolean(nullable: false),
                        IsPasswordResetAllowed = c.Boolean(nullable: false),
                        IncludeInAttestation = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.EndpointSystemId)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId)
                .ForeignKey("dbo.DI_M_User", t => t.OwnerUserId)
                .Index(t => t.EndpointSystemTypeId)
                .Index(t => t.OwnerUserId);
            
            CreateTable(
                "dbo.DI_M_EndpointSystemType",
                c => new
                    {
                        EndpointSystemTypeId = c.Int(nullable: false, identity: true),
                        EndpointSystemTypeName = c.Int(nullable: false),
                        DisplayName = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.EndpointSystemTypeId);
            
            CreateTable(
                "dbo.DI_M_EndpointObjects",
                c => new
                    {
                        EndpointObjectId = c.Int(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Int(nullable: false),
                        ObjectName = c.String(),
                    })
                .PrimaryKey(t => t.EndpointObjectId)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId, cascadeDelete: true)
                .Index(t => t.EndpointSystemTypeId);
            
            CreateTable(
                "dbo.DI_M_DiscoveryProcess",
                c => new
                    {
                        DiscoveryProcessId = c.Int(nullable: false, identity: true),
                        ProcessName = c.String(),
                        Description = c.String(),
                        Endpointsystemid = c.Int(nullable: false),
                        Default = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        DisableDeltaDiscovery = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DiscoveryProcessId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.Endpointsystemid, cascadeDelete: true)
                .Index(t => t.Endpointsystemid);
            
            CreateTable(
                "dbo.DI_M_AttributeMapping",
                c => new
                    {
                        Rowid = c.Int(nullable: false, identity: true),
                        DiColumn = c.String(),
                        Tgcolumn = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreateDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndPoint_EndpointSystemId = c.Int(),
                        EndPointSystemType_EndpointSystemTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.Rowid)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPoint_EndpointSystemId)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndPointSystemType_EndpointSystemTypeId)
                .Index(t => t.EndPoint_EndpointSystemId)
                .Index(t => t.EndPointSystemType_EndpointSystemTypeId);
            
            CreateTable(
                "dbo.DI_M_User",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        NRIC = c.String(),
                        FIN = c.String(),
                        PassportNo = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        TenantId = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        DisplayName = c.String(),
                        KnownAs = c.String(),
                        EmployeeId = c.String(),
                        FullTimeEmployee = c.Boolean(),
                        EEGroup = c.String(),
                        EmployeeStatus = c.String(),
                        IsExecutiveEmployee = c.Boolean(nullable: false),
                        CompanyId = c.Int(),
                        DepartmentId = c.Int(),
                        DivisionId = c.Int(),
                        OrganisationId = c.Int(),
                        BusinessRoleId = c.Int(),
                        ManagerId = c.Int(),
                        EmailId = c.String(),
                        TelephoneNumber = c.String(),
                        HomephoneNumber = c.String(),
                        MobileNumber = c.String(),
                        Fax = c.String(),
                        PostalCode = c.String(),
                        PostalAddress = c.String(),
                        JoinDate = c.DateTime(),
                        RehireDate = c.DateTime(),
                        TerminationDate = c.DateTime(),
                        IsActive = c.Boolean(),
                        Ethinicity = c.String(),
                        Country = c.String(),
                        EmployeeType = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        CostCenter = c.String(),
                        CostCenterText = c.String(),
                        CostDistribution = c.String(),
                        CompanyCode = c.String(),
                        CompanyName = c.String(),
                        DepartmentCode = c.String(),
                        DepartmentName = c.String(),
                        DivisionCode = c.String(),
                        DivisionName = c.String(),
                        SectionCode = c.String(),
                        SectionName = c.String(),
                        PositionCode = c.String(),
                        PositionName = c.String(),
                        Supervisor = c.String(),
                        AuthenticationMode = c.String(),
                        OrgCompany = c.String(),
                        OrgDepartment = c.String(),
                        OrgDivision = c.String(),
                        OrgBranch = c.String(),
                        OrgSection = c.String(),
                        OneKeyTokenId = c.String(),
                        FLINSLoginId = c.String(),
                        Reserved1 = c.String(),
                        Reserved2 = c.String(),
                        Reserved3 = c.String(),
                        Reserved4 = c.String(),
                        Reserved5 = c.String(),
                        Reserved6 = c.String(),
                        Reserved7 = c.String(),
                        Reserved8 = c.String(),
                        Reserved9 = c.String(),
                        Reserved10 = c.String(),
                        NeedEmailUpdateToHR = c.Boolean(nullable: false),
                        NeedFLINSLoginIdUpdateToHR = c.Boolean(nullable: false),
                        LastLogonDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_Designation", t => t.BusinessRoleId)
                .ForeignKey("dbo.DI_M_Company", t => t.CompanyId)
                .ForeignKey("dbo.DI_M_Department", t => t.DepartmentId)
                .ForeignKey("dbo.DI_M_Division", t => t.DivisionId)
                .ForeignKey("dbo.DI_M_Organisation", t => t.OrganisationId)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.DivisionId)
                .Index(t => t.OrganisationId)
                .Index(t => t.BusinessRoleId);
            
            CreateTable(
                "dbo.DI_M_Designation",
                c => new
                    {
                        BusinessRoleId = c.Int(nullable: false, identity: true),
                        DesignationCode = c.String(),
                        DesignationDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BusinessRoleId);
            
            CreateTable(
                "dbo.DI_M_Company",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        CompanyCode = c.String(),
                        CompanyName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CompanyId);
            
            CreateTable(
                "dbo.DI_M_Department",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        DepartmentCode = c.String(),
                        DepartmentText = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.DI_M_Division",
                c => new
                    {
                        DivisionId = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(nullable: false),
                        TenantId = c.String(),
                        DivisionCode = c.String(),
                        DivisionText = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.DivisionId);
            
            CreateTable(
                "dbo.DI_M_Group",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        GroupDescription = c.String(),
                        GroupOwner = c.Int(),
                        IsDefaultGroup = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.GroupId)
                .ForeignKey("dbo.DI_M_User", t => t.GroupOwner)
                .Index(t => t.GroupOwner);
            
            CreateTable(
                "dbo.DI_M_Organisation",
                c => new
                    {
                        OrganisationId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        OrgUnitPrefix = c.String(),
                        OrganisationName = c.String(),
                        ParentOrganisation = c.String(),
                        Scope = c.String(),
                        Head = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OrganisationId);
            
            CreateTable(
                "dbo.DI_M_Role",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        RoleDescription = c.String(),
                        RoleOwner = c.Int(),
                        IsDefaultRole = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.DI_M_User", t => t.RoleOwner)
                .Index(t => t.RoleOwner);
            
            CreateTable(
                "dbo.DI_M_ProvisioningForm",
                c => new
                    {
                        ProvisioningFormId = c.Long(nullable: false, identity: true),
                        TenantId = c.String(),
                        IsCreateAccountForm = c.Boolean(),
                        FormName = c.String(),
                        FormDescritpion = c.String(),
                        EndpointSystemType = c.Int(),
                        EndpointSystemId = c.Int(),
                        DependentOnFormId = c.Int(),
                        FormOwnerUserId = c.Int(),
                        PostActivityList = c.String(),
                        Extra = c.String(),
                        AccountType = c.Byte(nullable: false),
                        FormExecutorTypeName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        ProvisioningPassword_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ProvisioningFormId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .ForeignKey("dbo.DI_M_ProvisioningPassword", t => t.ProvisioningPassword_Id)
                .Index(t => t.EndpointSystemId)
                .Index(t => t.ProvisioningPassword_Id);
            
            CreateTable(
                "dbo.DI_M_ProvisioningFormGeneralData",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProvisioningFormId = c.Long(nullable: false),
                        AttributeName = c.String(),
                        AttributeColumnName = c.String(),
                        IsGroup = c.Boolean(),
                        Remarks = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId, cascadeDelete: true)
                .Index(t => t.ProvisioningFormId);
            
            CreateTable(
                "dbo.DI_M_ProvisioningPassword",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PasswordExpression = c.String(),
                        PasswordLength = c.Int(),
                        NoOfUppercaseChar = c.Int(),
                        NoOfLowercaseChar = c.Int(),
                        NoOfDigitChar = c.Int(),
                        SpecialCharacter = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccessRightHistory",
                c => new
                    {
                        AccessRightHistoryId = c.Long(nullable: false, identity: true),
                        AccessRightsId = c.Int(nullable: false),
                        RoleId = c.Int(),
                        TabMenuId = c.Int(),
                        UserAction = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AccessRightHistoryId);
            
            CreateTable(
                "dbo.DI_M_AccessRights",
                c => new
                    {
                        AccessRightsId = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        PagePermissionId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AccessRightsId)
                .ForeignKey("dbo.DI_M_PagePermission", t => t.PagePermissionId)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.PagePermissionId);
            
            CreateTable(
                "dbo.DI_M_PagePermission",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PageId = c.Int(nullable: false),
                        PermissionId = c.Int(nullable: false),
                        UrlId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Page", t => t.PageId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_Permission", t => t.PermissionId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_PageUrl", t => t.UrlId)
                .Index(t => t.PageId)
                .Index(t => t.PermissionId)
                .Index(t => t.UrlId);
            
            CreateTable(
                "dbo.DI_M_Page",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UniqueName = c.String(),
                        Name = c.String(),
                        PageUrlId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_PageUrl", t => t.PageUrlId)
                .Index(t => t.PageUrlId);
            
            CreateTable(
                "dbo.DI_M_PageUrl",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Permission",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IconURL = c.String(),
                        ClientScript = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccountActionCategory",
                c => new
                    {
                        AccountActionTypeId = c.Byte(nullable: false),
                        AccountActionTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AccountActionTypeId);
            
            CreateTable(
                "dbo.DI_M_AccountCategory",
                c => new
                    {
                        AccountTypeId = c.Byte(nullable: false),
                        AccountTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AccountTypeId);
            
            CreateTable(
                "dbo.DI_M_AccountReconcilation",
                c => new
                    {
                        AccountReconcilationId = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Int(nullable: false),
                        EndpointUserId = c.Int(nullable: false),
                        UserId = c.Int(),
                        Version = c.Byte(nullable: false),
                        AccountName = c.String(),
                        EnteredDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        IsManuallyAdded = c.Boolean(nullable: false),
                        ResolvedBy = c.String(),
                        Comments = c.String(),
                        ExceptionType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.AccountReconcilationId);
            
            CreateTable(
                "dbo.DI_ADS_ComputerGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndpointSystemId = c.Int(nullable: false),
                        ComputerCN = c.String(),
                        ComputerDistinguishedName = c.String(),
                        GroupCN = c.String(),
                        GroupDistinguishedName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_ADS_ComputerInfo",
                c => new
                    {
                        ComputerId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        cn = c.String(),
                        givenName = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        displayName = c.String(),
                        uSNCreated = c.Long(nullable: false),
                        uSNChanged = c.Long(nullable: false),
                        name = c.String(),
                        objectGUID = c.String(),
                        userAccountControl = c.String(),
                        badPwdCount = c.String(),
                        codePage = c.String(),
                        countryCode = c.String(),
                        badPasswordTime = c.DateTime(),
                        lastLogoff = c.DateTime(),
                        lastLogon = c.DateTime(),
                        localPolicyFlags = c.String(),
                        pwdLastSet = c.DateTime(),
                        primaryGroupID = c.String(),
                        objectSid = c.String(),
                        accountExpires = c.DateTime(),
                        logonCount = c.Int(nullable: false),
                        sAMAccountName = c.String(),
                        sAMAccountType = c.String(),
                        operatingSystem = c.String(),
                        operatingSystemVersion = c.String(),
                        operatingSystemServicePack = c.String(),
                        serverReferenceBL = c.String(),
                        dNSHostName = c.String(),
                        rIDSetReferences = c.String(),
                        servicePrincipalName = c.String(),
                        objectCategory = c.String(),
                        isCriticalSystemObject = c.String(),
                        frsComputerReferenceBL = c.String(),
                        nTSecurityDescriptor = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.ComputerId);
            
            CreateTable(
                "dbo.DI_ADS_Configuration",
                c => new
                    {
                        ADSConfigurationId = c.Int(nullable: false, identity: true),
                        IPAddress = c.String(),
                        DomainName = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        Ou = c.String(),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndpointSystem_EndpointSystemId = c.Int(),
                    })
                .PrimaryKey(t => t.ADSConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystem_EndpointSystemId)
                .Index(t => t.EndpointSystem_EndpointSystemId);
            
            CreateTable(
                "dbo.DI_ADS_GroupInfoHistory",
                c => new
                    {
                        GroupHistoryId = c.Long(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        cn = c.String(),
                        description = c.String(),
                        member = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        uSNCreated = c.Long(nullable: false),
                        uSNChanged = c.Long(nullable: false),
                        name = c.String(),
                        objectGUID = c.String(),
                        objectSid = c.String(),
                        sAMAccountName = c.String(),
                        sAMAccountType = c.String(),
                        groupType = c.String(),
                        groupTypeDesc = c.String(),
                        objectCategory = c.String(),
                        nTSecurityDescriptor = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        RecordStatus = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.GroupHistoryId);
            
            CreateTable(
                "dbo.DI_ADS_GroupInfo",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        cn = c.String(),
                        description = c.String(),
                        member = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        uSNCreated = c.Long(),
                        uSNChanged = c.Long(),
                        name = c.String(),
                        objectGUID = c.String(),
                        objectSid = c.String(),
                        sAMAccountName = c.String(),
                        sAMAccountType = c.String(),
                        groupType = c.String(),
                        groupTypeDesc = c.String(),
                        objectCategory = c.String(),
                        nTSecurityDescriptor = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                        CanRequest = c.Boolean(),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.DI_ADS_UserInfo",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        cn = c.String(),
                        sn = c.String(),
                        ou = c.String(),
                        PwdResetCnt = c.Int(nullable: false),
                        st = c.String(),
                        title = c.String(),
                        description = c.String(),
                        postalAddress = c.String(),
                        postalCode = c.String(),
                        postOfficeBox = c.String(),
                        physicalDeliveryOfficeName = c.String(),
                        telephoneNumber = c.String(),
                        facsimileTelephoneNumber = c.String(),
                        givenName = c.String(),
                        initials = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        displayName = c.String(),
                        uSNCreated = c.Long(nullable: false),
                        info = c.String(),
                        uSNChanged = c.Long(nullable: false),
                        co = c.String(),
                        department = c.String(),
                        company = c.String(),
                        manager = c.String(),
                        streetAddress = c.String(),
                        wWWHomePage = c.String(),
                        name = c.String(),
                        objectGUID = c.String(),
                        userAccountControl = c.String(),
                        IsAccountDisabled = c.Boolean(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        IsPasswordRequired = c.Boolean(nullable: false),
                        PasswordExpirationDate = c.DateTime(),
                        badPwdCount = c.String(),
                        codePage = c.String(),
                        countryCode = c.String(),
                        badPasswordTime = c.DateTime(),
                        lastLogoff = c.DateTime(),
                        lastLogon = c.DateTime(),
                        pwdLastSet = c.DateTime(),
                        primaryGroupID = c.String(),
                        objectSid = c.String(),
                        accountExpires = c.DateTime(),
                        logonCount = c.String(),
                        sAMAccountName = c.String(),
                        sAMAccountType = c.String(),
                        managedObjects = c.String(),
                        ipPhone = c.String(),
                        objectCategory = c.String(),
                        mail = c.String(),
                        homePhone = c.String(),
                        mobile = c.String(),
                        pager = c.String(),
                        nTSecurityDescriptor = c.String(),
                        MappingType = c.String(),
                        DIUserId = c.Int(),
                        ExUserId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        IACMLastPasswordChange = c.DateTime(),
                        IfUnmappedMapTo = c.String(),
                        Version = c.Int(nullable: false),
                        businessCategory = c.String(),
                        division = c.String(),
                        homeDrive = c.String(),
                        homeDirectory = c.String(),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        IsHomeFolderPermissionSetFailed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_ADS_OuInfoHistory",
                c => new
                    {
                        OuHistoryId = c.Long(nullable: false, identity: true),
                        OuId = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        ou = c.String(),
                        description = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        uSNCreated = c.Long(nullable: false),
                        uSNChanged = c.Long(nullable: false),
                        showInAdvancedViewOnly = c.String(),
                        name = c.String(),
                        objectGUID = c.String(),
                        systemFlags = c.String(),
                        objectCategory = c.String(),
                        isCriticalSystemObject = c.String(),
                        gPLink = c.String(),
                        nTSecurityDescriptor = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        RecordStatus = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.OuHistoryId);
            
            CreateTable(
                "dbo.DI_ADS_OUInfo",
                c => new
                    {
                        OuId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        ou = c.String(),
                        description = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        uSNCreated = c.Long(nullable: false),
                        uSNChanged = c.Long(nullable: false),
                        showInAdvancedViewOnly = c.String(),
                        name = c.String(),
                        objectGUID = c.String(),
                        systemFlags = c.String(),
                        objectCategory = c.String(),
                        isCriticalSystemObject = c.String(),
                        gPLink = c.String(),
                        nTSecurityDescriptor = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.OuId);
            
            CreateTable(
                "dbo.DI_ADS_PrinterInfo",
                c => new
                    {
                        PrinterId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        cn = c.String(),
                        description = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        uSNCreated = c.Long(nullable: false),
                        uSNChanged = c.Long(nullable: false),
                        name = c.String(),
                        objectGUID = c.String(),
                        uNCName = c.String(),
                        versionNumber = c.String(),
                        location = c.String(),
                        serverName = c.String(),
                        portName = c.String(),
                        driverName = c.String(),
                        priority = c.Int(nullable: false),
                        printStartTime = c.DateTime(),
                        printEndTime = c.DateTime(),
                        printBinNames = c.String(),
                        printMaxResolutionSupported = c.String(),
                        printOrientationsSupported = c.String(),
                        printCollate = c.String(),
                        printColor = c.String(),
                        printShareName = c.String(),
                        printSpooling = c.String(),
                        printKeepPrintedJobs = c.String(),
                        driverVersion = c.String(),
                        printMaxXExtent = c.String(),
                        printMaxYExtent = c.String(),
                        printMinXExtent = c.String(),
                        printMinYExtent = c.String(),
                        printStaplingSupported = c.String(),
                        printMemory = c.String(),
                        printRate = c.String(),
                        printerName = c.String(),
                        managedBy = c.String(),
                        url = c.String(),
                        objectCategory = c.String(),
                        shortServerName = c.String(),
                        printDuplexSupported = c.String(),
                        nTSecurityDescriptor = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.PrinterId);
            
            CreateTable(
                "dbo.DI_ADS_SharedFolderInfo",
                c => new
                    {
                        FolderId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        cn = c.String(),
                        description = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        uSNCreated = c.Long(nullable: false),
                        uSNChanged = c.Long(nullable: false),
                        name = c.String(),
                        objectGUID = c.String(),
                        uNCName = c.String(),
                        managedBy = c.String(),
                        objectCategory = c.String(),
                        nTSecurityDescriptor = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.FolderId);
            
            CreateTable(
                "dbo.DI_ADS_UserInfoHistory",
                c => new
                    {
                        UserHistoryId = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        cn = c.String(),
                        sn = c.String(),
                        ou = c.String(),
                        PwdResetCnt = c.Int(nullable: false),
                        st = c.String(),
                        title = c.String(),
                        description = c.String(),
                        postalAddress = c.String(),
                        postalCode = c.String(),
                        postOfficeBox = c.String(),
                        physicalDeliveryOfficeName = c.String(),
                        telephoneNumber = c.String(),
                        facsimileTelephoneNumber = c.String(),
                        givenName = c.String(),
                        initials = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        displayName = c.String(),
                        uSNCreated = c.Long(nullable: false),
                        info = c.String(),
                        uSNChanged = c.Long(nullable: false),
                        co = c.String(),
                        department = c.String(),
                        company = c.String(),
                        manager = c.String(),
                        streetAddress = c.String(),
                        wWWHomePage = c.String(),
                        name = c.String(),
                        objectGUID = c.String(),
                        userAccountControl = c.String(),
                        IsAccountDisabled = c.Boolean(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        IsPasswordRequired = c.Boolean(nullable: false),
                        PasswordExpirationDate = c.DateTime(),
                        badPwdCount = c.String(),
                        codePage = c.String(),
                        countryCode = c.String(),
                        badPasswordTime = c.DateTime(),
                        lastLogoff = c.DateTime(),
                        lastLogon = c.DateTime(),
                        pwdLastSet = c.DateTime(),
                        primaryGroupID = c.String(),
                        objectSid = c.String(),
                        accountExpires = c.DateTime(),
                        logonCount = c.String(),
                        sAMAccountName = c.String(),
                        sAMAccountType = c.String(),
                        managedObjects = c.String(),
                        ipPhone = c.String(),
                        objectCategory = c.String(),
                        mail = c.String(),
                        homePhone = c.String(),
                        mobile = c.String(),
                        pager = c.String(),
                        nTSecurityDescriptor = c.String(),
                        MappingType = c.String(),
                        DIUserId = c.Int(),
                        ExUserId = c.Int(),
                        IACMLastPasswordChange = c.DateTime(),
                        IfUnmappedMapTo = c.String(),
                        Version = c.Int(nullable: false),
                        businessCategory = c.String(),
                        division = c.String(),
                        homeDrive = c.String(),
                        homeDirectory = c.String(),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        IsHomeFolderPermissionSetFailed = c.Boolean(nullable: false),
                        AuditUser = c.String(),
                        AuditDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserHistoryId);
            
            CreateTable(
                "dbo.DI_ADS_XLProvMatrix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyCode = c.String(),
                        DepartmentCode = c.String(),
                        DivisionCode = c.String(),
                        OrganisationId = c.String(),
                        OrganisationName = c.String(),
                        CostCenterCode = c.String(),
                        CostDistributionCode = c.String(),
                        OU = c.String(),
                        DefaultGroups = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ApprovalUserTypes",
                c => new
                    {
                        ApprovalUserTypeId = c.Byte(nullable: false),
                        ApprovalUserTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ApprovalUserTypeId);
            
            CreateTable(
                "dbo.DI_M_XLApproverExceptions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Person = c.String(),
                        Approver = c.String(),
                        ApprovalLevel = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Attestation",
                c => new
                    {
                        AttestationLogId = c.Int(nullable: false, identity: true),
                        AttestationId = c.Int(),
                        AttestationName = c.String(),
                        LogLevel = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AttestationLogId);
            
            CreateTable(
                "dbo.DI_M_Attestation_Endpoint",
                c => new
                    {
                        AttestationEndpointId = c.Long(nullable: false, identity: true),
                        AttestationId = c.Int(nullable: false),
                        EndpointSystemType = c.Int(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AttestationEndpointId)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId, cascadeDelete: true)
                .Index(t => t.AttestationId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_Attestation",
                c => new
                    {
                        AttestationId = c.Int(nullable: false, identity: true),
                        AttestationName = c.String(),
                        AttestationDescription = c.String(),
                        TenantId = c.String(),
                        AttestationType = c.String(),
                        AttestationScope = c.String(),
                        Department = c.String(),
                        BusinessRole = c.String(),
                        Manager = c.String(),
                        Group = c.String(),
                        Company = c.String(),
                        Section = c.String(),
                        AllUser = c.Boolean(),
                        UserName = c.Int(),
                        WorkflowConfigurationId = c.Short(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AttestationId)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId, cascadeDelete: true)
                .Index(t => t.WorkflowConfigurationId);
            
            CreateTable(
                "dbo.DI_M_AttestationProcess",
                c => new
                    {
                        AttestationProcessId = c.Int(nullable: false, identity: true),
                        AttestationId = c.Int(),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        WorkflowStatus = c.String(),
                        HistoryId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AttestationProcessId)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId)
                .Index(t => t.AttestationId);
            
            CreateTable(
                "dbo.DI_M_WorkflowConfiguration",
                c => new
                    {
                        WorkflowConfigurationId = c.Short(nullable: false, identity: true),
                        WorkflowName = c.String(),
                        Description = c.String(),
                        RequestTypeId = c.Byte(nullable: false),
                        EndpointSystemId = c.Int(),
                        ApplicableUserTypes = c.String(),
                        ApplicableAccountTypes = c.String(),
                        ApplicableActionTypes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.WorkflowConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .ForeignKey("dbo.DI_M_RequestTypes", t => t.RequestTypeId, cascadeDelete: true)
                .Index(t => t.RequestTypeId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_WorkflowConfigurationDetail",
                c => new
                    {
                        WorkflowConfigurationDetailId = c.Int(nullable: false, identity: true),
                        WorkflowConfigurationId = c.Short(nullable: false),
                        SequenceNumber = c.Byte(nullable: false),
                        ApprovalUserType = c.Byte(nullable: false),
                        Approver = c.Int(nullable: false),
                        ResponseTimeInDays = c.Int(nullable: false),
                        IsActionRequired = c.Boolean(nullable: false),
                        RequiresApprovalFromAll = c.Boolean(nullable: false),
                        IsMockGroup = c.Boolean(nullable: false),
                        IsReminderRequired = c.Boolean(nullable: false),
                        ReminderIntervalInDays = c.Int(),
                        IsEscalationRequired = c.Boolean(nullable: false),
                        DaysToWaitAfterEscalation = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.WorkflowConfigurationDetailId)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId, cascadeDelete: true)
                .Index(t => t.WorkflowConfigurationId);
            
            CreateTable(
                "dbo.DI_M_AttestationProcessHistory",
                c => new
                    {
                        AttestationProcessId = c.Int(nullable: false, identity: true),
                        AttestationId = c.Int(),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        Status = c.String(),
                        Message = c.String(),
                        WorkflowStatus = c.String(),
                        HistoryId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.AttestationProcessId)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId)
                .Index(t => t.AttestationId);
            
            CreateTable(
                "dbo.DI_M_AttestationReview_EndpointSystemHistory",
                c => new
                    {
                        ReviewEndpointHistoryId = c.Long(nullable: false, identity: true),
                        ReviewEndpointId = c.Long(nullable: false),
                        EndpointUserId = c.Int(nullable: false),
                        EndpointUserName = c.String(),
                        EndpointSystemId = c.Int(nullable: false),
                        HostName = c.String(),
                        EndpointSystemTypeId = c.Int(nullable: false),
                        EndpointSystemTypeName = c.String(),
                        Approve = c.Boolean(nullable: false),
                        Revoke = c.Boolean(nullable: false),
                        Comments = c.String(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsCurrentLevelApproved = c.Boolean(nullable: false),
                        IsChanged = c.Boolean(nullable: false),
                        PreviousAccess = c.String(),
                    })
                .PrimaryKey(t => t.ReviewEndpointHistoryId);
            
            CreateTable(
                "dbo.DI_M_AttestationReview_EndpointSystem_L2DetailHistory",
                c => new
                    {
                        L2DetailHistoryId = c.Long(nullable: false, identity: true),
                        L2DetailId = c.Long(nullable: false),
                        ReviewEndpointId = c.Long(nullable: false),
                        AccessType = c.String(),
                        AccessName = c.String(),
                        ResourceType = c.String(),
                        ResourceCompletePath = c.String(),
                        Approve = c.Boolean(nullable: false),
                        Revoke = c.Boolean(nullable: false),
                        BusinessOwner = c.String(),
                        Comments = c.String(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsChanged = c.Boolean(nullable: false),
                        PreviousAccess = c.String(),
                    })
                .PrimaryKey(t => t.L2DetailHistoryId);
            
            CreateTable(
                "dbo.DI_M_AttestationReview_EndpointSystem_L2Detail",
                c => new
                    {
                        L2DetailId = c.Long(nullable: false, identity: true),
                        ReviewEndpointId = c.Long(nullable: false),
                        AccessType = c.String(),
                        AccessName = c.String(),
                        AccessDescription = c.String(),
                        ResourceType = c.String(),
                        ResourceCompletePath = c.String(),
                        Approve = c.Boolean(nullable: false),
                        Revoke = c.Boolean(nullable: false),
                        BusinessOwner = c.String(),
                        Comments = c.String(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsChanged = c.Boolean(nullable: false),
                        PreviousAccess = c.String(),
                    })
                .PrimaryKey(t => t.L2DetailId)
                .ForeignKey("dbo.DI_M_AttestationReview_EndpointSystem", t => t.ReviewEndpointId, cascadeDelete: true)
                .Index(t => t.ReviewEndpointId);
            
            CreateTable(
                "dbo.DI_M_AttestationReview_EndpointSystem_L3Detail",
                c => new
                    {
                        L3DetailId = c.Long(nullable: false, identity: true),
                        L2DetailId = c.Long(nullable: false),
                        AccessType = c.String(),
                        AccessName = c.String(),
                        ResourceType = c.String(),
                        ResourceCompletePath = c.String(),
                        Approve = c.Boolean(nullable: false),
                        Revoke = c.Boolean(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                        BusinessOwner = c.String(),
                        Comments = c.String(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsChanged = c.Boolean(nullable: false),
                        PreviousAccess = c.String(),
                    })
                .PrimaryKey(t => t.L3DetailId)
                .ForeignKey("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", t => t.L2DetailId, cascadeDelete: true)
                .Index(t => t.L2DetailId);
            
            CreateTable(
                "dbo.DI_M_AttestationReview_EndpointSystem_L3DetailHistory",
                c => new
                    {
                        L3DetailHistoryId = c.Long(nullable: false, identity: true),
                        L3DetailId = c.Long(nullable: false),
                        L2DetailId = c.Long(nullable: false),
                        AccessType = c.String(),
                        AccessName = c.String(),
                        ResourceType = c.String(),
                        ResourceCompletePath = c.String(),
                        Approve = c.Boolean(nullable: false),
                        Revoke = c.Boolean(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                        BusinessOwner = c.String(),
                        Comments = c.String(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsChanged = c.Boolean(nullable: false),
                        PreviousAccess = c.String(),
                    })
                .PrimaryKey(t => t.L3DetailHistoryId);
            
            CreateTable(
                "dbo.DI_M_AttestationReview_EndpointSystem",
                c => new
                    {
                        ReviewEndpointId = c.Long(nullable: false, identity: true),
                        EndpointUserId = c.Int(nullable: false),
                        EndpointUserName = c.String(),
                        AccountType = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        HostName = c.String(),
                        EndpointSystemTypeId = c.Int(nullable: false),
                        EndpointSystemTypeName = c.String(),
                        Approve = c.Boolean(nullable: false),
                        Revoke = c.Boolean(nullable: false),
                        Comments = c.String(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsCurrentLevelApproved = c.Boolean(nullable: false),
                        IsChanged = c.Boolean(nullable: false),
                        PreviousAccess = c.String(),
                    })
                .PrimaryKey(t => t.ReviewEndpointId);
            
            CreateTable(
                "dbo.DI_M_AttestationReview",
                c => new
                    {
                        ReviewId = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Comments = c.String(),
                        SubmittedBy = c.String(),
                        SubmittedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        ActionId = c.String(),
                        ActionTakenBy = c.String(),
                        ActionComments = c.String(),
                        HistoryId = c.Int(nullable: false),
                        IsChanged = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_ReviewEndpointSystems",
                c => new
                    {
                        ReviewEndpointSystemId = c.Long(nullable: false, identity: true),
                        ReviewId = c.Long(nullable: false),
                        ReviewEndpointId = c.Long(nullable: false),
                        TaskId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewEndpointSystemId)
                .ForeignKey("dbo.DI_M_AttestationReview_EndpointSystem", t => t.ReviewEndpointId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_AttestationReview", t => t.ReviewId, cascadeDelete: true)
                .Index(t => t.ReviewId)
                .Index(t => t.ReviewEndpointId);
            
            CreateTable(
                "dbo.DI_M_CompliancePolicy",
                c => new
                    {
                        CompliancePolicyId = c.Int(nullable: false, identity: true),
                        CompliancePolicyCode = c.String(),
                        CompliancePolicyName = c.String(),
                        CompliancePolicyDescription = c.String(),
                        CompliancePolicyType = c.String(),
                        Status = c.String(),
                        OwnerId = c.Int(),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.CompliancePolicyId)
                .ForeignKey("dbo.DI_M_User", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.DI_M_SODConflictFunctions",
                c => new
                    {
                        SodConflictFunctionId = c.Int(nullable: false, identity: true),
                        SodConflictCode = c.String(),
                        SodConflictDescription = c.String(),
                        RiskCodeId = c.Int(nullable: false),
                        LastGeneratedDate = c.DateTime(),
                        IsActive = c.Boolean(),
                        Status = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SodConflictFunctionId)
                .ForeignKey("dbo.DI_M_SODRiskCode", t => t.RiskCodeId, cascadeDelete: true)
                .Index(t => t.RiskCodeId);
            
            CreateTable(
                "dbo.DI_M_SODFunction",
                c => new
                    {
                        SodFunctionId = c.Int(nullable: false, identity: true),
                        SodFunctionCode = c.String(),
                        SodFunctionDescription = c.String(),
                        IsActive = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SodFunctionId);
            
            CreateTable(
                "dbo.DI_M_SODException",
                c => new
                    {
                        ExceptionId = c.Int(nullable: false, identity: true),
                        ExceptionCode = c.String(),
                        ExceptionDesc = c.String(),
                        RiskOwnerId = c.Int(),
                        SodFunction_SodFunctionId = c.Int(),
                        SODRunPolicy_SODRunPolicyId = c.Int(),
                    })
                .PrimaryKey(t => t.ExceptionId)
                .ForeignKey("dbo.DI_M_User", t => t.RiskOwnerId)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.SodFunction_SodFunctionId)
                .ForeignKey("dbo.DI_M_SODRunPolicy", t => t.SODRunPolicy_SODRunPolicyId)
                .Index(t => t.RiskOwnerId)
                .Index(t => t.SodFunction_SodFunctionId)
                .Index(t => t.SODRunPolicy_SODRunPolicyId);
            
            CreateTable(
                "dbo.DI_M_SODExceptionItem",
                c => new
                    {
                        ExceptionItemId = c.Int(nullable: false, identity: true),
                        ExceptionId = c.Int(),
                        UserId = c.Int(),
                        FunctionId1 = c.Int(),
                        FunctionItemId1 = c.Int(),
                        FunctionId2 = c.Int(),
                        FunctionItemId2 = c.Int(),
                    })
                .PrimaryKey(t => t.ExceptionItemId)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId1)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId2)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItemId1)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItemId2)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .ForeignKey("dbo.DI_M_SODException", t => t.ExceptionId)
                .Index(t => t.ExceptionId)
                .Index(t => t.UserId)
                .Index(t => t.FunctionId1)
                .Index(t => t.FunctionItemId1)
                .Index(t => t.FunctionId2)
                .Index(t => t.FunctionItemId2);
            
            CreateTable(
                "dbo.DI_M_SODFunctionItem",
                c => new
                    {
                        SodFunctionItemId = c.Int(nullable: false, identity: true),
                        SodFunctionId = c.Int(),
                        SodFunctionType = c.String(),
                        SodRoleId = c.String(),
                        SodRoleDesc = c.String(),
                        ObjectCode = c.String(),
                        FieldCode = c.String(),
                        FromValue = c.String(),
                        EndpointSystemTypeId = c.Int(),
                        Endpointsystemid = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SodFunctionItemId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.Endpointsystemid)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.SodFunctionId)
                .Index(t => t.SodFunctionId)
                .Index(t => t.EndpointSystemTypeId)
                .Index(t => t.Endpointsystemid);
            
            CreateTable(
                "dbo.DI_M_SODRiskCode",
                c => new
                    {
                        RiskId = c.Int(nullable: false, identity: true),
                        RiskCode = c.String(),
                        RiskDescription = c.String(),
                        RiskLevelId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        UserId = c.Int(),
                        RiskScoreId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RiskId)
                .ForeignKey("dbo.DI_M_SODRiskLevel", t => t.RiskLevelId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_SODRiskScore", t => t.RiskScoreId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.RiskLevelId)
                .Index(t => t.UserId)
                .Index(t => t.RiskScoreId);
            
            CreateTable(
                "dbo.DI_M_SODMitigation",
                c => new
                    {
                        MitigationId = c.Int(nullable: false, identity: true),
                        MitigationCode = c.String(),
                        MitigationDescription = c.String(),
                        MitigationSteps = c.String(),
                        ApproverUserId = c.Int(),
                        ApproverGroupId = c.Int(),
                        MonitorUserId = c.Int(),
                        MonitorGroupId = c.Int(),
                        IsActive = c.Boolean(),
                        DocumentName = c.String(),
                        DocumentUploadedPath = c.String(),
                        DocumentUploadedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MitigationId);
            
            CreateTable(
                "dbo.DI_M_SODRiskLevel",
                c => new
                    {
                        RiskLevelId = c.Int(nullable: false, identity: true),
                        RiskLevelDescription = c.String(),
                        RiskLevelCode = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RiskLevelId);
            
            CreateTable(
                "dbo.DI_M_SODRiskScore",
                c => new
                    {
                        RiskScoreId = c.Int(nullable: false, identity: true),
                        RiskScore = c.Int(nullable: false),
                        RiskScoreDescription = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RiskScoreId);
            
            CreateTable(
                "dbo.DI_M_ComplianceType",
                c => new
                    {
                        PolicyTypeId = c.Int(nullable: false, identity: true),
                        PolicyTypeName = c.String(),
                    })
                .PrimaryKey(t => t.PolicyTypeId);
            
            CreateTable(
                "dbo.DI_M_ConfigurationMasterHistory",
                c => new
                    {
                        ConfigHistoryId = c.Long(nullable: false, identity: true),
                        ConfigId = c.Short(nullable: false),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ConfigHistoryId);
            
            CreateTable(
                "dbo.DI_M_ConfigurationMaster",
                c => new
                    {
                        ConfigId = c.Short(nullable: false, identity: true),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsUiDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ConfigId);
            
            CreateTable(
                "dbo.DI_M_CostCenter",
                c => new
                    {
                        CostCenterId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        CompanyId = c.Int(nullable: false),
                        CostCenterCode = c.String(),
                        CostCenterText = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CostCenterId);
            
            CreateTable(
                "dbo.DI_M_DashboardAccessRights",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        DashboardId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Dashboard", t => t.DashboardId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.DashboardId);
            
            CreateTable(
                "dbo.DI_M_Dashboard",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DashboardName = c.String(),
                        DashboardCategory = c.String(),
                        DashBoardType = c.Int(nullable: false),
                        TargetUrl = c.String(),
                        NavigateUrl = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_DefaultProvisioningFormGeneralData",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Int(nullable: false),
                        MasterAttributeName = c.String(),
                        EndPointAttributeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_DefaultRoleDashboard",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        DashBoardId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Dashboard", t => t.DashBoardId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.DashBoardId);
            
            CreateTable(
                "dbo.DI_M_DefaultRoleMenu",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        TabMenuId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_TabMenu", t => t.TabMenuId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.TabMenuId);
            
            CreateTable(
                "dbo.DI_M_TabMenu",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UniqueName = c.String(),
                        MenuName = c.String(),
                        ParentTabMenuId = c.Int(),
                        PageId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Page", t => t.PageId)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.DI_M_DiscoveryJobConfiguration",
                c => new
                    {
                        DiscoveryJobId = c.Int(nullable: false, identity: true),
                        DiscoveryProcessId = c.Int(nullable: false),
                        EndpointObjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiscoveryJobId)
                .ForeignKey("dbo.DI_M_DiscoveryProcess", t => t.DiscoveryProcessId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_EndpointObjects", t => t.EndpointObjectId, cascadeDelete: true)
                .Index(t => t.DiscoveryProcessId)
                .Index(t => t.EndpointObjectId);
            
            CreateTable(
                "dbo.DI_Log_Discovery",
                c => new
                    {
                        DiscoveryLogId = c.Int(nullable: false, identity: true),
                        DiscoveryProcessId = c.Int(),
                        LogLevel = c.String(),
                        EndpointSystemId = c.Int(nullable: false),
                        EndpointSystemType = c.Int(nullable: false),
                        EndpointSystemName = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DiscoveryLogId);
            
            CreateTable(
                "dbo.DI_M_DiscoveryProcessHistory",
                c => new
                    {
                        DiscoveryProcessHistoryId = c.Long(nullable: false, identity: true),
                        ProcessId = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        Status = c.String(),
                        Message = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        RecordStatus = c.String(),
                    })
                .PrimaryKey(t => t.DiscoveryProcessHistoryId);
            
            CreateTable(
                "dbo.DI_M_EmailHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActivityType = c.String(),
                        Priority = c.String(),
                        From = c.String(),
                        To = c.String(),
                        Cc = c.String(),
                        Bcc = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        IsHtml = c.Boolean(nullable: false),
                        Attachment = c.String(),
                        Status = c.String(),
                        CreatedBy = c.String(),
                        CreatedDateTime = c.DateTime(),
                        DispatchDateTime = c.DateTime(),
                        SentDateTime = c.DateTime(),
                        NoOfAttempts = c.Int(nullable: false),
                        RecordStatus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Email",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActivityType = c.String(),
                        Priority = c.String(),
                        From = c.String(),
                        To = c.String(),
                        Cc = c.String(),
                        Bcc = c.String(),
                        Subject = c.String(),
                        Body = c.String(),
                        IsHtml = c.Boolean(nullable: false),
                        Attachment = c.String(),
                        Status = c.String(),
                        CreatedBy = c.String(),
                        CreatedDateTime = c.DateTime(),
                        DispatchDateTime = c.DateTime(),
                        SentDateTime = c.DateTime(),
                        NoOfAttempts = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Email",
                c => new
                    {
                        EmailLogId = c.Int(nullable: false, identity: true),
                        EmailId = c.Int(),
                        EmailSubject = c.String(),
                        LogLevel = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EmailLogId);
            
            CreateTable(
                "dbo.DI_M_EmployeeEventTrigger",
                c => new
                    {
                        EmployeeEventTriggerId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        UserId = c.Int(nullable: false),
                        EventType = c.Byte(nullable: false),
                        EffectiveDate = c.DateTime(nullable: false),
                        IsRequestCreated = c.Boolean(nullable: false),
                        IsAdminCheckRequired = c.Boolean(nullable: false),
                        IsRejoiningUser = c.Boolean(nullable: false),
                        UserDetails = c.String(),
                        OldUserDetails = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmployeeEventTriggerId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_EmployeeTerm",
                c => new
                    {
                        EmployeeTermId = c.Byte(nullable: false),
                        EmployeeTermDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeTermId);
            
            CreateTable(
                "dbo.DI_M_EntitlementReconcilation",
                c => new
                    {
                        EntitlementReconcilationId = c.Long(nullable: false, identity: true),
                        AccountReconcilationId = c.Long(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        EndpointUserId = c.Int(nullable: false),
                        UserId = c.Int(),
                        Version = c.Byte(nullable: false),
                        EntitlementType = c.Int(nullable: false),
                        AccountName = c.String(),
                        Entitlement = c.String(),
                        EnteredDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        IsManuallyAdded = c.Boolean(nullable: false),
                        ResolvedBy = c.String(),
                        Comments = c.String(),
                        ExceptionType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.EntitlementReconcilationId);
            
            CreateTable(
                "dbo.DI_M_GlobalAnswer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Question = c.String(),
                        Answer = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_GlobalQuestion",
                c => new
                    {
                        GlobalQuestionId = c.Int(nullable: false, identity: true),
                        Question = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GlobalQuestionId);
            
            CreateTable(
                "dbo.DI_M_GroupHistory",
                c => new
                    {
                        GroupHistoryId = c.Long(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        GroupName = c.String(),
                        GroupType = c.String(),
                        GroupOwner = c.String(),
                        GroupDescription = c.String(),
                        IsDefaultGroup = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.GroupHistoryId);
            
            CreateTable(
                "dbo.DI_INH_Permission",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemID = c.Int(nullable: false),
                        InhouseSystemCode = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemID, cascadeDelete: true)
                .Index(t => t.EndPointSystemID);
            
            CreateTable(
                "dbo.DI_INH_Configuration",
                c => new
                    {
                        InhouseSystemId = c.Int(nullable: false, identity: true),
                        EndpointSystemId = c.Int(nullable: false),
                        CanRequestMultipleRoles = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.InhouseSystemId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId, cascadeDelete: true)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_INH_UserInfo",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        InhouseSystemCode = c.String(),
                        NRIC = c.String(),
                        EmployeeId = c.String(),
                        Email = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        DisplayName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        AccountCreatedDate = c.DateTime(),
                        LastLoginDate = c.DateTime(),
                        IsLocked = c.Boolean(nullable: false),
                        LockedDate = c.DateTime(),
                        IsDisabled = c.Boolean(nullable: false),
                        DisabledDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        DIUserId = c.Int(),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        Remarks1 = c.String(),
                        Remarks2 = c.String(),
                        Remarks3 = c.String(),
                        Remarks4 = c.String(),
                        Remarks5 = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Permission = c.String(),
                        Version = c.Int(nullable: false),
                        AccountName = c.String(),
                        MappingType = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_M_InterimStore",
                c => new
                    {
                        InterimStoreId = c.Long(nullable: false, identity: true),
                        InterimStoreItemType = c.Byte(nullable: false),
                        TaskId = c.Long(),
                        UserId = c.Int(),
                        XmlData = c.String(),
                        IsSaved = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.InterimStoreId);
            
            CreateTable(
                "dbo.DI_LNS_Configuration",
                c => new
                    {
                        LNSConfigurationId = c.Int(nullable: false, identity: true),
                        EndPointSystemId = c.Int(nullable: false),
                        HostName = c.String(),
                        IPAddress = c.String(),
                        DomainName = c.String(),
                        ServerName = c.String(),
                        Password = c.String(),
                        RootCertificate = c.String(),
                        DenyAllAccessGroupName = c.String(),
                        IDVaultAdmin = c.String(),
                        IDVaultAdminFile = c.String(),
                        MailDomain = c.String(),
                        MailServer = c.String(),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.LNSConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_LNS_MailQuotaConfig",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.String(),
                        MailQuotaSizeInMB = c.Int(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_LNS_XLProvMatrix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyCode = c.String(),
                        DepartmentCode = c.String(),
                        DivisionCode = c.String(),
                        OrganisationId = c.String(),
                        OrganisationName = c.String(),
                        CostCenterCode = c.String(),
                        CostDistributionCode = c.String(),
                        MailServer = c.String(),
                        CertifierIdFile = c.String(),
                        CertifierOU = c.String(),
                        DefaultGroups = c.String(),
                        PolicyName = c.String(),
                        ReplicatSever = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_LNS_Certifiers",
                c => new
                    {
                        CertifierId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CertifierType = c.String(),
                        Owner = c.String(),
                        FullName = c.String(),
                        FormIssuedBy = c.String(),
                        MailDomain = c.String(),
                        CertificateType = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.CertifierId);
            
            CreateTable(
                "dbo.DI_LNS_GroupsHistory",
                c => new
                    {
                        GroupHistoryId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Group = c.String(),
                        Category = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        RecordStatus = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.GroupHistoryId);
            
            CreateTable(
                "dbo.DI_LNS_Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        Owner = c.String(),
                        Group = c.String(),
                        Category = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                        CanRequest = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_LNS_Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Name = c.String(),
                        LastName = c.String(),
                        ListNameFullName = c.String(),
                        FullName = c.String(),
                        ListName = c.String(),
                        Department = c.String(),
                        CompanyName = c.String(),
                        Owner = c.String(),
                        JobTitle = c.String(),
                        PasswordChangedDate = c.DateTime(),
                        Members = c.String(),
                        MailServer = c.String(),
                        MailFile = c.String(),
                        MailDomain = c.String(),
                        MailAddress = c.String(),
                        ShortName = c.String(),
                        Location = c.String(),
                        EncryptIncomingMail = c.String(),
                        CalendarDomain = c.String(),
                        NetUserName = c.String(),
                        HTTPPassword = c.String(),
                        CertThumbprint = c.String(),
                        InternetAddress = c.String(),
                        NewMailServer = c.String(),
                        NewMailFile = c.String(),
                        AltFullName = c.String(),
                        MessageStorage = c.String(),
                        ServerName = c.String(),
                        AltFullNameLanguage = c.String(),
                        QuickPlace = c.String(),
                        SametimeServer = c.String(),
                        Type = c.String(),
                        MailSystem = c.String(),
                        AdminpOldWebName = c.String(),
                        HTTPPasswordChangeDate = c.String(),
                        HTTPPasswordForceChange = c.String(),
                        Policy = c.String(),
                        LTPA_UsrNm = c.String(),
                        DB2UserName = c.String(),
                        OfficePhoneNumber = c.String(),
                        OfficeFAXPhoneNumber = c.String(),
                        AvailableForDirSync = c.Boolean(nullable: false),
                        MailExternalAccess = c.Boolean(nullable: false),
                        MailChargeable = c.Boolean(nullable: false),
                        Comment = c.String(),
                        IDMPolicy = c.Boolean(nullable: false),
                        NRIC = c.String(),
                        PwdResetCnt = c.Int(nullable: false),
                        CryptoCap = c.String(),
                        DIUserId = c.Int(),
                        IfUnmappedMapTo = c.String(),
                        MappingType = c.String(),
                        IACMLastPasswordChange = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_LNS_UsersHistory",
                c => new
                    {
                        UserHistoryId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Name = c.String(),
                        LastName = c.String(),
                        ListNameFullName = c.String(),
                        FullName = c.String(),
                        ListName = c.String(),
                        Department = c.String(),
                        CompanyName = c.String(),
                        Owner = c.String(),
                        JobTitle = c.String(),
                        PasswordChangedDate = c.DateTime(),
                        Members = c.String(),
                        MailServer = c.String(),
                        MailFile = c.String(),
                        MailDomain = c.String(),
                        MailAddress = c.String(),
                        ShortName = c.String(),
                        Location = c.String(),
                        EncryptIncomingMail = c.String(),
                        CalendarDomain = c.String(),
                        NetUserName = c.String(),
                        HTTPPassword = c.String(),
                        CertThumbprint = c.String(),
                        InternetAddress = c.String(),
                        NewMailServer = c.String(),
                        NewMailFile = c.String(),
                        AltFullName = c.String(),
                        MessageStorage = c.String(),
                        ServerName = c.String(),
                        AltFullNameLanguage = c.String(),
                        QuickPlace = c.String(),
                        SametimeServer = c.String(),
                        Type = c.String(),
                        MailSystem = c.String(),
                        AdminpOldWebName = c.String(),
                        HTTPPasswordChangeDate = c.String(),
                        HTTPPasswordForceChange = c.String(),
                        Policy = c.String(),
                        LTPA_UsrNm = c.String(),
                        DB2UserName = c.String(),
                        OfficePhoneNumber = c.String(),
                        OfficeFAXPhoneNumber = c.String(),
                        AvailableForDirSync = c.Boolean(nullable: false),
                        MailExternalAccess = c.Boolean(nullable: false),
                        MailChargeable = c.Boolean(nullable: false),
                        Comment = c.String(),
                        IDMPolicy = c.Boolean(nullable: false),
                        NRIC = c.String(),
                        PwdResetCnt = c.Int(nullable: false),
                        CryptoCap = c.String(),
                        DIUserId = c.Int(),
                        IfUnmappedMapTo = c.String(),
                        MappingType = c.String(),
                        IACMLastPasswordChange = c.DateTime(),
                        Version = c.Int(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AuditUser = c.String(),
                        AuditDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserHistoryId);
            
            CreateTable(
                "dbo.DI_M_MailServerConfiguration",
                c => new
                    {
                        MailServerId = c.Int(nullable: false, identity: true),
                        EmailID = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        HostName = c.String(),
                        PortNumber = c.String(),
                        EnableSSL = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.MailServerId);
            
            CreateTable(
                "dbo.DI_M_MailTemplate",
                c => new
                    {
                        TemplateId = c.Int(nullable: false, identity: true),
                        TemplateName = c.String(),
                        TemplateDescription = c.String(),
                        MailFrom = c.String(),
                        MailTo = c.String(),
                        MailCC = c.String(),
                        MailBcc = c.String(),
                        MailSubject = c.String(),
                        MailContent = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.TemplateId);
            
            CreateTable(
                "dbo.DI_OEBS_Configuration",
                c => new
                    {
                        OEBSConfigurationId = c.Int(nullable: false, identity: true),
                        EndpointSystemId = c.Int(nullable: false),
                        HostName = c.String(),
                        IPAddress = c.String(),
                        DomainName = c.String(),
                        ServerName = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        InstanceName = c.String(),
                        PortNo = c.Int(nullable: false),
                        ImportUsers = c.Boolean(nullable: false),
                        ImportRoles = c.Boolean(nullable: false),
                        ImportTables = c.Boolean(nullable: false),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OEBSConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId, cascadeDelete: true)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_OEBS_HRMSUserInfo",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        DIUserId = c.Int(),
                        UserName = c.String(),
                        PERSONID = c.String(),
                        EFFECTIVE_START_DATE = c.DateTime(),
                        EFFECTIVE_END_DATE = c.DateTime(),
                        BUSINESS_GROUP_ID = c.String(),
                        PERSON_TYPE_ID = c.String(),
                        START_DATE = c.DateTime(),
                        DATE_OF_BIRTH = c.DateTime(),
                        EMAIL_ADDRESS = c.String(),
                        FIRST_NAME = c.String(),
                        LAST_NAME = c.String(),
                        FULL_NAME = c.String(),
                        KNOWN_AS = c.String(),
                        MARTIAL_STATUS = c.String(),
                        NATIONALITY = c.String(),
                        NATIONAL_IDENTIFIER = c.String(),
                        PREVIOUS_LAST_NAME = c.String(),
                        SEX = c.String(),
                        TITLE = c.String(),
                        WORK_TELEPHONE = c.String(),
                        ORGINAL_DATE_OF_HIRE = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AccountPassword = c.String(),
                        OrganisationName = c.String(),
                        OrganisationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.DI_OEBS_HRMSUserInfoHistory",
                c => new
                    {
                        OEBSHRMSUserInfoHistoryid = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        DIUserId = c.Int(),
                        UserName = c.String(),
                        PERSONID = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EFFECTIVE_START_DATE = c.DateTime(nullable: false),
                        EFFECTIVE_END_DATE = c.DateTime(nullable: false),
                        PERSON_TYPE_ID = c.String(),
                        DATE_OF_BIRTH = c.DateTime(nullable: false),
                        EMAIL_ADDRESS = c.String(),
                        FIRST_NAME = c.String(),
                        FULL_NAME = c.String(),
                        MARTIAL_STATUS = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NATIONALITY = c.String(),
                        NATIONAL_IDENTIFIER = c.String(),
                        PREVIOUS_LAST_NAME = c.String(),
                        SEX = c.String(),
                        TITLE = c.String(),
                        ORGINAL_DATE_OF_HIRE = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AccountPassword = c.String(),
                    })
                .PrimaryKey(t => t.OEBSHRMSUserInfoHistoryid);
            
            CreateTable(
                "dbo.DI_OEBS_MenusInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MENU_ID = c.Double(),
                        MENU_NAME = c.String(),
                        LAST_UPDATED_BY = c.Double(),
                        LAST_UPDATE_DATE = c.DateTime(),
                        LAST_UPDATE_LOGIN = c.Double(),
                        CREATED_BY = c.Double(nullable: false),
                        CREATION_DATE = c.DateTime(),
                        SECURITY_GROUP_ID = c.Int(),
                        LANGUAGE = c.String(),
                        SOURCE_LANG = c.String(),
                        DESCRIPTION = c.String(),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_OEBS_ResponsibilitiesInfo",
                c => new
                    {
                        ResponsibilityId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        APPLICATION_ID = c.Double(nullable: false),
                        RESPONSIBILITY_ID = c.Double(nullable: false),
                        LAST_UPDATED_BY = c.Double(),
                        LAST_UPDATE_DATE = c.DateTime(),
                        CREATED_BY = c.Double(nullable: false),
                        CREATION_DATE = c.DateTime(),
                        DATA_GROUP_APPLICATION_ID = c.Double(nullable: false),
                        DATA_GROUP_ID = c.Double(nullable: false),
                        MENU_ID = c.Double(nullable: false),
                        START_DATE = c.DateTime(),
                        END_DATE = c.DateTime(),
                        GROUP_APPLICATION_ID = c.Int(nullable: false),
                        REQUEST_GROUP_ID = c.Int(nullable: false),
                        OEBS_VERSION = c.String(),
                        WEB_HOST_NAME = c.String(),
                        WEB_AGENT_NAME = c.String(),
                        RESPONSIBILITY_KEY = c.String(),
                        SECURITY_GROUP_ID = c.Int(),
                        LANGUAGE = c.String(),
                        RESPONSIBILITY_NAME = c.String(),
                        DESCRIPTION = c.String(),
                        LAST_UPDATE_LOGIN = c.Double(nullable: false),
                        APPLICATION_SHORT_NAME = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ResponsibilityId);
            
            CreateTable(
                "dbo.DI_OEBS_UserInfo",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        DIUserId = c.Int(),
                        USER_ID = c.String(),
                        USER_NAME = c.String(),
                        LAST_UPDATE_DATE = c.DateTime(),
                        LAST_UPDATED_BY = c.String(),
                        CREATION_DATE = c.DateTime(),
                        CREATED_BY = c.String(),
                        SESSION_NUMBER = c.Double(),
                        START_DATE = c.DateTime(),
                        END_DATE = c.DateTime(),
                        DESCRIPTION = c.String(),
                        LAST_LOGON_DATE = c.DateTime(),
                        PASSWORD_DATE = c.DateTime(),
                        PASSWORD_ACCESSES_LEFT = c.Double(),
                        PASSWORD_LIFESPAN_ACCESSES = c.Int(),
                        PASSWORD_LIFESPAN_DAYS = c.Double(),
                        EMPLOYEE_ID = c.Double(),
                        EMAIL_ADDRESS = c.String(),
                        FAX = c.String(),
                        CUSTOMER_ID = c.Double(),
                        SUPPLIER_ID = c.Double(),
                        SECURITY_GROUP_ID = c.Double(),
                        GCN_CODE_COMBINATION_ID = c.Int(),
                        PERSON_PARTY_ID = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        AccountName = c.String(),
                        AccountPassword = c.String(),
                        MappingType = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_OEBS_RoleInfo",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        NAME = c.String(),
                        DISPLAY_NAME = c.String(),
                        DESCRIPTION = c.String(),
                        NOTIFICATION_PREFERENCE = c.String(),
                        LANGUAGE = c.String(),
                        TERRITORY = c.String(),
                        EMAIL_ADDRESS = c.String(),
                        FAX = c.String(),
                        ORIG_SYSTEM = c.String(),
                        ORIG_SYSTEM_ID = c.Double(),
                        START_DATE = c.DateTime(),
                        STATUS = c.String(),
                        EXPIRATION_DATE = c.DateTime(),
                        SECURITY_GROUP_ID = c.Double(),
                        USER_FLAG = c.String(),
                        PARTITION_ID = c.Double(),
                        CREATED_BY = c.Double(),
                        CREATION_DATE = c.DateTime(),
                        LAST_UPDATED_BY = c.Double(),
                        LAST_UPDATE_DATE = c.DateTime(),
                        LAST_UPDATE_LOGIN = c.Double(),
                        OWNER_TAG = c.String(),
                        PARENT_ORIG_SYSTEM = c.String(),
                        PARENT_ORIG_SYSTEM_ID = c.Double(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        version = c.Int(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.DI_OEBS_ResponsibilitiesInfoHistory",
                c => new
                    {
                        ResponsibilityHistoryId = c.Int(nullable: false, identity: true),
                        ResponsibilityId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        ResponsibilityName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ResponsibilityHistoryId);
            
            CreateTable(
                "dbo.DI_OEBS_RoleInfoHistory",
                c => new
                    {
                        RoleHistoryId = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        RoleName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleHistoryId);
            
            CreateTable(
                "dbo.DI_OEBS_SubmenusInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LANGUAGE = c.String(),
                        MENU_ID = c.Double(),
                        ENTRY_SEQUENCE = c.Int(),
                        LAST_UPDATE_DATE = c.DateTime(),
                        LAST_UPDATE_BY = c.Double(),
                        LAST_UPDATE_LOGIN = c.Double(),
                        CREATION_DATE = c.DateTime(),
                        CREATED_BY = c.Double(),
                        PROMPT = c.String(),
                        DESCRIPTION = c.String(),
                        SOURCE_LANG = c.String(),
                        SECURITY_GROUP_ID = c.Int(),
                        EndPointSystemId = c.Int(nullable: false),
                        TenantId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_OEBS_UserInfoHistory",
                c => new
                    {
                        OEBSUserInfoHistoryid = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        DIUserId = c.Int(),
                        USER_ID = c.String(),
                        USER_NAME = c.String(),
                        LAST_UPDATE_DATE = c.DateTime(),
                        LAST_UPDATED_BY = c.String(),
                        CREATION_DATE = c.DateTime(),
                        CREATED_BY = c.String(),
                        SESSION_NUMBER = c.Double(),
                        START_DATE = c.DateTime(),
                        END_DATE = c.DateTime(),
                        DESCRIPTION = c.String(),
                        LAST_LOGON_DATE = c.DateTime(),
                        PASSWORD_DATE = c.DateTime(),
                        PASSWORD_ACCESSES_LEFT = c.Double(),
                        PASSWORD_LIFESPAN_ACCESSES = c.Int(),
                        PASSWORD_LIFESPAN_DAYS = c.Double(),
                        EMPLOYEE_ID = c.Double(),
                        EMAIL_ADDRESS = c.String(),
                        FAX = c.String(),
                        CUSTOMER_ID = c.Double(),
                        SUPPLIER_ID = c.Double(),
                        SECURITY_GROUP_ID = c.Double(),
                        GCN_CODE_COMBINATION_ID = c.Int(),
                        PERSON_PARTY_ID = c.Int(),
                        Version = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        AccountName = c.String(),
                        AccountPassword = c.String(),
                        MappingType = c.String(),
                        AuditUser = c.String(),
                        AuditDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OEBSUserInfoHistoryid);
            
            CreateTable(
                "dbo.DI_OEBS_XLProvMatrix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyCode = c.String(),
                        DepartmentCode = c.String(),
                        DivisionCode = c.String(),
                        OrganisationId = c.String(),
                        OrganisationName = c.String(),
                        CostCenterCode = c.String(),
                        CostDistributionCode = c.String(),
                        Roles = c.String(),
                        Responsibilities = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_OUChangeSnapshot",
                c => new
                    {
                        SnapshotId = c.Int(nullable: false, identity: true),
                        BatchId = c.String(),
                        UserId = c.Int(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        EndpointUserId = c.Int(nullable: false),
                        EntitlementType = c.Int(nullable: false),
                        Entitlement = c.String(),
                        EnteredDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SnapshotId);
            
            CreateTable(
                "dbo.DI_M_PasswordPolicy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        EndpointSystemTypeId = c.Int(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        PasswordLength = c.Int(),
                        PasswordMinimumLength = c.Int(),
                        PasswordMaximumLength = c.Int(),
                        NoOfSpecialCharacters = c.Int(),
                        SpecialCharNotAllowed = c.Boolean(nullable: false),
                        NoOfAlphabets = c.Int(),
                        AlphabetsNotAllowed = c.Boolean(nullable: false),
                        NoOfNumericCharacters = c.Int(),
                        NumericCharacterNotAllowed = c.Boolean(nullable: false),
                        NotAllowedCharacters = c.String(),
                        NoOfDaysPasswordChangeAllowed = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        NotAllowedKeywords = c.String(),
                        UserNameNotAllowedInPassword = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId, cascadeDelete: true)
                .Index(t => t.EndpointSystemTypeId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_PasswordSyncConfigurationDetail",
                c => new
                    {
                        ConfigDetailId = c.Int(nullable: false, identity: true),
                        ConfigId = c.Int(nullable: false),
                        EndpointSystemTypeId = c.Int(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        AllowSync = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ConfigDetailId)
                .ForeignKey("dbo.DI_M_PasswordSyncConfiguration", t => t.ConfigId, cascadeDelete: true)
                .Index(t => t.ConfigId);
            
            CreateTable(
                "dbo.DI_M_PasswordSyncConfiguration",
                c => new
                    {
                        ConfigId = c.Int(nullable: false, identity: true),
                        AllowPasswordSyncThruFilter = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ConfigId);
            
            CreateTable(
                "dbo.DI_M_PreferenceItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Title = c.String(),
                        Number = c.String(),
                        Icon = c.String(),
                        Information = c.String(),
                        TargetUrl = c.String(),
                        Type = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Process",
                c => new
                    {
                        ProcessId = c.Int(nullable: false, identity: true),
                        ProcessType = c.Int(nullable: false),
                        ProcessTypeKeyId = c.Int(nullable: false),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        LastRunOn = c.DateTime(),
                        Status = c.String(),
                        ErrorMessage = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Scheduler_SchedulerId = c.Int(),
                    })
                .PrimaryKey(t => t.ProcessId)
                .ForeignKey("dbo.DI_M_Scheduler", t => t.Scheduler_SchedulerId)
                .Index(t => t.Scheduler_SchedulerId);
            
            CreateTable(
                "dbo.DI_M_Scheduler",
                c => new
                    {
                        SchedulerId = c.Int(nullable: false, identity: true),
                        ProcessName = c.String(),
                        StartDate = c.DateTime(nullable: false, storeType: "date"),
                        StartTime = c.Time(nullable: false, precision: 7),
                        Occurence = c.Int(),
                        ReccurEveryDays = c.Int(),
                        ReccurEveryWeeks = c.Int(),
                        WeekDay = c.String(),
                        ReccurEveryMonths = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SchedulerId);
            
            CreateTable(
                "dbo.DI_M_ProcessHistroy",
                c => new
                    {
                        HistroyID = c.Int(nullable: false, identity: true),
                        ProcessType = c.Int(nullable: false),
                        ProcessId = c.Int(nullable: false),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        Status = c.String(),
                        ErrorMessage = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.HistroyID);
            
            CreateTable(
                "dbo.DI_M_ProvisioningFormAD",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvisioningFormId = c.Long(),
                        UserLogonName = c.String(),
                        PswdChangeOnNextLogIn = c.Boolean(),
                        PasswordCannotChange = c.Boolean(),
                        IsPasswdNeverExpires = c.Boolean(),
                        IsAccountDisabled = c.Boolean(),
                        CreateHomeFolder = c.Boolean(),
                        AccountContainerRules = c.String(),
                        UserProfilePath = c.String(),
                        HomeFolderPath = c.String(),
                        HomeDrive = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Groups = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId)
                .Index(t => t.ProvisioningFormId);
            
            CreateTable(
                "dbo.DI_M_ProvisioningFormLINUX",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvisioningFormId = c.Long(),
                        UserLogonName = c.String(),
                        PswdChangeOnNextLogIn = c.Boolean(),
                        PasswordCannotChange = c.Boolean(),
                        IsPasswdNeverExpires = c.Boolean(),
                        IsAccountDisabled = c.Boolean(),
                        CreateHomeFolder = c.Boolean(),
                        AccountContainerRules = c.String(),
                        UserProfilePath = c.String(),
                        HomeFolderPath = c.String(),
                        HomeDrive = c.String(),
                        PasswordExpression = c.String(),
                        PasswordLength = c.Int(),
                        NoOfUppercaseChar = c.Int(),
                        NoOfLowercaseChar = c.Int(),
                        NoOfDigitChar = c.Int(),
                        SpecialCharacter = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Groups = c.String(),
                        PasswordChangeInterval = c.Int(nullable: false),
                        Comments = c.String(),
                        PasswordGracePeriod = c.Int(nullable: false),
                        IsPasswordEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId)
                .Index(t => t.ProvisioningFormId);
            
            CreateTable(
                "dbo.DI_M_ProvisioningFormLNS",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvisioningFormId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Groups = c.String(),
                        MailTemplateName = c.String(),
                        MailACLManager = c.String(),
                        CertIdFilePassword = c.String(),
                        AssignedPolicy = c.String(),
                        IdmPolicy = c.Boolean(nullable: false),
                        CheckPassword = c.Boolean(nullable: false),
                        PasswordChangeInterval = c.Int(nullable: false),
                        PasswordGracePeriod = c.Int(nullable: false),
                        MailChargeable = c.Boolean(nullable: false),
                        AllowForeignSync = c.Boolean(nullable: false),
                        ForceUserToChangePwdOnNextLogin = c.Boolean(nullable: false),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId)
                .Index(t => t.ProvisioningFormId);
            
            CreateTable(
                "dbo.DI_M_ProvisioningFormOracleEBS",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvisioningFormId = c.Long(),
                        UserLogonName = c.String(),
                        PswdChangeOnNextLogIn = c.Boolean(),
                        AllowPasswordChange = c.Boolean(),
                        IsPasswdNeverExpires = c.Boolean(),
                        IsAccountDisabled = c.Boolean(),
                        AccountContainerRules = c.String(),
                        PasswordExpression = c.String(),
                        PasswordLength = c.Int(),
                        NoOfUppercaseChar = c.Int(),
                        NoOfLowercaseChar = c.Int(),
                        NoOfDigitChar = c.Int(),
                        SpecialCharacter = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Roles = c.String(),
                        Responsibilities = c.String(),
                        MailTemplateName = c.String(),
                        MailACLManager = c.String(),
                        CertIdFilePassword = c.String(),
                        AssignedPolicy = c.String(),
                        IdmPolicy = c.Boolean(nullable: false),
                        CheckPassword = c.Boolean(nullable: false),
                        PasswordChangeInterval = c.Int(nullable: false),
                        PasswordGracePeriod = c.Int(nullable: false),
                        MailChargeable = c.Boolean(nullable: false),
                        AllowForeignSync = c.Boolean(nullable: false),
                        ForceUserToChangePwdOnNextLogin = c.Boolean(nullable: false),
                        Comments = c.String(),
                        IsPasswordEnabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId)
                .Index(t => t.ProvisioningFormId);
            
            CreateTable(
                "dbo.DI_M_ProvisioningFormSAPEP",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvisioningFormId = c.Long(),
                        UserLogonName = c.String(),
                        PswdChangeOnNextLogIn = c.Boolean(),
                        AllowPasswordChange = c.Boolean(),
                        IsPasswdNeverExpires = c.Boolean(),
                        IsAccountDisabled = c.Boolean(),
                        roles = c.String(),
                        Groups = c.String(),
                        UserLanguage = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId)
                .Index(t => t.ProvisioningFormId);
            
            CreateTable(
                "dbo.DI_M_ProvisioningFormSAP",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProvisioningFormId = c.Long(),
                        roles = c.String(),
                        parameters = c.String(),
                        start_menu = c.String(),
                        OutputDevice = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        TimeZone = c.String(),
                        DefaultUserGroup = c.String(),
                        NewUser_DecimalNotation = c.String(),
                        LogonLanguage = c.String(),
                        FormatCountry = c.String(),
                        DateFormat = c.String(),
                        TimeFormat = c.String(),
                        UserType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId)
                .Index(t => t.ProvisioningFormId);
            
            CreateTable(
                "dbo.DI_Log_Provisioning",
                c => new
                    {
                        ProvisioningLogId = c.Int(nullable: false, identity: true),
                        AccessRequestId = c.Long(),
                        LogLevel = c.String(),
                        EventType = c.Int(nullable: false),
                        EndpointSystemTypeName = c.String(),
                        EndpointSystemId = c.Int(nullable: false),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProvisioningLogId);
            
            CreateTable(
                "dbo.DI_M_QuickLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Category = c.String(),
                        TabMenuId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_TabMenu", t => t.TabMenuId, cascadeDelete: true)
                .Index(t => t.TabMenuId);
            
            CreateTable(
                "dbo.DI_M_RequestTypes",
                c => new
                    {
                        RequestTypeId = c.Byte(nullable: false),
                        RequestTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RequestTypeId);
            
            CreateTable(
                "dbo.DI_M_RoleHistory",
                c => new
                    {
                        RoleHistoryId = c.Long(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        RoleName = c.String(),
                        RoleDescription = c.String(),
                        IsDefaultRole = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleHistoryId);
            
            CreateTable(
                "dbo.DI_M_RoleMatrix",
                c => new
                    {
                        RoleMatrixId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        CompanyName = c.String(),
                        CostCenterId = c.Int(nullable: false),
                        CostCenter = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        DepartmentName = c.String(),
                        DivisionId = c.Int(nullable: false),
                        DivisionName = c.String(),
                        OrganisationId = c.Int(nullable: false),
                        SectionName = c.String(),
                        BusinessRoleId = c.Int(nullable: false),
                        Designation = c.String(),
                        EmployeeTerm = c.String(),
                        version = c.Byte(nullable: false),
                        IncludeDefaultsFromAllDeptAndDsg = c.Boolean(nullable: false),
                        IncludeDefaultsFromSpcDeptAndAllDsg = c.Boolean(nullable: false),
                        IncludeDefaultsFromAllDeptAndSpcDsg = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleMatrixId)
                .ForeignKey("dbo.DI_M_Designation", t => t.BusinessRoleId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_Company", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_Department", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_Division", t => t.DivisionId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_Organisation", t => t.OrganisationId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.DepartmentId)
                .Index(t => t.DivisionId)
                .Index(t => t.OrganisationId)
                .Index(t => t.BusinessRoleId);
            
            CreateTable(
                "dbo.DI_M_RoleMatrixDetail",
                c => new
                    {
                        RoleMatrixDetailId = c.Long(nullable: false, identity: true),
                        RoleMatrixId = c.Int(nullable: false),
                        ProvisioningFormId = c.Long(),
                        ProvisioningRoleId = c.Int(),
                        IsFromDefault = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RoleMatrixDetailId)
                .ForeignKey("dbo.DI_M_ProvisioningForm", t => t.ProvisioningFormId)
                .ForeignKey("dbo.DI_M_RoleMatrix", t => t.RoleMatrixId, cascadeDelete: true)
                .Index(t => t.RoleMatrixId)
                .Index(t => t.ProvisioningFormId);
            
            CreateTable(
                "dbo.DI_M_RoleTypes",
                c => new
                    {
                        RoleTypeId = c.Byte(nullable: false),
                        RoleTypeDesc = c.String(),
                        ApplicableApprovalUserTypes = c.String(),
                        ApplicableRequestTypes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RoleTypeId);
            
            CreateTable(
                "dbo.Ruleset",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Name = c.String(),
                        MajorVersion = c.Int(nullable: false),
                        MinorVersion = c.Int(nullable: false),
                        RuleSet = c.String(),
                        Active = c.Boolean(nullable: false),
                        Status = c.Short(nullable: false),
                        AssemblyPath = c.String(),
                        ActivityName = c.String(),
                        ModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DI_SAP_ActivityInfo",
                c => new
                    {
                        ActivityId = c.Int(nullable: false, identity: true),
                        ActivityValue = c.String(),
                        ActivityDescription = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.ActivityId);
            
            CreateTable(
                "dbo.DI_SAP_AuthorizationProfile",
                c => new
                    {
                        AuthorizationID = c.Int(nullable: false, identity: true),
                        ProfileCode = c.String(),
                        ObjectCode = c.String(),
                        AuthorizationCode = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.AuthorizationID);
            
            CreateTable(
                "dbo.DI_SAP_AuthObjectMap",
                c => new
                    {
                        SapObjectId = c.Int(nullable: false, identity: true),
                        AuthorizationCode = c.String(),
                        ObjectCode = c.String(),
                        FieldCode = c.String(),
                        FromValue = c.String(),
                        ToValue = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SapObjectId);
            
            CreateTable(
                "dbo.DI_SAP_COMPOSITEPROFILEINFO",
                c => new
                    {
                        SAP_PROFILEID = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        PROFILE_CODE = c.String(),
                        PROFILE_DESC = c.String(),
                        SUBProfile_Code = c.String(),
                        TargetSysId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAP_PROFILEID);
            
            CreateTable(
                "dbo.DI_SAP_COMPOSITEROLEINFO",
                c => new
                    {
                        CompositeRole_Id = c.Int(nullable: false, identity: true),
                        CompositeRole_Code = c.String(),
                        CompositeRole_Desc = c.String(),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.CompositeRole_Id);
            
            CreateTable(
                "dbo.DI_SAP_SINGLEROLEINFO",
                c => new
                    {
                        SingleRole_Id = c.Int(nullable: false, identity: true),
                        SingleRole_Code = c.String(),
                        SingleRole_Desc = c.String(),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SingleRole_Id);
            
            CreateTable(
                "dbo.DI_SAP_TCODEINFO",
                c => new
                    {
                        TCODE_Id = c.Int(nullable: false, identity: true),
                        TCODE_Code = c.String(),
                        TCODE_Desc = c.String(),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.TCODE_Id);
            
            CreateTable(
                "dbo.DI_SAP_Configuration",
                c => new
                    {
                        SAPConfigurationID = c.Int(nullable: false, identity: true),
                        EndPointSystemId = c.Int(nullable: false),
                        HostName = c.String(),
                        IPAddress = c.String(),
                        DomainName = c.String(),
                        ServerName = c.String(),
                        DataSourceName = c.String(),
                        ClientNumber = c.Int(),
                        SystemNumber = c.Int(),
                        UserName = c.String(),
                        Password = c.String(),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SAPConfigurationID)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAP_XLCriticalTCodeMatrix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Transaction = c.String(),
                        TransactionDescription = c.String(),
                        TransactionOwner = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPEP_Configuration",
                c => new
                    {
                        SapEpConfigurationId = c.Int(nullable: false, identity: true),
                        EndPointSystemId = c.Int(nullable: false),
                        HostName = c.String(),
                        IPAddress = c.String(),
                        DomainName = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        WSDLUrl = c.String(),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.SapEpConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAPEP_GroupInfoHistory",
                c => new
                    {
                        GroupHistoryId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        GroupId = c.String(),
                        GroupName = c.String(),
                        GroupDescription = c.String(),
                        Extar1 = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                        RecordStatus = c.String(),
                    })
                .PrimaryKey(t => t.GroupHistoryId);
            
            CreateTable(
                "dbo.DI_SAPEP_GroupInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        GroupId = c.String(),
                        GroupName = c.String(),
                        GroupDescription = c.String(),
                        GroupUniqueName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                        IsXLUploaded = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPEP_UserInfo",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        UserName = c.String(),
                        UniqueName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Salutation = c.String(),
                        DisplayName = c.String(),
                        Email = c.String(),
                        Department = c.String(),
                        JobTitle = c.String(),
                        Title = c.String(),
                        Company = c.String(),
                        Cellphone = c.String(),
                        Telephone = c.String(),
                        Fax = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        Zip = c.String(),
                        Currency = c.String(),
                        Language = c.String(),
                        Password = c.String(),
                        UserLastLogOnTime = c.DateTime(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        IsCompanyUser = c.Boolean(),
                        UniqueId = c.String(),
                        DIUserId = c.Int(),
                        PwdResetCnt = c.Int(nullable: false),
                        Extra1 = c.String(),
                        Extra2 = c.String(),
                        Extra3 = c.String(),
                        IACMLastPasswordChange = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        MappingType = c.String(),
                        IfUnmappedMapTo = c.String(),
                        Version = c.Int(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAPEP_RoleInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        RoleId = c.String(),
                        RoleName = c.String(),
                        RoleDescription = c.String(),
                        UniqueRoleName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                        IsXLUploaded = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPEP_RoleInfoHistory",
                c => new
                    {
                        RoleHistoryId = c.Int(nullable: false, identity: true),
                        Id = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        RoleId = c.String(),
                        RoleName = c.String(),
                        RoleDescription = c.String(),
                        Extar1 = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        RecordStatus = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.RoleHistoryId);
            
            CreateTable(
                "dbo.DI_SAPEP_RoleInfoTemp",
                c => new
                    {
                        RoleInfoId = c.Int(nullable: false, identity: true),
                        RoleUniqueName = c.String(),
                        RoleFullName = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.RoleInfoId);
            
            CreateTable(
                "dbo.DI_SAPEP_UserInfoHistory",
                c => new
                    {
                        UserHistoryId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        UserName = c.String(),
                        UniqueName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Salutation = c.String(),
                        DisplayName = c.String(),
                        Email = c.String(),
                        Department = c.String(),
                        JobTitle = c.String(),
                        Title = c.String(),
                        Company = c.String(),
                        Cellphone = c.String(),
                        Telephone = c.String(),
                        Fax = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        Zip = c.String(),
                        Currency = c.String(),
                        Language = c.String(),
                        Password = c.String(),
                        UserLastLogOnTime = c.DateTime(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        IsCompanyUser = c.Boolean(),
                        UniqueId = c.String(),
                        DIUserId = c.Int(),
                        PwdResetCnt = c.Int(nullable: false),
                        Extra1 = c.String(),
                        Extra2 = c.String(),
                        Extra3 = c.String(),
                        IACMLastPasswordChange = c.DateTime(),
                        MappingType = c.String(),
                        IfUnmappedMapTo = c.String(),
                        Version = c.Int(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AuditUser = c.String(),
                        AuditDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserHistoryId);
            
            CreateTable(
                "dbo.DI_SAP_FIELDVALUEMAPPING",
                c => new
                    {
                        SAPFieldValueMapId = c.Int(nullable: false, identity: true),
                        RoleCode = c.String(),
                        FieldCode = c.String(),
                        FromValue = c.String(),
                        ToValue = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAPFieldValueMapId);
            
            CreateTable(
                "dbo.DI_SAP_GROUPHISTORYINFO",
                c => new
                    {
                        SAP_GROUPID = c.Int(nullable: false, identity: true),
                        HistoryId = c.Int(),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        GROUP_CODE = c.String(),
                        GROUP_DESC = c.String(),
                        TargetSysId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAP_GROUPID);
            
            CreateTable(
                "dbo.DI_SAP_GROUPINFO",
                c => new
                    {
                        SAP_GROUPID = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        GROUP_CODE = c.String(),
                        GROUP_DESC = c.String(),
                        TargetSysId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAP_GROUPID);
            
            CreateTable(
                "dbo.DI_SAP_UserInfo",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        Initials = c.String(),
                        Department = c.String(),
                        E_Mail = c.String(),
                        Pers_No = c.String(),
                        Tel1_Numbr = c.String(),
                        Function0 = c.String(),
                        Title = c.String(),
                        City = c.String(),
                        Namcountry = c.String(),
                        Pboxcit_No = c.String(),
                        Postl_Cod1 = c.String(),
                        Street = c.String(),
                        Start_Menu = c.String(),
                        ActivityGroup = c.String(),
                        LastLoginTime = c.DateTime(),
                        UserLogonName = c.String(),
                        AccntLockNoPwd = c.String(),
                        AccntLocDurPwd = c.String(),
                        LOCKEDBYINTRUDER = c.String(),
                        EMPLOYEEID = c.String(),
                        EMPLOYEETYPE = c.String(),
                        PASSWORDEXPTIME = c.DateTime(),
                        LOGINDISABLED = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        DomainName = c.String(),
                        TargetSysId = c.Int(nullable: false),
                        PwdResetCnt = c.Int(nullable: false),
                        DIUserId = c.Int(),
                        IACMLastPasswordChange = c.DateTime(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedBy = c.String(),
                        Position = c.String(),
                        CostCenter = c.String(),
                        validFromDate = c.DateTime(),
                        validToDate = c.DateTime(),
                        SapUserType = c.String(),
                        IsUserLocked = c.Boolean(nullable: false),
                        MappingType = c.String(),
                        IfUnmappedMapTo = c.String(),
                        Version = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        NeedEPGroupAssignment = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAP_PARAMETERINFO",
                c => new
                    {
                        SAP_ParameterID = c.Int(nullable: false, identity: true),
                        PARAM_CODE = c.String(),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        PARAM_DESC = c.String(),
                        PARAM_VALUE = c.String(),
                        TargetSysId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAP_ParameterID);
            
            CreateTable(
                "dbo.DI_SAP_PROFILEINFO",
                c => new
                    {
                        SAP_PROFILEID = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        PROFILE_CODE = c.String(),
                        PROFILE_DESC = c.String(),
                        TargetSysId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAP_PROFILEID);
            
            CreateTable(
                "dbo.DI_SAP_ROLEINFO",
                c => new
                    {
                        SAP_ROLEID = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        ROLE_CODE = c.String(),
                        ROLE_DESC = c.String(),
                        TargetSysId = c.String(),
                        RoleType = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAP_ROLEID);
            
            CreateTable(
                "dbo.DI_SAP_SAPHRP1001",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Int(nullable: false),
                        ActualObjectCode = c.String(),
                        RelatedObjectType = c.String(),
                        RelatedObjectCode = c.String(),
                        BeginDate = c.String(),
                        EndDate = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_Objects",
                c => new
                    {
                        SapObjectId = c.Int(nullable: false, identity: true),
                        SapObjectCode = c.String(),
                        FieldCode1 = c.String(),
                        FieldCode2 = c.String(),
                        FieldCode3 = c.String(),
                        FieldCode4 = c.String(),
                        FieldCode5 = c.String(),
                        FieldCode6 = c.String(),
                        FieldCode7 = c.String(),
                        FieldCode8 = c.String(),
                        FieldCode9 = c.String(),
                        FieldCode0 = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SapObjectId);
            
            CreateTable(
                "dbo.DI_SAP_ObjectValues",
                c => new
                    {
                        SapObjectValueId = c.Int(nullable: false, identity: true),
                        SapObjectType = c.String(),
                        SapObjectCode = c.String(),
                        SapOkFlag = c.String(),
                        ModificationID = c.String(),
                        OrgName = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SapObjectValueId);
            
            CreateTable(
                "dbo.DI_SAP_PARAMETERHISTORYINFO",
                c => new
                    {
                        SAP_ParameterID = c.Int(nullable: false, identity: true),
                        PARAM_CODE = c.String(),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        PARAM_DESC = c.String(),
                        PARAM_VALUE = c.String(),
                        TargetSysId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAP_ParameterID);
            
            CreateTable(
                "dbo.DI_SAP_Position",
                c => new
                    {
                        PositionId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Code = c.String(),
                        Name = c.String(),
                        BEGDA = c.DateTime(),
                        ENDDA = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.PositionId);
            
            CreateTable(
                "dbo.DI_SAP_XLPositionsMatrix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Remarks = c.String(),
                        PositionText = c.String(),
                        PositionCode = c.String(),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_RoleCatalog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Remarks = c.String(maxLength: 250),
                        Application = c.String(maxLength: 250),
                        Module = c.String(maxLength: 250),
                        SubModule = c.String(maxLength: 250),
                        BusinessRoleName = c.String(maxLength: 250),
                        TechnicalRole = c.String(maxLength: 250),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        IsModified = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_ROLETCODEMAPPING",
                c => new
                    {
                        SapRoleTCodeMappingId = c.Int(nullable: false, identity: true),
                        RoleCode = c.String(),
                        TCode = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SapRoleTCodeMappingId);
            
            CreateTable(
                "dbo.DI_SAP_RoleToAuthMap",
                c => new
                    {
                        SAPRoleToAuthMapId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        ObjectCode = c.String(),
                        AuthCode = c.String(),
                        Variant = c.String(),
                        BiwMenuCounter = c.String(),
                        FieldCode = c.String(),
                        ProfileGeneratorVariant = c.String(),
                        FromValue = c.String(),
                        ToValue = c.String(),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SAPRoleToAuthMapId);
            
            CreateTable(
                "dbo.DI_SAP_TCodeObject",
                c => new
                    {
                        TCodeObjectId = c.Int(nullable: false, identity: true),
                        ObjectType = c.String(),
                        FieldValue = c.String(),
                        ObjectCode = c.String(),
                        FieldCode = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.TCodeObjectId);
            
            CreateTable(
                "dbo.DI_SAP_UserHistoryInfo",
                c => new
                    {
                        UserHistoryId = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        Initials = c.String(),
                        Department = c.String(),
                        E_Mail = c.String(),
                        Pers_No = c.String(),
                        Tel1_Numbr = c.String(),
                        Function0 = c.String(),
                        Title = c.String(),
                        City = c.String(),
                        Namcountry = c.String(),
                        Pboxcit_No = c.String(),
                        Postl_Cod1 = c.String(),
                        Street = c.String(),
                        Start_Menu = c.String(),
                        ActivityGroup = c.String(),
                        LastLoginTime = c.DateTime(),
                        UserLogonName = c.String(),
                        AccntLockNoPwd = c.String(),
                        AccntLocDurPwd = c.String(),
                        LOCKEDBYINTRUDER = c.String(),
                        EMPLOYEEID = c.String(),
                        EMPLOYEETYPE = c.String(),
                        PASSWORDEXPTIME = c.DateTime(),
                        LOGINDISABLED = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        DomainName = c.String(),
                        TargetSysId = c.Int(nullable: false),
                        PwdResetCnt = c.Int(nullable: false),
                        DIUserId = c.Int(),
                        SpecialAccount = c.Boolean(nullable: false),
                        IACMLastPasswordChange = c.DateTime(),
                        Position = c.String(),
                        CostCenter = c.String(),
                        validFromDate = c.DateTime(),
                        validToDate = c.DateTime(),
                        SapUserType = c.String(),
                        IsUserLocked = c.Boolean(nullable: false),
                        MappingType = c.String(),
                        IfUnmappedMapTo = c.String(),
                        Version = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        NeedEPGroupAssignment = c.Boolean(nullable: false),
                        AuditUser = c.String(),
                        AuditDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserHistoryId);
            
            CreateTable(
                "dbo.DI_SAP_UserProfile",
                c => new
                    {
                        SapUserProfileId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        ProfileName = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        TenantId = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.SapUserProfileId);
            
            CreateTable(
                "dbo.DI_SAP_XLProvMatrix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyCode = c.String(),
                        CompanyName = c.String(),
                        DepartmentCode = c.String(),
                        DepartmentName = c.String(),
                        DivisionCode = c.String(),
                        DivisionName = c.String(),
                        CostCenterCode = c.String(),
                        CostCenterName = c.String(),
                        DefaultHRMSGeneralRoles = c.String(),
                        DefaultEPGroup = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_LOG_Scheduler",
                c => new
                    {
                        SchedulerLogId = c.Int(nullable: false, identity: true),
                        LogLevel = c.String(),
                        JobType = c.String(),
                        NoOfJobs = c.Int(nullable: false),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SchedulerLogId);
            
            CreateTable(
                "dbo.DI_M_SODReport",
                c => new
                    {
                        SODReportId = c.Int(nullable: false, identity: true),
                        SODPolicyID = c.Int(),
                        FunctionId1 = c.Int(),
                        Role1 = c.String(),
                        FunctionItem1 = c.Int(),
                        FunctionType1 = c.String(),
                        FunctionId2 = c.Int(),
                        Role2 = c.String(),
                        FunctionItem2 = c.Int(),
                        FunctionType2 = c.String(),
                        AuthorizationCode1 = c.String(),
                        AuthorizationCode2 = c.String(),
                        ProfileCode1 = c.String(),
                        ProfileCode2 = c.String(),
                        RoleCode1 = c.String(),
                        RoleCode2 = c.String(),
                        ConflictId = c.Int(),
                        ConflictCode = c.String(),
                        UserId = c.Int(nullable: false),
                        UserName = c.String(),
                        RunCode = c.String(),
                        PassedObjectLevelLeft = c.Boolean(nullable: false),
                        PassedObjectLevelRight = c.Boolean(nullable: false),
                        TotalMatching = c.Int(nullable: false),
                        MitigationCode = c.String(),
                        SODRunPolicyId = c.Int(),
                        ExceptionId = c.Int(),
                    })
                .PrimaryKey(t => t.SODReportId)
                .ForeignKey("dbo.DI_M_SODConflictFunctions", t => t.ConflictId)
                .ForeignKey("dbo.DI_M_SODException", t => t.ExceptionId)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId1)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId2)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItem1)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItem2)
                .ForeignKey("dbo.DI_M_SODRunPolicy", t => t.SODRunPolicyId)
                .Index(t => t.FunctionId1)
                .Index(t => t.FunctionItem1)
                .Index(t => t.FunctionId2)
                .Index(t => t.FunctionItem2)
                .Index(t => t.ConflictId)
                .Index(t => t.SODRunPolicyId)
                .Index(t => t.ExceptionId);
            
            CreateTable(
                "dbo.DI_M_SODRunPolicy",
                c => new
                    {
                        SODRunPolicyId = c.Int(nullable: false, identity: true),
                        CompliancePolicyId = c.Int(),
                        UniqueRunCode = c.String(),
                        RiskLevelId = c.Int(),
                        RiskScoreFrom = c.Int(),
                        RiskScoreTo = c.Int(),
                        RunDate = c.DateTime(nullable: false),
                        RunAtObjectLevel = c.Boolean(),
                        WorkflowConfigurationId = c.Short(),
                        ExceptionCode = c.String(),
                    })
                .PrimaryKey(t => t.SODRunPolicyId)
                .ForeignKey("dbo.DI_M_CompliancePolicy", t => t.CompliancePolicyId)
                .ForeignKey("dbo.DI_M_SODRiskLevel", t => t.RiskLevelId)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId)
                .Index(t => t.CompliancePolicyId)
                .Index(t => t.RiskLevelId)
                .Index(t => t.WorkflowConfigurationId);
            
            CreateTable(
                "dbo.DI_M_SODRunPolicySelection",
                c => new
                    {
                        SODRunPolicySelectionId = c.Int(nullable: false, identity: true),
                        SODRunPolicyId = c.Int(nullable: false),
                        EndpointTypeName = c.String(),
                        LevelName = c.String(),
                        LevelValue = c.String(),
                    })
                .PrimaryKey(t => t.SODRunPolicySelectionId)
                .ForeignKey("dbo.DI_M_SODRunPolicy", t => t.SODRunPolicyId, cascadeDelete: true)
                .Index(t => t.SODRunPolicyId);
            
            CreateTable(
                "dbo.DI_M_SODConflict",
                c => new
                    {
                        SodConflictId = c.Int(nullable: false, identity: true),
                        SODConflictCode = c.String(),
                        SODConflictDescription = c.String(),
                        FunctionId1 = c.Int(),
                        FunctionId2 = c.Int(),
                        RiskCodeId = c.Int(nullable: false),
                        CompliancePolicyId = c.Int(nullable: false),
                        SodConflictType = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        SodRiskId_RiskId = c.Int(),
                    })
                .PrimaryKey(t => t.SodConflictId)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId1)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId2)
                .ForeignKey("dbo.DI_M_SODRiskCode", t => t.SodRiskId_RiskId)
                .Index(t => t.FunctionId1)
                .Index(t => t.FunctionId2)
                .Index(t => t.SodRiskId_RiskId);
            
            CreateTable(
                "dbo.DI_M_SODExecutionConfig",
                c => new
                    {
                        SODExecutionConfigId = c.Int(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Int(),
                        Level1 = c.String(),
                        Level2 = c.String(),
                        Level3 = c.String(),
                        Level4 = c.String(),
                        SODRunPolicyId = c.Int(),
                    })
                .PrimaryKey(t => t.SODExecutionConfigId)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId)
                .Index(t => t.EndpointSystemTypeId);
            
            CreateTable(
                "dbo.DI_M_SODPolicy",
                c => new
                    {
                        SODPolicyID = c.Int(nullable: false, identity: true),
                        SODPolicyName = c.String(),
                        SODPolicyDescription = c.String(),
                        PolicyTypeId = c.Int(nullable: false),
                        BindType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SODPolicyID)
                .ForeignKey("dbo.DI_M_ComplianceType", t => t.PolicyTypeId, cascadeDelete: true)
                .Index(t => t.PolicyTypeId);
            
            CreateTable(
                "dbo.DI_M_SubTask",
                c => new
                    {
                        SubTaskId = c.Long(nullable: false, identity: true),
                        RoleName = c.String(),
                        TCode = c.String(),
                        TCodeDescription = c.String(),
                        Remarks = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubTaskId);
            
            CreateTable(
                "dbo.DI_M_TaskDelegation",
                c => new
                    {
                        DelegationId = c.Int(nullable: false, identity: true),
                        ActionUser = c.Int(nullable: false),
                        DelegatedTo = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        DelegateAttestationTask = c.Boolean(nullable: false),
                        DelegateProvisioningTask = c.Boolean(nullable: false),
                        DelegateSODTask = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DelegationComments = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DelegationId)
                .ForeignKey("dbo.DI_M_User", t => t.DelegatedTo, cascadeDelete: true)
                .Index(t => t.DelegatedTo);
            
            CreateTable(
                "dbo.DI_M_TaskReviews",
                c => new
                    {
                        TaskReviewId = c.Long(nullable: false, identity: true),
                        TaskId = c.Long(nullable: false),
                        ReviewId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.TaskReviewId)
                .ForeignKey("dbo.DI_M_AttestationReview", t => t.ReviewId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_Task", t => t.TaskId, cascadeDelete: true)
                .Index(t => t.TaskId)
                .Index(t => t.ReviewId);
            
            CreateTable(
                "dbo.DI_M_Task",
                c => new
                    {
                        TaskId = c.Long(nullable: false, identity: true),
                        RequestTypeId = c.Byte(nullable: false),
                        AttestationId = c.Int(),
                        AccessRequestId = c.Long(),
                        SubTaskId = c.Long(),
                        ActionUserId = c.Int(),
                        ActionGroupId = c.Int(),
                        WorkflowStateId = c.Byte(nullable: false),
                        WorkflowConfigurationId = c.Short(nullable: false),
                        WorkflowInstanceId = c.Guid(nullable: false),
                        IsEscalated = c.Boolean(nullable: false),
                        IsNotResponded = c.Boolean(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        Action = c.Byte(nullable: false),
                        Approver = c.String(),
                        Comments = c.String(),
                        Remarks = c.String(),
                        IsDelegated = c.Boolean(nullable: false),
                        DelegatedFrom = c.Int(),
                        DelegatedBy = c.String(),
                        DelegationComments = c.String(),
                        DelegatedDate = c.DateTime(),
                        IsActionOnlyRequiredTask = c.Boolean(nullable: false),
                        IsGroupTask = c.Boolean(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AssignedTo = c.Int(),
                        AssignedBy = c.Int(),
                        GroupName = c.String(),
                    })
                .PrimaryKey(t => t.TaskId)
                .ForeignKey("dbo.DI_M_AccessRequest", t => t.AccessRequestId)
                .ForeignKey("dbo.DI_M_SubTask", t => t.SubTaskId)
                .Index(t => t.AccessRequestId)
                .Index(t => t.SubTaskId);
            
            CreateTable(
                "dbo.DI_M_Tenant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        TenantName = c.String(),
                        TenantDescription = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        Address3 = c.String(),
                        CreatedBy = c.String(),
                        CreateDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Config_General_TrustedSource",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttributeName = c.String(),
                        TrustedSource = c.Int(nullable: false),
                        TrustedSourceAttribute = c.String(),
                        SelfUpdate = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_UI",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        LogLevel = c.String(),
                        UserName = c.String(),
                        SessionId = c.String(),
                        IP = c.String(),
                        TabName = c.String(),
                        MenuName = c.String(),
                        UIAction = c.String(),
                        MessageText = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Unix_Configuration",
                c => new
                    {
                        UnixConfigurationId = c.Int(nullable: false, identity: true),
                        IPAddress = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndpointSystem_EndpointSystemId = c.Int(),
                    })
                .PrimaryKey(t => t.UnixConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystem_EndpointSystemId)
                .Index(t => t.EndpointSystem_EndpointSystemId);
            
            CreateTable(
                "dbo.DI_Unix_GroupInfoHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        GroupNumber = c.String(),
                        GroupName = c.String(),
                        GroupPassword = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Unix_GroupInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        GroupNumber = c.String(),
                        GroupName = c.String(),
                        GroupPassword = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Unix_UserInfo",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        UnixUserId = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        GroupId = c.String(),
                        UserIdInfo = c.String(),
                        HomeDirectory = c.String(),
                        CommandShell = c.String(),
                        DIUserId = c.Int(),
                        SpecialAccount = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(nullable: false),
                        AccountPassword = c.String(),
                        AccountName = c.String(),
                        IsLocked = c.Boolean(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        IsUserAccount = c.Boolean(nullable: false),
                        UserValidFrom = c.String(),
                        UserValidTo = c.String(),
                        Encrypt_Pwd = c.String(),
                        MappingType = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_Unix_ResourceInfoHistory",
                c => new
                    {
                        Resource_Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Resource_Name = c.String(),
                        Resource_Type = c.String(),
                        Resource_User = c.String(),
                        Resource_Group = c.String(),
                        Resource_Parent = c.String(),
                        Resource_Level = c.Int(nullable: false),
                        Resouce_AccessRights = c.String(),
                        Resouce_Size = c.String(),
                        Resouce_LastModified = c.String(),
                        MountDrive = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Resource_Id);
            
            CreateTable(
                "dbo.DI_Unix_ResourceInfo",
                c => new
                    {
                        Resource_Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Resource_Name = c.String(),
                        Resource_Type = c.String(),
                        Resource_User = c.String(),
                        Resource_Group = c.String(),
                        Resource_Parent = c.String(),
                        Resource_Level = c.Int(nullable: false),
                        Resouce_AccessRights = c.String(),
                        Resouce_Size = c.String(),
                        Resouce_LastModified = c.String(),
                        MountDrive = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Resource_Id);
            
            CreateTable(
                "dbo.DI_Unix_UserInfoHistory",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        HistoryId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        UnixUserId = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        GroupId = c.String(),
                        UserIdInfo = c.String(),
                        HomeDirectory = c.String(),
                        CommandShell = c.String(),
                        DIUserId = c.Int(),
                        SpecialAccount = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserDashboardPreference",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                        DashboardId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Dashboard", t => t.DashboardId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.DashboardId);
            
            CreateTable(
                "dbo.DI_M_UserFilterInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PagePath = c.String(),
                        GridId = c.String(),
                        UserId = c.Int(nullable: false),
                        Operand = c.String(),
                        ColumnName = c.String(),
                        FilterValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserGridFilterSettings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PagePath = c.String(),
                        GridID = c.String(),
                        Operand = c.String(),
                        ColumnName = c.String(),
                        FilterValue = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserGridInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PagePath = c.String(),
                        GridId = c.String(),
                        UserId = c.Int(nullable: false),
                        SortDirection = c.String(),
                        SortColumnName = c.String(),
                        FilterExpression = c.String(),
                        PageSize = c.Int(nullable: false),
                        PageIndex = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserGridSettings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PagePath = c.String(),
                        GridID = c.String(),
                        SortColumnName = c.String(),
                        SortDirection = c.String(),
                        FilterCondition = c.String(),
                        PageSize = c.Int(nullable: false),
                        PageIndex = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserQuickLink",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Name = c.String(),
                        TabMenuId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_TabMenu", t => t.TabMenuId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TabMenuId);
            
            CreateTable(
                "dbo.DI_VPN_Master",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationName = c.String(),
                        ProductionADGroups = c.String(),
                        QAADGroups = c.String(),
                        DevADGroups = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DiscoveryProcessEndpointObjects",
                c => new
                    {
                        DiscoveryProcess_DiscoveryProcessId = c.Int(nullable: false),
                        EndpointObjects_EndpointObjectId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DiscoveryProcess_DiscoveryProcessId, t.EndpointObjects_EndpointObjectId })
                .ForeignKey("dbo.DI_M_DiscoveryProcess", t => t.DiscoveryProcess_DiscoveryProcessId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_EndpointObjects", t => t.EndpointObjects_EndpointObjectId, cascadeDelete: true)
                .Index(t => t.DiscoveryProcess_DiscoveryProcessId)
                .Index(t => t.EndpointObjects_EndpointObjectId);
            
            CreateTable(
                "dbo.UserGroup",
                c => new
                    {
                        Group_GroupId = c.Int(nullable: false),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Group_GroupId, t.User_UserId })
                .ForeignKey("dbo.DI_M_Group", t => t.Group_GroupId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_User", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.Group_GroupId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Role_RoleId = c.Int(nullable: false),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleId, t.User_UserId })
                .ForeignKey("dbo.DI_M_Role", t => t.Role_RoleId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_User", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.Role_RoleId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.ADSUserInfoADSGroupInfo",
                c => new
                    {
                        ADSUserInfo_UserId = c.Int(nullable: false),
                        ADSGroupInfo_GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ADSUserInfo_UserId, t.ADSGroupInfo_GroupId })
                .ForeignKey("dbo.DI_ADS_UserInfo", t => t.ADSUserInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.DI_ADS_GroupInfo", t => t.ADSGroupInfo_GroupId, cascadeDelete: true)
                .Index(t => t.ADSUserInfo_UserId)
                .Index(t => t.ADSGroupInfo_GroupId);
            
            CreateTable(
                "dbo.SODConflictFunctionCompliancePolicy",
                c => new
                    {
                        SODConflictFunction_SodConflictFunctionId = c.Int(nullable: false),
                        CompliancePolicy_CompliancePolicyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SODConflictFunction_SodConflictFunctionId, t.CompliancePolicy_CompliancePolicyId })
                .ForeignKey("dbo.DI_M_SODConflictFunctions", t => t.SODConflictFunction_SodConflictFunctionId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_CompliancePolicy", t => t.CompliancePolicy_CompliancePolicyId, cascadeDelete: true)
                .Index(t => t.SODConflictFunction_SodConflictFunctionId)
                .Index(t => t.CompliancePolicy_CompliancePolicyId);
            
            CreateTable(
                "dbo.SODFunctionSODConflictFunction",
                c => new
                    {
                        SodFunction_SodFunctionId = c.Int(nullable: false),
                        SODConflictFunction_SodConflictFunctionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SodFunction_SodFunctionId, t.SODConflictFunction_SodConflictFunctionId })
                .ForeignKey("dbo.DI_M_SODFunction", t => t.SodFunction_SodFunctionId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_SODConflictFunctions", t => t.SODConflictFunction_SodConflictFunctionId, cascadeDelete: true)
                .Index(t => t.SodFunction_SodFunctionId)
                .Index(t => t.SODConflictFunction_SodConflictFunctionId);
            
            CreateTable(
                "dbo.SODMitigationSODRiskCode",
                c => new
                    {
                        SODMitigation_MitigationId = c.Int(nullable: false),
                        SODRiskCode_RiskId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SODMitigation_MitigationId, t.SODRiskCode_RiskId })
                .ForeignKey("dbo.DI_M_SODMitigation", t => t.SODMitigation_MitigationId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_SODRiskCode", t => t.SODRiskCode_RiskId, cascadeDelete: true)
                .Index(t => t.SODMitigation_MitigationId)
                .Index(t => t.SODRiskCode_RiskId);
            
            CreateTable(
                "dbo.LotusUserInfoLotusGroupInfo",
                c => new
                    {
                        LotusUserInfo_UserId = c.Int(nullable: false),
                        LotusGroupInfo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LotusUserInfo_UserId, t.LotusGroupInfo_Id })
                .ForeignKey("dbo.DI_LNS_Users", t => t.LotusUserInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.DI_LNS_Groups", t => t.LotusGroupInfo_Id, cascadeDelete: true)
                .Index(t => t.LotusUserInfo_UserId)
                .Index(t => t.LotusGroupInfo_Id);
            
            CreateTable(
                "dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo",
                c => new
                    {
                        OracleEBSUserInfo_UserId = c.Int(nullable: false),
                        OracleEBSResponsibilitiesInfo_ResponsibilityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OracleEBSUserInfo_UserId, t.OracleEBSResponsibilitiesInfo_ResponsibilityId })
                .ForeignKey("dbo.DI_OEBS_UserInfo", t => t.OracleEBSUserInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.DI_OEBS_ResponsibilitiesInfo", t => t.OracleEBSResponsibilitiesInfo_ResponsibilityId, cascadeDelete: true)
                .Index(t => t.OracleEBSUserInfo_UserId)
                .Index(t => t.OracleEBSResponsibilitiesInfo_ResponsibilityId);
            
            CreateTable(
                "dbo.OracleEBSRoleInfoOracleEBSUserInfo",
                c => new
                    {
                        OracleEBSRoleInfo_RoleId = c.Int(nullable: false),
                        OracleEBSUserInfo_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OracleEBSRoleInfo_RoleId, t.OracleEBSUserInfo_UserId })
                .ForeignKey("dbo.DI_OEBS_RoleInfo", t => t.OracleEBSRoleInfo_RoleId, cascadeDelete: true)
                .ForeignKey("dbo.DI_OEBS_UserInfo", t => t.OracleEBSUserInfo_UserId, cascadeDelete: true)
                .Index(t => t.OracleEBSRoleInfo_RoleId)
                .Index(t => t.OracleEBSUserInfo_UserId);
            
            CreateTable(
                "dbo.SAPSingleRoleInfoSAPCompositeRoleInfo",
                c => new
                    {
                        SAPSingleRoleInfo_SingleRole_Id = c.Int(nullable: false),
                        SAPCompositeRoleInfo_CompositeRole_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPSingleRoleInfo_SingleRole_Id, t.SAPCompositeRoleInfo_CompositeRole_Id })
                .ForeignKey("dbo.DI_SAP_SINGLEROLEINFO", t => t.SAPSingleRoleInfo_SingleRole_Id, cascadeDelete: true)
                .ForeignKey("dbo.DI_SAP_COMPOSITEROLEINFO", t => t.SAPCompositeRoleInfo_CompositeRole_Id, cascadeDelete: true)
                .Index(t => t.SAPSingleRoleInfo_SingleRole_Id)
                .Index(t => t.SAPCompositeRoleInfo_CompositeRole_Id);
            
            CreateTable(
                "dbo.SAPTCodeInfoSAPSingleRoleInfo",
                c => new
                    {
                        SAPTCodeInfo_TCODE_Id = c.Int(nullable: false),
                        SAPSingleRoleInfo_SingleRole_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPTCodeInfo_TCODE_Id, t.SAPSingleRoleInfo_SingleRole_Id })
                .ForeignKey("dbo.DI_SAP_TCODEINFO", t => t.SAPTCodeInfo_TCODE_Id, cascadeDelete: true)
                .ForeignKey("dbo.DI_SAP_SINGLEROLEINFO", t => t.SAPSingleRoleInfo_SingleRole_Id, cascadeDelete: true)
                .Index(t => t.SAPTCodeInfo_TCODE_Id)
                .Index(t => t.SAPSingleRoleInfo_SingleRole_Id);
            
            CreateTable(
                "dbo.SAPEPUserInfoSAPEPGroupInfo",
                c => new
                    {
                        SAPEPUserInfo_UserId = c.Int(nullable: false),
                        SAPEPGroupInfo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPEPUserInfo_UserId, t.SAPEPGroupInfo_Id })
                .ForeignKey("dbo.DI_SAPEP_UserInfo", t => t.SAPEPUserInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.DI_SAPEP_GroupInfo", t => t.SAPEPGroupInfo_Id, cascadeDelete: true)
                .Index(t => t.SAPEPUserInfo_UserId)
                .Index(t => t.SAPEPGroupInfo_Id);
            
            CreateTable(
                "dbo.SAPEPRoleInfoSAPEPUserInfo",
                c => new
                    {
                        SAPEPRoleInfo_Id = c.Int(nullable: false),
                        SAPEPUserInfo_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPEPRoleInfo_Id, t.SAPEPUserInfo_UserId })
                .ForeignKey("dbo.DI_SAPEP_RoleInfo", t => t.SAPEPRoleInfo_Id, cascadeDelete: true)
                .ForeignKey("dbo.DI_SAPEP_UserInfo", t => t.SAPEPUserInfo_UserId, cascadeDelete: true)
                .Index(t => t.SAPEPRoleInfo_Id)
                .Index(t => t.SAPEPUserInfo_UserId);
            
            CreateTable(
                "dbo.SAPUserinfoSAPGroupInfo",
                c => new
                    {
                        SAPUserinfo_UserId = c.Int(nullable: false),
                        SAPGroupInfo_SAP_GROUPID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPUserinfo_UserId, t.SAPGroupInfo_SAP_GROUPID })
                .ForeignKey("dbo.DI_SAP_UserInfo", t => t.SAPUserinfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.DI_SAP_GROUPINFO", t => t.SAPGroupInfo_SAP_GROUPID, cascadeDelete: true)
                .Index(t => t.SAPUserinfo_UserId)
                .Index(t => t.SAPGroupInfo_SAP_GROUPID);
            
            CreateTable(
                "dbo.SAPParameterInfoSAPUserinfo",
                c => new
                    {
                        SAPParameterInfo_SAP_ParameterID = c.Int(nullable: false),
                        SAPUserinfo_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPParameterInfo_SAP_ParameterID, t.SAPUserinfo_UserId })
                .ForeignKey("dbo.DI_SAP_PARAMETERINFO", t => t.SAPParameterInfo_SAP_ParameterID, cascadeDelete: true)
                .ForeignKey("dbo.DI_SAP_UserInfo", t => t.SAPUserinfo_UserId, cascadeDelete: true)
                .Index(t => t.SAPParameterInfo_SAP_ParameterID)
                .Index(t => t.SAPUserinfo_UserId);
            
            CreateTable(
                "dbo.SAPProfileInfoSAPUserinfo",
                c => new
                    {
                        SAPProfileInfo_SAP_PROFILEID = c.Int(nullable: false),
                        SAPUserinfo_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPProfileInfo_SAP_PROFILEID, t.SAPUserinfo_UserId })
                .ForeignKey("dbo.DI_SAP_PROFILEINFO", t => t.SAPProfileInfo_SAP_PROFILEID, cascadeDelete: true)
                .ForeignKey("dbo.DI_SAP_UserInfo", t => t.SAPUserinfo_UserId, cascadeDelete: true)
                .Index(t => t.SAPProfileInfo_SAP_PROFILEID)
                .Index(t => t.SAPUserinfo_UserId);
            
            CreateTable(
                "dbo.SAPRoleInfoSAPUserinfo",
                c => new
                    {
                        SAPRoleInfo_SAP_ROLEID = c.Int(nullable: false),
                        SAPUserinfo_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPRoleInfo_SAP_ROLEID, t.SAPUserinfo_UserId })
                .ForeignKey("dbo.DI_SAP_ROLEINFO", t => t.SAPRoleInfo_SAP_ROLEID, cascadeDelete: true)
                .ForeignKey("dbo.DI_SAP_UserInfo", t => t.SAPUserinfo_UserId, cascadeDelete: true)
                .Index(t => t.SAPRoleInfo_SAP_ROLEID)
                .Index(t => t.SAPUserinfo_UserId);
            
            CreateTable(
                "dbo.UnixUserInfoUnixGroupInfo",
                c => new
                    {
                        UnixUserInfo_UserId = c.Int(nullable: false),
                        UnixGroupInfo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UnixUserInfo_UserId, t.UnixGroupInfo_Id })
                .ForeignKey("dbo.DI_Unix_UserInfo", t => t.UnixUserInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.DI_Unix_GroupInfo", t => t.UnixGroupInfo_Id, cascadeDelete: true)
                .Index(t => t.UnixUserInfo_UserId)
                .Index(t => t.UnixGroupInfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_UserQuickLink", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserQuickLink", "TabMenuId", "dbo.DI_M_TabMenu");
            DropForeignKey("dbo.DI_M_UserGridSettings", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserGridInfo", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserGridFilterSettings", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserFilterInfo", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserDashboardPreference", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserDashboardPreference", "DashboardId", "dbo.DI_M_Dashboard");
            DropForeignKey("dbo.UnixUserInfoUnixGroupInfo", "UnixGroupInfo_Id", "dbo.DI_Unix_GroupInfo");
            DropForeignKey("dbo.UnixUserInfoUnixGroupInfo", "UnixUserInfo_UserId", "dbo.DI_Unix_UserInfo");
            DropForeignKey("dbo.DI_Unix_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_Unix_Configuration", "EndpointSystem_EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_TaskReviews", "TaskId", "dbo.DI_M_Task");
            DropForeignKey("dbo.DI_M_Task", "SubTaskId", "dbo.DI_M_SubTask");
            DropForeignKey("dbo.DI_M_Task", "AccessRequestId", "dbo.DI_M_AccessRequest");
            DropForeignKey("dbo.DI_M_TaskReviews", "ReviewId", "dbo.DI_M_AttestationReview");
            DropForeignKey("dbo.DI_M_TaskDelegation", "DelegatedTo", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODPolicy", "PolicyTypeId", "dbo.DI_M_ComplianceType");
            DropForeignKey("dbo.DI_M_SODExecutionConfig", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_SODConflict", "SodRiskId_RiskId", "dbo.DI_M_SODRiskCode");
            DropForeignKey("dbo.DI_M_SODConflict", "FunctionId2", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODConflict", "FunctionId1", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODRunPolicy", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropForeignKey("dbo.DI_M_SODRunPolicy", "RiskLevelId", "dbo.DI_M_SODRiskLevel");
            DropForeignKey("dbo.DI_M_SODReport", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
            DropForeignKey("dbo.DI_M_SODException", "SODRunPolicy_SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
            DropForeignKey("dbo.DI_M_SODRunPolicySelection", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
            DropForeignKey("dbo.DI_M_SODRunPolicy", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.DI_M_SODReport", "FunctionItem2", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODReport", "FunctionItem1", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODReport", "FunctionId2", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODReport", "FunctionId1", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODReport", "ExceptionId", "dbo.DI_M_SODException");
            DropForeignKey("dbo.DI_M_SODReport", "ConflictId", "dbo.DI_M_SODConflictFunctions");
            DropForeignKey("dbo.SAPRoleInfoSAPUserinfo", "SAPUserinfo_UserId", "dbo.DI_SAP_UserInfo");
            DropForeignKey("dbo.SAPRoleInfoSAPUserinfo", "SAPRoleInfo_SAP_ROLEID", "dbo.DI_SAP_ROLEINFO");
            DropForeignKey("dbo.SAPProfileInfoSAPUserinfo", "SAPUserinfo_UserId", "dbo.DI_SAP_UserInfo");
            DropForeignKey("dbo.SAPProfileInfoSAPUserinfo", "SAPProfileInfo_SAP_PROFILEID", "dbo.DI_SAP_PROFILEINFO");
            DropForeignKey("dbo.SAPParameterInfoSAPUserinfo", "SAPUserinfo_UserId", "dbo.DI_SAP_UserInfo");
            DropForeignKey("dbo.SAPParameterInfoSAPUserinfo", "SAPParameterInfo_SAP_ParameterID", "dbo.DI_SAP_PARAMETERINFO");
            DropForeignKey("dbo.SAPUserinfoSAPGroupInfo", "SAPGroupInfo_SAP_GROUPID", "dbo.DI_SAP_GROUPINFO");
            DropForeignKey("dbo.SAPUserinfoSAPGroupInfo", "SAPUserinfo_UserId", "dbo.DI_SAP_UserInfo");
            DropForeignKey("dbo.DI_SAP_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.SAPEPRoleInfoSAPEPUserInfo", "SAPEPUserInfo_UserId", "dbo.DI_SAPEP_UserInfo");
            DropForeignKey("dbo.SAPEPRoleInfoSAPEPUserInfo", "SAPEPRoleInfo_Id", "dbo.DI_SAPEP_RoleInfo");
            DropForeignKey("dbo.SAPEPUserInfoSAPEPGroupInfo", "SAPEPGroupInfo_Id", "dbo.DI_SAPEP_GroupInfo");
            DropForeignKey("dbo.SAPEPUserInfoSAPEPGroupInfo", "SAPEPUserInfo_UserId", "dbo.DI_SAPEP_UserInfo");
            DropForeignKey("dbo.DI_SAPEP_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SAPEP_Configuration", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SAP_Configuration", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.SAPTCodeInfoSAPSingleRoleInfo", "SAPSingleRoleInfo_SingleRole_Id", "dbo.DI_SAP_SINGLEROLEINFO");
            DropForeignKey("dbo.SAPTCodeInfoSAPSingleRoleInfo", "SAPTCodeInfo_TCODE_Id", "dbo.DI_SAP_TCODEINFO");
            DropForeignKey("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", "SAPCompositeRoleInfo_CompositeRole_Id", "dbo.DI_SAP_COMPOSITEROLEINFO");
            DropForeignKey("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", "SAPSingleRoleInfo_SingleRole_Id", "dbo.DI_SAP_SINGLEROLEINFO");
            DropForeignKey("dbo.DI_M_RoleMatrixDetail", "RoleMatrixId", "dbo.DI_M_RoleMatrix");
            DropForeignKey("dbo.DI_M_RoleMatrixDetail", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_RoleMatrix", "OrganisationId", "dbo.DI_M_Organisation");
            DropForeignKey("dbo.DI_M_RoleMatrix", "DivisionId", "dbo.DI_M_Division");
            DropForeignKey("dbo.DI_M_RoleMatrix", "DepartmentId", "dbo.DI_M_Department");
            DropForeignKey("dbo.DI_M_RoleMatrix", "CompanyId", "dbo.DI_M_Company");
            DropForeignKey("dbo.DI_M_RoleMatrix", "BusinessRoleId", "dbo.DI_M_Designation");
            DropForeignKey("dbo.DI_M_WorkflowConfiguration", "RequestTypeId", "dbo.DI_M_RequestTypes");
            DropForeignKey("dbo.DI_M_QuickLink", "TabMenuId", "dbo.DI_M_TabMenu");
            DropForeignKey("dbo.DI_M_ProvisioningFormSAP", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_ProvisioningFormSAPEP", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_ProvisioningFormOracleEBS", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_ProvisioningFormLNS", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_ProvisioningFormLINUX", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_ProvisioningFormAD", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_Process", "Scheduler_SchedulerId", "dbo.DI_M_Scheduler");
            DropForeignKey("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", "dbo.DI_M_PasswordSyncConfiguration");
            DropForeignKey("dbo.DI_M_PasswordPolicy", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_PasswordPolicy", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.OracleEBSRoleInfoOracleEBSUserInfo", "OracleEBSUserInfo_UserId", "dbo.DI_OEBS_UserInfo");
            DropForeignKey("dbo.OracleEBSRoleInfoOracleEBSUserInfo", "OracleEBSRoleInfo_RoleId", "dbo.DI_OEBS_RoleInfo");
            DropForeignKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_ResponsibilityId", "dbo.DI_OEBS_ResponsibilitiesInfo");
            DropForeignKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSUserInfo_UserId", "dbo.DI_OEBS_UserInfo");
            DropForeignKey("dbo.DI_OEBS_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_OEBS_Configuration", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.LotusUserInfoLotusGroupInfo", "LotusGroupInfo_Id", "dbo.DI_LNS_Groups");
            DropForeignKey("dbo.LotusUserInfoLotusGroupInfo", "LotusUserInfo_UserId", "dbo.DI_LNS_Users");
            DropForeignKey("dbo.DI_LNS_Users", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_LNS_Configuration", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_INH_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_INH_Configuration", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_INH_Permission", "EndPointSystemID", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_GlobalAnswer", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_EmployeeEventTrigger", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_DiscoveryJobConfiguration", "EndpointObjectId", "dbo.DI_M_EndpointObjects");
            DropForeignKey("dbo.DI_M_DiscoveryJobConfiguration", "DiscoveryProcessId", "dbo.DI_M_DiscoveryProcess");
            DropForeignKey("dbo.DI_M_DefaultRoleMenu", "TabMenuId", "dbo.DI_M_TabMenu");
            DropForeignKey("dbo.DI_M_TabMenu", "PageId", "dbo.DI_M_Page");
            DropForeignKey("dbo.DI_M_DefaultRoleMenu", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_DefaultRoleDashboard", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_DefaultRoleDashboard", "DashBoardId", "dbo.DI_M_Dashboard");
            DropForeignKey("dbo.DI_M_DashboardAccessRights", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_DashboardAccessRights", "DashboardId", "dbo.DI_M_Dashboard");
            DropForeignKey("dbo.DI_M_CompliancePolicy", "OwnerId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODConflictFunctions", "RiskCodeId", "dbo.DI_M_SODRiskCode");
            DropForeignKey("dbo.DI_M_SODRiskCode", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODRiskCode", "RiskScoreId", "dbo.DI_M_SODRiskScore");
            DropForeignKey("dbo.DI_M_SODRiskCode", "RiskLevelId", "dbo.DI_M_SODRiskLevel");
            DropForeignKey("dbo.SODMitigationSODRiskCode", "SODRiskCode_RiskId", "dbo.DI_M_SODRiskCode");
            DropForeignKey("dbo.SODMitigationSODRiskCode", "SODMitigation_MitigationId", "dbo.DI_M_SODMitigation");
            DropForeignKey("dbo.DI_M_SODFunctionItem", "SodFunctionId", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODException", "SodFunction_SodFunctionId", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODException", "RiskOwnerId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "ExceptionId", "dbo.DI_M_SODException");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "FunctionItemId2", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "FunctionItemId1", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODFunctionItem", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_SODFunctionItem", "Endpointsystemid", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "FunctionId2", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "FunctionId1", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.SodFunctionSODConflictFunction", "SODConflictFunction_SodConflictFunctionId", "dbo.DI_M_SODConflictFunctions");
            DropForeignKey("dbo.SodFunctionSODConflictFunction", "SodFunction_SodFunctionId", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.SODConflictFunctionCompliancePolicy", "CompliancePolicy_CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.SODConflictFunctionCompliancePolicy", "SODConflictFunction_SodConflictFunctionId", "dbo.DI_M_SODConflictFunctions");
            DropForeignKey("dbo.DI_M_AttestationReview", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_ReviewEndpointSystems", "ReviewId", "dbo.DI_M_AttestationReview");
            DropForeignKey("dbo.DI_M_ReviewEndpointSystems", "ReviewEndpointId", "dbo.DI_M_AttestationReview_EndpointSystem");
            DropForeignKey("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", "ReviewEndpointId", "dbo.DI_M_AttestationReview_EndpointSystem");
            DropForeignKey("dbo.DI_M_AttestationReview_EndpointSystem_L3Detail", "L2DetailId", "dbo.DI_M_AttestationReview_EndpointSystem_L2Detail");
            DropForeignKey("dbo.DI_M_AttestationProcessHistory", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_Attestation_Endpoint", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_Attestation", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropForeignKey("dbo.DI_M_WorkflowConfigurationDetail", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropForeignKey("dbo.DI_M_WorkflowConfiguration", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_AttestationProcess", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_Attestation_Endpoint", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_ADS_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.ADSUserInfoADSGroupInfo", "ADSGroupInfo_GroupId", "dbo.DI_ADS_GroupInfo");
            DropForeignKey("dbo.ADSUserInfoADSGroupInfo", "ADSUserInfo_UserId", "dbo.DI_ADS_UserInfo");
            DropForeignKey("dbo.DI_ADS_Configuration", "EndpointSystem_EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_AccessRights", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_AccessRights", "PagePermissionId", "dbo.DI_M_PagePermission");
            DropForeignKey("dbo.DI_M_PagePermission", "UrlId", "dbo.DI_M_PageUrl");
            DropForeignKey("dbo.DI_M_PagePermission", "PermissionId", "dbo.DI_M_Permission");
            DropForeignKey("dbo.DI_M_PagePermission", "PageId", "dbo.DI_M_Page");
            DropForeignKey("dbo.DI_M_Page", "PageUrlId", "dbo.DI_M_PageUrl");
            DropForeignKey("dbo.DI_M_AccessRequest", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_AccessRequest", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_ProvisioningForm", "ProvisioningPassword_Id", "dbo.DI_M_ProvisioningPassword");
            DropForeignKey("dbo.DI_M_ProvisioningFormGeneralData", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropForeignKey("dbo.DI_M_ProvisioningForm", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_AccessRequest", "AdHocAccessRequestId", "dbo.DI_M_AdHocAccessRequest");
            DropForeignKey("dbo.DI_M_AdHocAccessRequest", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_EndpointSystem", "OwnerUserId", "dbo.DI_M_User");
            DropForeignKey("dbo.UserRole", "User_UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.UserRole", "Role_RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_Role", "RoleOwner", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_User", "OrganisationId", "dbo.DI_M_Organisation");
            DropForeignKey("dbo.UserGroup", "User_UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.UserGroup", "Group_GroupId", "dbo.DI_M_Group");
            DropForeignKey("dbo.DI_M_Group", "GroupOwner", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_User", "DivisionId", "dbo.DI_M_Division");
            DropForeignKey("dbo.DI_M_User", "DepartmentId", "dbo.DI_M_Department");
            DropForeignKey("dbo.DI_M_User", "CompanyId", "dbo.DI_M_Company");
            DropForeignKey("dbo.DI_M_User", "BusinessRoleId", "dbo.DI_M_Designation");
            DropForeignKey("dbo.DI_M_AttributeMapping", "EndPointSystemType_EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_AttributeMapping", "EndPoint_EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_EndpointSystem", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_EndpointObjects", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DiscoveryProcessEndpointObjects", "EndpointObjects_EndpointObjectId", "dbo.DI_M_EndpointObjects");
            DropForeignKey("dbo.DiscoveryProcessEndpointObjects", "DiscoveryProcess_DiscoveryProcessId", "dbo.DI_M_DiscoveryProcess");
            DropForeignKey("dbo.DI_M_DiscoveryProcess", "Endpointsystemid", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.UnixUserInfoUnixGroupInfo", new[] { "UnixGroupInfo_Id" });
            DropIndex("dbo.UnixUserInfoUnixGroupInfo", new[] { "UnixUserInfo_UserId" });
            DropIndex("dbo.SAPRoleInfoSAPUserinfo", new[] { "SAPUserinfo_UserId" });
            DropIndex("dbo.SAPRoleInfoSAPUserinfo", new[] { "SAPRoleInfo_SAP_ROLEID" });
            DropIndex("dbo.SAPProfileInfoSAPUserinfo", new[] { "SAPUserinfo_UserId" });
            DropIndex("dbo.SAPProfileInfoSAPUserinfo", new[] { "SAPProfileInfo_SAP_PROFILEID" });
            DropIndex("dbo.SAPParameterInfoSAPUserinfo", new[] { "SAPUserinfo_UserId" });
            DropIndex("dbo.SAPParameterInfoSAPUserinfo", new[] { "SAPParameterInfo_SAP_ParameterID" });
            DropIndex("dbo.SAPUserinfoSAPGroupInfo", new[] { "SAPGroupInfo_SAP_GROUPID" });
            DropIndex("dbo.SAPUserinfoSAPGroupInfo", new[] { "SAPUserinfo_UserId" });
            DropIndex("dbo.SAPEPRoleInfoSAPEPUserInfo", new[] { "SAPEPUserInfo_UserId" });
            DropIndex("dbo.SAPEPRoleInfoSAPEPUserInfo", new[] { "SAPEPRoleInfo_Id" });
            DropIndex("dbo.SAPEPUserInfoSAPEPGroupInfo", new[] { "SAPEPGroupInfo_Id" });
            DropIndex("dbo.SAPEPUserInfoSAPEPGroupInfo", new[] { "SAPEPUserInfo_UserId" });
            DropIndex("dbo.SAPTCodeInfoSAPSingleRoleInfo", new[] { "SAPSingleRoleInfo_SingleRole_Id" });
            DropIndex("dbo.SAPTCodeInfoSAPSingleRoleInfo", new[] { "SAPTCodeInfo_TCODE_Id" });
            DropIndex("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", new[] { "SAPCompositeRoleInfo_CompositeRole_Id" });
            DropIndex("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", new[] { "SAPSingleRoleInfo_SingleRole_Id" });
            DropIndex("dbo.OracleEBSRoleInfoOracleEBSUserInfo", new[] { "OracleEBSUserInfo_UserId" });
            DropIndex("dbo.OracleEBSRoleInfoOracleEBSUserInfo", new[] { "OracleEBSRoleInfo_RoleId" });
            DropIndex("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", new[] { "OracleEBSResponsibilitiesInfo_ResponsibilityId" });
            DropIndex("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", new[] { "OracleEBSUserInfo_UserId" });
            DropIndex("dbo.LotusUserInfoLotusGroupInfo", new[] { "LotusGroupInfo_Id" });
            DropIndex("dbo.LotusUserInfoLotusGroupInfo", new[] { "LotusUserInfo_UserId" });
            DropIndex("dbo.SODMitigationSODRiskCode", new[] { "SODRiskCode_RiskId" });
            DropIndex("dbo.SODMitigationSODRiskCode", new[] { "SODMitigation_MitigationId" });
            DropIndex("dbo.SodFunctionSODConflictFunction", new[] { "SODConflictFunction_SodConflictFunctionId" });
            DropIndex("dbo.SodFunctionSODConflictFunction", new[] { "SodFunction_SodFunctionId" });
            DropIndex("dbo.SODConflictFunctionCompliancePolicy", new[] { "CompliancePolicy_CompliancePolicyId" });
            DropIndex("dbo.SODConflictFunctionCompliancePolicy", new[] { "SODConflictFunction_SodConflictFunctionId" });
            DropIndex("dbo.ADSUserInfoADSGroupInfo", new[] { "ADSGroupInfo_GroupId" });
            DropIndex("dbo.ADSUserInfoADSGroupInfo", new[] { "ADSUserInfo_UserId" });
            DropIndex("dbo.UserRole", new[] { "User_UserId" });
            DropIndex("dbo.UserRole", new[] { "Role_RoleId" });
            DropIndex("dbo.UserGroup", new[] { "User_UserId" });
            DropIndex("dbo.UserGroup", new[] { "Group_GroupId" });
            DropIndex("dbo.DiscoveryProcessEndpointObjects", new[] { "EndpointObjects_EndpointObjectId" });
            DropIndex("dbo.DiscoveryProcessEndpointObjects", new[] { "DiscoveryProcess_DiscoveryProcessId" });
            DropIndex("dbo.DI_M_UserQuickLink", new[] { "TabMenuId" });
            DropIndex("dbo.DI_M_UserQuickLink", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserGridSettings", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserGridInfo", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserGridFilterSettings", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserFilterInfo", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserDashboardPreference", new[] { "DashboardId" });
            DropIndex("dbo.DI_M_UserDashboardPreference", new[] { "UserId" });
            DropIndex("dbo.DI_Unix_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_Unix_Configuration", new[] { "EndpointSystem_EndpointSystemId" });
            DropIndex("dbo.DI_M_Task", new[] { "SubTaskId" });
            DropIndex("dbo.DI_M_Task", new[] { "AccessRequestId" });
            DropIndex("dbo.DI_M_TaskReviews", new[] { "ReviewId" });
            DropIndex("dbo.DI_M_TaskReviews", new[] { "TaskId" });
            DropIndex("dbo.DI_M_TaskDelegation", new[] { "DelegatedTo" });
            DropIndex("dbo.DI_M_SODPolicy", new[] { "PolicyTypeId" });
            DropIndex("dbo.DI_M_SODExecutionConfig", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_SODConflict", new[] { "SodRiskId_RiskId" });
            DropIndex("dbo.DI_M_SODConflict", new[] { "FunctionId2" });
            DropIndex("dbo.DI_M_SODConflict", new[] { "FunctionId1" });
            DropIndex("dbo.DI_M_SODRunPolicySelection", new[] { "SODRunPolicyId" });
            DropIndex("dbo.DI_M_SODRunPolicy", new[] { "WorkflowConfigurationId" });
            DropIndex("dbo.DI_M_SODRunPolicy", new[] { "RiskLevelId" });
            DropIndex("dbo.DI_M_SODRunPolicy", new[] { "CompliancePolicyId" });
            DropIndex("dbo.DI_M_SODReport", new[] { "ExceptionId" });
            DropIndex("dbo.DI_M_SODReport", new[] { "SODRunPolicyId" });
            DropIndex("dbo.DI_M_SODReport", new[] { "ConflictId" });
            DropIndex("dbo.DI_M_SODReport", new[] { "FunctionItem2" });
            DropIndex("dbo.DI_M_SODReport", new[] { "FunctionId2" });
            DropIndex("dbo.DI_M_SODReport", new[] { "FunctionItem1" });
            DropIndex("dbo.DI_M_SODReport", new[] { "FunctionId1" });
            DropIndex("dbo.DI_SAP_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SAPEP_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SAPEP_Configuration", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SAP_Configuration", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_M_RoleMatrixDetail", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_RoleMatrixDetail", new[] { "RoleMatrixId" });
            DropIndex("dbo.DI_M_RoleMatrix", new[] { "BusinessRoleId" });
            DropIndex("dbo.DI_M_RoleMatrix", new[] { "OrganisationId" });
            DropIndex("dbo.DI_M_RoleMatrix", new[] { "DivisionId" });
            DropIndex("dbo.DI_M_RoleMatrix", new[] { "DepartmentId" });
            DropIndex("dbo.DI_M_RoleMatrix", new[] { "CompanyId" });
            DropIndex("dbo.DI_M_QuickLink", new[] { "TabMenuId" });
            DropIndex("dbo.DI_M_ProvisioningFormSAP", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_ProvisioningFormSAPEP", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_ProvisioningFormOracleEBS", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_ProvisioningFormLNS", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_ProvisioningFormLINUX", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_ProvisioningFormAD", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_Process", new[] { "Scheduler_SchedulerId" });
            DropIndex("dbo.DI_M_PasswordSyncConfigurationDetail", new[] { "ConfigId" });
            DropIndex("dbo.DI_M_PasswordPolicy", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_PasswordPolicy", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_OEBS_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_OEBS_Configuration", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_LNS_Users", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_LNS_Configuration", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_INH_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_INH_Configuration", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_INH_Permission", new[] { "EndPointSystemID" });
            DropIndex("dbo.DI_M_GlobalAnswer", new[] { "UserId" });
            DropIndex("dbo.DI_M_EmployeeEventTrigger", new[] { "UserId" });
            DropIndex("dbo.DI_M_DiscoveryJobConfiguration", new[] { "EndpointObjectId" });
            DropIndex("dbo.DI_M_DiscoveryJobConfiguration", new[] { "DiscoveryProcessId" });
            DropIndex("dbo.DI_M_TabMenu", new[] { "PageId" });
            DropIndex("dbo.DI_M_DefaultRoleMenu", new[] { "TabMenuId" });
            DropIndex("dbo.DI_M_DefaultRoleMenu", new[] { "RoleId" });
            DropIndex("dbo.DI_M_DefaultRoleDashboard", new[] { "DashBoardId" });
            DropIndex("dbo.DI_M_DefaultRoleDashboard", new[] { "RoleId" });
            DropIndex("dbo.DI_M_DashboardAccessRights", new[] { "DashboardId" });
            DropIndex("dbo.DI_M_DashboardAccessRights", new[] { "RoleId" });
            DropIndex("dbo.DI_M_SODRiskCode", new[] { "RiskScoreId" });
            DropIndex("dbo.DI_M_SODRiskCode", new[] { "UserId" });
            DropIndex("dbo.DI_M_SODRiskCode", new[] { "RiskLevelId" });
            DropIndex("dbo.DI_M_SODFunctionItem", new[] { "Endpointsystemid" });
            DropIndex("dbo.DI_M_SODFunctionItem", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_SODFunctionItem", new[] { "SodFunctionId" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "FunctionItemId2" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "FunctionId2" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "FunctionItemId1" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "FunctionId1" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "UserId" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "ExceptionId" });
            DropIndex("dbo.DI_M_SODException", new[] { "SODRunPolicy_SODRunPolicyId" });
            DropIndex("dbo.DI_M_SODException", new[] { "SodFunction_SodFunctionId" });
            DropIndex("dbo.DI_M_SODException", new[] { "RiskOwnerId" });
            DropIndex("dbo.DI_M_SODConflictFunctions", new[] { "RiskCodeId" });
            DropIndex("dbo.DI_M_CompliancePolicy", new[] { "OwnerId" });
            DropIndex("dbo.DI_M_ReviewEndpointSystems", new[] { "ReviewEndpointId" });
            DropIndex("dbo.DI_M_ReviewEndpointSystems", new[] { "ReviewId" });
            DropIndex("dbo.DI_M_AttestationReview", new[] { "UserId" });
            DropIndex("dbo.DI_M_AttestationReview_EndpointSystem_L3Detail", new[] { "L2DetailId" });
            DropIndex("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", new[] { "ReviewEndpointId" });
            DropIndex("dbo.DI_M_AttestationProcessHistory", new[] { "AttestationId" });
            DropIndex("dbo.DI_M_WorkflowConfigurationDetail", new[] { "WorkflowConfigurationId" });
            DropIndex("dbo.DI_M_WorkflowConfiguration", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_WorkflowConfiguration", new[] { "RequestTypeId" });
            DropIndex("dbo.DI_M_AttestationProcess", new[] { "AttestationId" });
            DropIndex("dbo.DI_M_Attestation", new[] { "WorkflowConfigurationId" });
            DropIndex("dbo.DI_M_Attestation_Endpoint", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_Attestation_Endpoint", new[] { "AttestationId" });
            DropIndex("dbo.DI_ADS_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_ADS_Configuration", new[] { "EndpointSystem_EndpointSystemId" });
            DropIndex("dbo.DI_M_Page", new[] { "PageUrlId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "UrlId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "PermissionId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "PageId" });
            DropIndex("dbo.DI_M_AccessRights", new[] { "PagePermissionId" });
            DropIndex("dbo.DI_M_AccessRights", new[] { "RoleId" });
            DropIndex("dbo.DI_M_ProvisioningFormGeneralData", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_ProvisioningForm", new[] { "ProvisioningPassword_Id" });
            DropIndex("dbo.DI_M_ProvisioningForm", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_Role", new[] { "RoleOwner" });
            DropIndex("dbo.DI_M_Group", new[] { "GroupOwner" });
            DropIndex("dbo.DI_M_User", new[] { "BusinessRoleId" });
            DropIndex("dbo.DI_M_User", new[] { "OrganisationId" });
            DropIndex("dbo.DI_M_User", new[] { "DivisionId" });
            DropIndex("dbo.DI_M_User", new[] { "DepartmentId" });
            DropIndex("dbo.DI_M_User", new[] { "CompanyId" });
            DropIndex("dbo.DI_M_AttributeMapping", new[] { "EndPointSystemType_EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_AttributeMapping", new[] { "EndPoint_EndpointSystemId" });
            DropIndex("dbo.DI_M_DiscoveryProcess", new[] { "Endpointsystemid" });
            DropIndex("dbo.DI_M_EndpointObjects", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_EndpointSystem", new[] { "OwnerUserId" });
            DropIndex("dbo.DI_M_EndpointSystem", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_AdHocAccessRequest", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_AccessRequest", new[] { "AdHocAccessRequestId" });
            DropIndex("dbo.DI_M_AccessRequest", new[] { "ProvisioningFormId" });
            DropIndex("dbo.DI_M_AccessRequest", new[] { "UserId" });
            DropTable("dbo.UnixUserInfoUnixGroupInfo");
            DropTable("dbo.SAPRoleInfoSAPUserinfo");
            DropTable("dbo.SAPProfileInfoSAPUserinfo");
            DropTable("dbo.SAPParameterInfoSAPUserinfo");
            DropTable("dbo.SAPUserinfoSAPGroupInfo");
            DropTable("dbo.SAPEPRoleInfoSAPEPUserInfo");
            DropTable("dbo.SAPEPUserInfoSAPEPGroupInfo");
            DropTable("dbo.SAPTCodeInfoSAPSingleRoleInfo");
            DropTable("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo");
            DropTable("dbo.OracleEBSRoleInfoOracleEBSUserInfo");
            DropTable("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo");
            DropTable("dbo.LotusUserInfoLotusGroupInfo");
            DropTable("dbo.SODMitigationSODRiskCode");
            DropTable("dbo.SodFunctionSODConflictFunction");
            DropTable("dbo.SODConflictFunctionCompliancePolicy");
            DropTable("dbo.ADSUserInfoADSGroupInfo");
            DropTable("dbo.UserRole");
            DropTable("dbo.UserGroup");
            DropTable("dbo.DiscoveryProcessEndpointObjects");
            DropTable("dbo.DI_VPN_Master");
            DropTable("dbo.DI_M_UserQuickLink");
            DropTable("dbo.DI_M_UserGridSettings");
            DropTable("dbo.DI_M_UserGridInfo");
            DropTable("dbo.DI_M_UserGridFilterSettings");
            DropTable("dbo.DI_M_UserFilterInfo");
            DropTable("dbo.DI_M_UserDashboardPreference");
            DropTable("dbo.DI_Unix_UserInfoHistory");
            DropTable("dbo.DI_Unix_ResourceInfo");
            DropTable("dbo.DI_Unix_ResourceInfoHistory");
            DropTable("dbo.DI_Unix_UserInfo");
            DropTable("dbo.DI_Unix_GroupInfo");
            DropTable("dbo.DI_Unix_GroupInfoHistory");
            DropTable("dbo.DI_Unix_Configuration");
            DropTable("dbo.DI_Log_UI");
            DropTable("dbo.DI_M_Config_General_TrustedSource");
            DropTable("dbo.DI_M_Tenant");
            DropTable("dbo.DI_M_Task");
            DropTable("dbo.DI_M_TaskReviews");
            DropTable("dbo.DI_M_TaskDelegation");
            DropTable("dbo.DI_M_SubTask");
            DropTable("dbo.DI_M_SODPolicy");
            DropTable("dbo.DI_M_SODExecutionConfig");
            DropTable("dbo.DI_M_SODConflict");
            DropTable("dbo.DI_M_SODRunPolicySelection");
            DropTable("dbo.DI_M_SODRunPolicy");
            DropTable("dbo.DI_M_SODReport");
            DropTable("dbo.DI_LOG_Scheduler");
            DropTable("dbo.DI_SAP_XLProvMatrix");
            DropTable("dbo.DI_SAP_UserProfile");
            DropTable("dbo.DI_SAP_UserHistoryInfo");
            DropTable("dbo.DI_SAP_TCodeObject");
            DropTable("dbo.DI_SAP_RoleToAuthMap");
            DropTable("dbo.DI_SAP_ROLETCODEMAPPING");
            DropTable("dbo.DI_SAP_RoleCatalog");
            DropTable("dbo.DI_SAP_XLPositionsMatrix");
            DropTable("dbo.DI_SAP_Position");
            DropTable("dbo.DI_SAP_PARAMETERHISTORYINFO");
            DropTable("dbo.DI_SAP_ObjectValues");
            DropTable("dbo.DI_SAP_Objects");
            DropTable("dbo.DI_SAP_SAPHRP1001");
            DropTable("dbo.DI_SAP_ROLEINFO");
            DropTable("dbo.DI_SAP_PROFILEINFO");
            DropTable("dbo.DI_SAP_PARAMETERINFO");
            DropTable("dbo.DI_SAP_UserInfo");
            DropTable("dbo.DI_SAP_GROUPINFO");
            DropTable("dbo.DI_SAP_GROUPHISTORYINFO");
            DropTable("dbo.DI_SAP_FIELDVALUEMAPPING");
            DropTable("dbo.DI_SAPEP_UserInfoHistory");
            DropTable("dbo.DI_SAPEP_RoleInfoTemp");
            DropTable("dbo.DI_SAPEP_RoleInfoHistory");
            DropTable("dbo.DI_SAPEP_RoleInfo");
            DropTable("dbo.DI_SAPEP_UserInfo");
            DropTable("dbo.DI_SAPEP_GroupInfo");
            DropTable("dbo.DI_SAPEP_GroupInfoHistory");
            DropTable("dbo.DI_SAPEP_Configuration");
            DropTable("dbo.DI_SAP_XLCriticalTCodeMatrix");
            DropTable("dbo.DI_SAP_Configuration");
            DropTable("dbo.DI_SAP_TCODEINFO");
            DropTable("dbo.DI_SAP_SINGLEROLEINFO");
            DropTable("dbo.DI_SAP_COMPOSITEROLEINFO");
            DropTable("dbo.DI_SAP_COMPOSITEPROFILEINFO");
            DropTable("dbo.DI_SAP_AuthObjectMap");
            DropTable("dbo.DI_SAP_AuthorizationProfile");
            DropTable("dbo.DI_SAP_ActivityInfo");
            DropTable("dbo.Ruleset");
            DropTable("dbo.DI_M_RoleTypes");
            DropTable("dbo.DI_M_RoleMatrixDetail");
            DropTable("dbo.DI_M_RoleMatrix");
            DropTable("dbo.DI_M_RoleHistory");
            DropTable("dbo.DI_M_RequestTypes");
            DropTable("dbo.DI_M_QuickLink");
            DropTable("dbo.DI_Log_Provisioning");
            DropTable("dbo.DI_M_ProvisioningFormSAP");
            DropTable("dbo.DI_M_ProvisioningFormSAPEP");
            DropTable("dbo.DI_M_ProvisioningFormOracleEBS");
            DropTable("dbo.DI_M_ProvisioningFormLNS");
            DropTable("dbo.DI_M_ProvisioningFormLINUX");
            DropTable("dbo.DI_M_ProvisioningFormAD");
            DropTable("dbo.DI_M_ProcessHistroy");
            DropTable("dbo.DI_M_Scheduler");
            DropTable("dbo.DI_M_Process");
            DropTable("dbo.DI_M_PreferenceItem");
            DropTable("dbo.DI_M_PasswordSyncConfiguration");
            DropTable("dbo.DI_M_PasswordSyncConfigurationDetail");
            DropTable("dbo.DI_M_PasswordPolicy");
            DropTable("dbo.DI_M_OUChangeSnapshot");
            DropTable("dbo.DI_OEBS_XLProvMatrix");
            DropTable("dbo.DI_OEBS_UserInfoHistory");
            DropTable("dbo.DI_OEBS_SubmenusInfo");
            DropTable("dbo.DI_OEBS_RoleInfoHistory");
            DropTable("dbo.DI_OEBS_ResponsibilitiesInfoHistory");
            DropTable("dbo.DI_OEBS_RoleInfo");
            DropTable("dbo.DI_OEBS_UserInfo");
            DropTable("dbo.DI_OEBS_ResponsibilitiesInfo");
            DropTable("dbo.DI_OEBS_MenusInfo");
            DropTable("dbo.DI_OEBS_HRMSUserInfoHistory");
            DropTable("dbo.DI_OEBS_HRMSUserInfo");
            DropTable("dbo.DI_OEBS_Configuration");
            DropTable("dbo.DI_M_MailTemplate");
            DropTable("dbo.DI_M_MailServerConfiguration");
            DropTable("dbo.DI_LNS_UsersHistory");
            DropTable("dbo.DI_LNS_Users");
            DropTable("dbo.DI_LNS_Groups");
            DropTable("dbo.DI_LNS_GroupsHistory");
            DropTable("dbo.DI_LNS_Certifiers");
            DropTable("dbo.DI_LNS_XLProvMatrix");
            DropTable("dbo.DI_LNS_MailQuotaConfig");
            DropTable("dbo.DI_LNS_Configuration");
            DropTable("dbo.DI_M_InterimStore");
            DropTable("dbo.DI_INH_UserInfo");
            DropTable("dbo.DI_INH_Configuration");
            DropTable("dbo.DI_INH_Permission");
            DropTable("dbo.DI_M_GroupHistory");
            DropTable("dbo.DI_M_GlobalQuestion");
            DropTable("dbo.DI_M_GlobalAnswer");
            DropTable("dbo.DI_M_EntitlementReconcilation");
            DropTable("dbo.DI_M_EmployeeTerm");
            DropTable("dbo.DI_M_EmployeeEventTrigger");
            DropTable("dbo.DI_Log_Email");
            DropTable("dbo.DI_M_Email");
            DropTable("dbo.DI_M_EmailHistory");
            DropTable("dbo.DI_M_DiscoveryProcessHistory");
            DropTable("dbo.DI_Log_Discovery");
            DropTable("dbo.DI_M_DiscoveryJobConfiguration");
            DropTable("dbo.DI_M_TabMenu");
            DropTable("dbo.DI_M_DefaultRoleMenu");
            DropTable("dbo.DI_M_DefaultRoleDashboard");
            DropTable("dbo.DI_M_DefaultProvisioningFormGeneralData");
            DropTable("dbo.DI_M_Dashboard");
            DropTable("dbo.DI_M_DashboardAccessRights");
            DropTable("dbo.DI_M_CostCenter");
            DropTable("dbo.DI_M_ConfigurationMaster");
            DropTable("dbo.DI_M_ConfigurationMasterHistory");
            DropTable("dbo.DI_M_ComplianceType");
            DropTable("dbo.DI_M_SODRiskScore");
            DropTable("dbo.DI_M_SODRiskLevel");
            DropTable("dbo.DI_M_SODMitigation");
            DropTable("dbo.DI_M_SODRiskCode");
            DropTable("dbo.DI_M_SODFunctionItem");
            DropTable("dbo.DI_M_SODExceptionItem");
            DropTable("dbo.DI_M_SODException");
            DropTable("dbo.DI_M_SODFunction");
            DropTable("dbo.DI_M_SODConflictFunctions");
            DropTable("dbo.DI_M_CompliancePolicy");
            DropTable("dbo.DI_M_ReviewEndpointSystems");
            DropTable("dbo.DI_M_AttestationReview");
            DropTable("dbo.DI_M_AttestationReview_EndpointSystem");
            DropTable("dbo.DI_M_AttestationReview_EndpointSystem_L3DetailHistory");
            DropTable("dbo.DI_M_AttestationReview_EndpointSystem_L3Detail");
            DropTable("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail");
            DropTable("dbo.DI_M_AttestationReview_EndpointSystem_L2DetailHistory");
            DropTable("dbo.DI_M_AttestationReview_EndpointSystemHistory");
            DropTable("dbo.DI_M_AttestationProcessHistory");
            DropTable("dbo.DI_M_WorkflowConfigurationDetail");
            DropTable("dbo.DI_M_WorkflowConfiguration");
            DropTable("dbo.DI_M_AttestationProcess");
            DropTable("dbo.DI_M_Attestation");
            DropTable("dbo.DI_M_Attestation_Endpoint");
            DropTable("dbo.DI_Log_Attestation");
            DropTable("dbo.DI_M_XLApproverExceptions");
            DropTable("dbo.DI_M_ApprovalUserTypes");
            DropTable("dbo.DI_ADS_XLProvMatrix");
            DropTable("dbo.DI_ADS_UserInfoHistory");
            DropTable("dbo.DI_ADS_SharedFolderInfo");
            DropTable("dbo.DI_ADS_PrinterInfo");
            DropTable("dbo.DI_ADS_OUInfo");
            DropTable("dbo.DI_ADS_OuInfoHistory");
            DropTable("dbo.DI_ADS_UserInfo");
            DropTable("dbo.DI_ADS_GroupInfo");
            DropTable("dbo.DI_ADS_GroupInfoHistory");
            DropTable("dbo.DI_ADS_Configuration");
            DropTable("dbo.DI_ADS_ComputerInfo");
            DropTable("dbo.DI_ADS_ComputerGroup");
            DropTable("dbo.DI_M_AccountReconcilation");
            DropTable("dbo.DI_M_AccountCategory");
            DropTable("dbo.DI_M_AccountActionCategory");
            DropTable("dbo.DI_M_Permission");
            DropTable("dbo.DI_M_PageUrl");
            DropTable("dbo.DI_M_Page");
            DropTable("dbo.DI_M_PagePermission");
            DropTable("dbo.DI_M_AccessRights");
            DropTable("dbo.DI_M_AccessRightHistory");
            DropTable("dbo.DI_M_ProvisioningPassword");
            DropTable("dbo.DI_M_ProvisioningFormGeneralData");
            DropTable("dbo.DI_M_ProvisioningForm");
            DropTable("dbo.DI_M_Role");
            DropTable("dbo.DI_M_Organisation");
            DropTable("dbo.DI_M_Group");
            DropTable("dbo.DI_M_Division");
            DropTable("dbo.DI_M_Department");
            DropTable("dbo.DI_M_Company");
            DropTable("dbo.DI_M_Designation");
            DropTable("dbo.DI_M_User");
            DropTable("dbo.DI_M_AttributeMapping");
            DropTable("dbo.DI_M_DiscoveryProcess");
            DropTable("dbo.DI_M_EndpointObjects");
            DropTable("dbo.DI_M_EndpointSystemType");
            DropTable("dbo.DI_M_EndpointSystem");
            DropTable("dbo.DI_M_AdHocAccessRequest");
            DropTable("dbo.DI_M_AccessRequest");
            DropTable("dbo.DI_M_AccessRequestHistory");
            DropTable("dbo.DI_M_AccessRequestCategory");
        }
    }
}
