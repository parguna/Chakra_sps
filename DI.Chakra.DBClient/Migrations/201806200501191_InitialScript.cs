namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialScript : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_AccessRequest",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequesterId = c.Long(),
                        UserId = c.Long(nullable: false),
                        Description = c.String(),
                        ProvisioningFormId = c.Long(),
                        AdHocAccessRequestId = c.Long(),
                        EndpointSystemId = c.Long(),
                        EmployeeEventTriggerId = c.Long(),
                        RequestStatus = c.Long(nullable: false),
                        ProgressStatus = c.Long(nullable: false),
                        RequestSource = c.Long(nullable: false),
                        ActivationDate = c.DateTime(),
                        DeactivationDate = c.DateTime(),
                        ExecutionTime = c.DateTime(),
                        Remarks = c.String(),
                        ApproverType = c.String(),
                        AttestationCampaignId = c.Long(),
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
                        AccountType = c.Byte(nullable: false),
                        RESPONSETXID = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AdHocAccessRequest", t => t.AdHocAccessRequestId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .ForeignKey("dbo.DI_M_User", t => t.RequesterId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.RequesterId)
                .Index(t => t.UserId)
                .Index(t => t.AdHocAccessRequestId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_AdHocAccessRequest",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequesterUserId = c.Long(nullable: false),
                        RequesteeUserId = c.Long(nullable: false),
                        EndpointSystemType = c.Int(nullable: false),
                        EndpointSystemId = c.Long(),
                        AdhocActionType = c.Byte(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        IsSelfRequest = c.Boolean(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        Comments = c.String(),
                        EntitlementDetail = c.String(),
                        ApplicationType = c.Byte(nullable: false),
                        SecurityQuestions = c.String(),
                        ActualSupervisor = c.Int(),
                        SelectedSupervisor = c.Int(),
                        SupervisorChanged = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsÉxpiryMailSend = c.Boolean(nullable: false),
                        IsÉxpiryHandled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_EndpointSystem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        EndpointSystemTypeId = c.Long(),
                        DisplayName = c.String(),
                        OwnerUserId = c.Long(),
                        ApplicationCode = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        IsTrustedSource = c.Boolean(nullable: false),
                        IsPasswordResetAllowed = c.Boolean(nullable: false),
                        IncludeInAttestation = c.Boolean(nullable: false),
                        UserObjectName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        ApplicationType = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId)
                .ForeignKey("dbo.DI_M_User", t => t.OwnerUserId)
                .Index(t => t.EndpointSystemTypeId)
                .Index(t => t.OwnerUserId);
            
            CreateTable(
                "dbo.DI_M_EndpointSystemType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemTypeName = c.Int(nullable: false),
                        DisplayName = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        ObjectName = c.String(),
                        CanBeTrustedSource = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_EndpointObjects",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Int(nullable: false),
                        ObjectName = c.String(),
                        IsTrustedSource = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndPointSystemType_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndPointSystemType_Id)
                .Index(t => t.EndPointSystemType_Id);
            
            CreateTable(
                "dbo.DI_M_DiscoveryProcess",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProcessName = c.String(),
                        Description = c.String(),
                        Endpointsystemid = c.Long(nullable: false),
                        Default = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        DisableDeltaDiscovery = c.Boolean(nullable: false),
                        process_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.Endpointsystemid)
                .ForeignKey("dbo.DI_M_Process", t => t.process_Id)
                .Index(t => t.Endpointsystemid)
                .Index(t => t.process_Id);
            
            CreateTable(
                "dbo.DI_M_Process",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProcessType = c.Int(nullable: false),
                        ProcessTypeKeyId = c.Long(nullable: false),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        LastRunOn = c.DateTime(),
                        Status = c.String(),
                        ErrorMessage = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Scheduler_SchedulerId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Scheduler", t => t.Scheduler_SchedulerId)
                .Index(t => t.Scheduler_SchedulerId);
            
            CreateTable(
                "dbo.DI_M_Scheduler",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProcessName = c.String(),
                        StartDate = c.DateTime(nullable: false, storeType: "date"),
                        StartTime = c.Time(nullable: false, precision: 7),
                        Occurence = c.Int(),
                        ReccurEveryDays = c.Int(),
                        ReccurEveryWeeks = c.Int(),
                        WeekDay = c.String(),
                        ReccurEveryMonths = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AttributeMapping",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DiColumn = c.String(),
                        Tgcolumn = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndpointSystemId = c.Long(),
                        EndpointSystemTypeId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId)
                .Index(t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemTypeId);
            
            CreateTable(
                "dbo.DI_M_User",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        IdentificationNo = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        DisplayName = c.String(),
                        KnownAs = c.String(),
                        EmployeeId = c.String(),
                        FullTimeEmployee = c.Boolean(nullable: false),
                        EEGroup = c.String(),
                        EmployeeStatus = c.String(),
                        IsExecutiveEmployee = c.Boolean(nullable: false),
                        CompanyId = c.Long(),
                        DepartmentId = c.Long(),
                        DivisionId = c.Long(),
                        NOrganisationId = c.Long(),
                        NBusinessRoleId = c.Long(),
                        ManagerId = c.Long(),
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
                        CreatedDate = c.DateTime(nullable: false),
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
                        UnitCode = c.String(),
                        UnitName = c.String(),
                        PositionCode = c.String(),
                        PositionName = c.String(),
                        Supervisor = c.String(),
                        AuthenticationMode = c.String(),
                        OrgCompany = c.String(),
                        OrgDepartment = c.String(),
                        OrgDivision = c.String(),
                        OrgBranch = c.String(),
                        OrgSection = c.String(),
                        OrgUnit = c.String(),
                        OneKeyTokenId = c.String(),
                        FLINSLoginId = c.String(),
                        ROEmployeeId = c.String(),
                        COEmployeeId = c.String(),
                        ProjectDescription = c.String(),
                        ReferenceUserId = c.Long(nullable: false),
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
                        RequestStatus = c.Long(nullable: false),
                        LastLogonDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_NDesignation", t => t.NBusinessRoleId)
                .ForeignKey("dbo.DI_M_NOrganisation", t => t.NOrganisationId)
                .Index(t => t.NOrganisationId)
                .Index(t => t.NBusinessRoleId);
            
            CreateTable(
                "dbo.DI_M_Group",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupName = c.String(),
                        GroupDescription = c.String(),
                        EmailId = c.String(),
                        GroupOwner = c.Long(),
                        IsDefaultGroup = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.GroupOwner)
                .Index(t => t.GroupOwner);
            
            CreateTable(
                "dbo.DI_M_NDesignation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_NOrganisation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ParentOrgCode = c.String(),
                        ParentOrgId = c.Long(),
                        ScopeId = c.Long(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_NOrganisationScope", t => t.ScopeId)
                .Index(t => t.ScopeId);
            
            CreateTable(
                "dbo.DI_M_NOrganisationScope",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Scope = c.String(),
                        Description = c.String(),
                        Level = c.Int(nullable: false),
                        IsValidForApproval = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Role",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleName = c.String(),
                        RoleDescription = c.String(),
                        RoleOwner = c.Long(),
                        IsDefaultRole = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.RoleOwner)
                .Index(t => t.RoleOwner);
            
            CreateTable(
                "dbo.DI_M_AccessRequestCategory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccessRequestTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccessRequestConfig",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        EnableActionType = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_AccessRequestProgress",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccessRequestSource",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccessRequestStatus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccessRightHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccessRightsId = c.Int(nullable: false),
                        RoleId = c.Long(),
                        TabMenuId = c.Int(),
                        UserAction = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccessRights",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleId = c.Long(nullable: false),
                        PagePermissionId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_PagePermission", t => t.PagePermissionId)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.PagePermissionId);
            
            CreateTable(
                "dbo.DI_M_PagePermission",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PageId = c.Long(nullable: false),
                        PermissionId = c.Long(nullable: false),
                        UrlId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_PageUrl", t => t.UrlId)
                .ForeignKey("dbo.DI_M_Permission", t => t.PermissionId)
                .ForeignKey("dbo.DI_M_Page", t => t.PageId)
                .Index(t => t.PageId)
                .Index(t => t.PermissionId)
                .Index(t => t.UrlId);
            
            CreateTable(
                "dbo.DI_M_PageUrl",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Url = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Permission",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        IconURL = c.String(),
                        ClientScript = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Page",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UniqueName = c.String(),
                        Name = c.String(),
                        PageUrlId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_PageUrl", t => t.PageUrlId)
                .Index(t => t.PageUrlId);
            
            CreateTable(
                "dbo.DI_M_AccountActionCategory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccountActionTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccountCategory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccountTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AccountReconcilation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        EndpointUserId = c.Long(nullable: false),
                        UserId = c.Long(),
                        AccountName = c.String(),
                        FullName = c.String(),
                        AccountCreatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        IsResolved = c.Boolean(nullable: false),
                        ResolvedBy = c.String(),
                        Comments = c.String(),
                        ExceptionType = c.Byte(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        MappingType = c.Byte(nullable: false),
                        OffboardingAccountAction = c.Long(nullable: false),
                        ResolvedStatus = c.Byte(nullable: false),
                        ProvisionedThrough = c.String(),
                        IsSODConflict = c.Boolean(nullable: false),
                        IsSensitiveAccess = c.Boolean(nullable: false),
                        LastAttestedOn = c.DateTime(),
                        LastLogonDate = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        ValidFromDate = c.DateTime(),
                        ValidToDate = c.DateTime(),
                        AccessRequestId = c.Long(),
                        ExceptionOccuredDate = c.DateTime(),
                        ResolvedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_AdminModeItem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        TabMenuId = c.Long(nullable: false),
                        Icon = c.String(),
                        Information = c.String(),
                        Type = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        TableName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_TabMenu", t => t.TabMenuId)
                .Index(t => t.TabMenuId);
            
            CreateTable(
                "dbo.DI_M_TabMenu",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UniqueName = c.String(),
                        MenuName = c.String(),
                        ParentTabMenuId = c.Long(),
                        PageId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Page", t => t.PageId)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.DI_ADS_ComputerGroup",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        ComputerCN = c.String(),
                        ComputerDistinguishedName = c.String(),
                        GroupCN = c.String(),
                        GroupDistinguishedName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_ADS_ComputerInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_ADS_GroupInfoHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        EndPointSystemId = c.Long(nullable: false),
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
                        LastModifiedDate = c.DateTime(),
                        RecordStatus = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_ADS_GroupInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                        CanRequest = c.Boolean(),
                        UserDefinedDesc = c.String(),
                        UserDefinedDisplayName = c.String(),
                        Level1Approver = c.Long(),
                        Level2Approver = c.Long(),
                        IsLevel1Group = c.Boolean(),
                        IsLevel2Group = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_ADS_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                        IsAccountLocked = c.Boolean(nullable: false),
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
                        ExUserId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IACMLastPasswordChange = c.DateTime(),
                        businessCategory = c.String(),
                        division = c.String(),
                        homeDrive = c.String(),
                        homeDirectory = c.String(),
                        IsHomeFolderPermissionSetFailed = c.Boolean(nullable: false),
                        CardSerialNumber = c.String(),
                        EmployeeId = c.String(),
                        Last4DigitNRIC = c.String(),
                        DateOfBirth = c.String(),
                        CostCenterCode = c.String(),
                        CostCenterDesc = c.String(),
                        PrivilegeAccount = c.String(),
                        WogUserId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_ADS_NestedGroup",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Supergroup = c.String(),
                        Subgroup = c.String(),
                        Depth = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_ADS_OuInfoHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OuId = c.Long(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        EndPointSystemId = c.Long(nullable: false),
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
                        LastModifiedDate = c.DateTime(),
                        RecordStatus = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_ADS_OUInfo", t => t.OuId)
                .Index(t => t.OuId);
            
            CreateTable(
                "dbo.DI_ADS_OUInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_ADS_PrinterInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_ADS_SharedFolderInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_ADS_UserInfoHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        EndPointSystemId = c.Long(nullable: false),
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
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_ADS_UserInfo", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_ApprovalUserTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ApprovalUserTypeDesc = c.String(),
                        ApplicableWorkFlowTypes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ApproverDelegationHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(nullable: false),
                        ApproverName = c.String(),
                        Level = c.String(),
                        Remarks = c.String(),
                        DelegatedDateTime = c.DateTime(nullable: false),
                        ApproverRole = c.String(),
                        IsActionOnlyRequired = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        TaskId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_XLApproverExceptions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Person = c.String(),
                        Approver = c.String(),
                        ApprovalLevel = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Attachment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequestTypeId = c.Long(nullable: false),
                        TaskId = c.Long(),
                        AccessRequestId = c.Long(nullable: false),
                        ContentType = c.String(),
                        FileName = c.String(),
                        AttachmentFile = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AttestationCampaignAttachment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationCampaignId = c.Long(),
                        ContentType = c.String(),
                        FileName = c.String(),
                        AttachmentFile = c.Binary(),
                        FileSavePath = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AttestationCampaign", t => t.AttestationCampaignId)
                .Index(t => t.AttestationCampaignId);
            
            CreateTable(
                "dbo.DI_M_AttestationCampaign",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationID = c.Long(nullable: false),
                        ParentCampaign = c.Long(),
                        WorkflowInstanceID = c.Guid(nullable: false),
                        CampaignName = c.String(),
                        Description = c.String(),
                        SplitUsing = c.String(),
                        DefaultReviewStatus = c.Byte(nullable: false),
                        Comments = c.String(),
                        Status = c.Byte(nullable: false),
                        ChildSplitCount = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CloseDate = c.DateTime(),
                        SnapshotFileName = c.String(),
                        RequiresAttestationRemediation = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationID)
                .Index(t => t.AttestationID);
            
            CreateTable(
                "dbo.DI_M_Attestation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationName = c.String(),
                        AttestationDescription = c.String(),
                        CampaignNameFormat = c.String(),
                        RequiresAttestationRemediation = c.Boolean(nullable: false),
                        WorkflowConfigurationId = c.Long(nullable: false),
                        IncludeUnmappedAccount = c.Boolean(nullable: false),
                        IncludeDormantAccount = c.Boolean(nullable: false),
                        DormantAccountDays = c.Long(nullable: false),
                        CreateTaskOnNoReviewer = c.Boolean(nullable: false),
                        TaskActionUserId = c.Long(),
                        IsGroupTask = c.Boolean(nullable: false),
                        SendEmailOnNoReviewer = c.Boolean(nullable: false),
                        EmailEntityId = c.Long(),
                        IsGroupMail = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        DefaultReviewStatus = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId)
                .Index(t => t.WorkflowConfigurationId);
            
            CreateTable(
                "dbo.DI_M_Attestation_Endpoint",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationId = c.Long(nullable: false),
                        EndpointSystemType = c.Int(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.AttestationId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_AttestationHierarchy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationId = c.Long(),
                        EndpointHierarchyId = c.Long(),
                        EndpointId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        IsActionAllowed = c.Boolean(nullable: false),
                        DefaultAttestationAction = c.Byte(nullable: false),
                        IsActionExtendable = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointHierarchy", t => t.EndpointHierarchyId)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId)
                .Index(t => t.AttestationId)
                .Index(t => t.EndpointHierarchyId);
            
            CreateTable(
                "dbo.DI_M_EndpointHierarchy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointType = c.Int(nullable: false),
                        EndpointId = c.Long(),
                        EntitlementType = c.Int(nullable: false),
                        EndpointObjectId = c.Long(nullable: false),
                        IsActionable = c.Boolean(nullable: false),
                        IsAssociatedWithUser = c.Boolean(nullable: false),
                        ParentId = c.Long(),
                        Level = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AttestationProcess",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProcessName = c.String(),
                        Description = c.String(),
                        AttestationId = c.Long(),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        WorkflowStatus = c.String(),
                        HistoryId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        process_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId)
                .ForeignKey("dbo.DI_M_Process", t => t.process_Id)
                .Index(t => t.AttestationId)
                .Index(t => t.process_Id);
            
            CreateTable(
                "dbo.DI_M_AttestationScope",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationId = c.Long(nullable: false),
                        OrganisationScopeId = c.Long(),
                        AttestationScopeName = c.String(),
                        AttestationScopeValue = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId)
                .ForeignKey("dbo.DI_M_NOrganisation", t => t.OrganisationScopeId)
                .Index(t => t.AttestationId)
                .Index(t => t.OrganisationScopeId);
            
            CreateTable(
                "dbo.DI_M_WorkflowConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WorkflowName = c.String(),
                        Description = c.String(),
                        RequestTypeId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(),
                        ApplicableUserTypes = c.String(),
                        ApplicableAccountTypes = c.String(),
                        ApplicableActionTypes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        ValidFrom = c.DateTime(nullable: false),
                        ValidTo = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .ForeignKey("dbo.DI_M_RequestTypes", t => t.RequestTypeId)
                .Index(t => t.RequestTypeId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_RequestTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequestTypeDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_WorkflowConfigurationDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        WorkflowConfigurationId = c.Long(nullable: false),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId)
                .Index(t => t.WorkflowConfigurationId);
            
            CreateTable(
                "dbo.DI_M_AttestationCampaignReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationCampaignId = c.Long(),
                        AttestationCampaignName = c.String(),
                        ReportType = c.Byte(nullable: false),
                        ContentType = c.String(),
                        FileName = c.String(),
                        AttachmentFile = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Attestation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationId = c.Long(),
                        AttestationName = c.String(),
                        LogLevel = c.String(),
                        LogType = c.String(),
                        EndpointSystemId = c.Long(),
                        Reviewer = c.String(),
                        ReviewerType = c.String(),
                        Reviewee = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AttestationProcessHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationId = c.Long(),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        Status = c.String(),
                        Message = c.String(),
                        WorkflowStatus = c.String(),
                        HistoryId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId)
                .Index(t => t.AttestationId);
            
            CreateTable(
                "dbo.DI_M_AttestationSnapshotAction_History",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Version = c.Long(nullable: false),
                        AttestationSnapshotActionId = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        TaskId = c.Long(nullable: false),
                        Approve = c.Boolean(nullable: false),
                        Revoke = c.Boolean(nullable: false),
                        Comment = c.String(),
                        EffectiveDate = c.DateTime(),
                        ReviewerId = c.Long(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AttestationSnapshotActionInterim",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationSnapshotActionId = c.Long(nullable: false),
                        TaskId = c.Long(nullable: false),
                        ReviewerId = c.Long(),
                        UnSavedApprove = c.Boolean(nullable: false),
                        UnSavedRevoke = c.Boolean(nullable: false),
                        UnSavedComment = c.String(),
                        UnSavedEffectiveDate = c.DateTime(),
                        UnSavedIsChanged = c.Boolean(nullable: false),
                        UnSavedReserved1 = c.String(),
                        UnSavedReserved2 = c.String(),
                        UnSavedReserved3 = c.String(),
                        SavedApprove = c.Boolean(nullable: false),
                        SavedRevoke = c.Boolean(nullable: false),
                        SavedComment = c.String(),
                        SavedEffectiveDate = c.DateTime(),
                        SavedIsChanged = c.Boolean(nullable: false),
                        SavedReserved1 = c.String(),
                        SavedReserved2 = c.String(),
                        SavedReserved3 = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AttestationSnapshotAction", t => t.AttestationSnapshotActionId)
                .Index(t => t.AttestationSnapshotActionId);
            
            CreateTable(
                "dbo.DI_M_AttestationSnapshotAction",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationSnapshotId = c.Long(nullable: false),
                        TaskId = c.Long(nullable: false),
                        Approve = c.Boolean(nullable: false),
                        Revoke = c.Boolean(nullable: false),
                        Comment = c.String(),
                        EffectiveDate = c.DateTime(),
                        ReviewerId = c.Long(),
                        IsReviewerGroup = c.Boolean(nullable: false),
                        Expired = c.Boolean(nullable: false),
                        IsChanged = c.Boolean(nullable: false),
                        IsChangedInPreviousLevel = c.Boolean(nullable: false),
                        IsDelta = c.Boolean(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        IsActionable = c.Boolean(nullable: false),
                        IsActionExtendable = c.Boolean(nullable: false),
                        IsException = c.Boolean(nullable: false),
                        Reserved1 = c.String(),
                        Reserved2 = c.String(),
                        Reserved3 = c.String(),
                        RootSnapshotId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        RemediationStatus = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AttestationSnapshot", t => t.AttestationSnapshotId)
                .Index(t => t.AttestationSnapshotId);
            
            CreateTable(
                "dbo.DI_M_AttestationSnapshot",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationCampaignId = c.Long(nullable: false),
                        UserId = c.Long(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        KeyId = c.Long(nullable: false),
                        Entity = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        AccountName = c.String(),
                        DIUserName = c.String(),
                        DIUserDisplayName = c.String(),
                        EndpointId = c.Long(nullable: false),
                        EndpointName = c.String(),
                        EndpointType = c.Int(nullable: false),
                        ParentEntity = c.String(),
                        LastLogon = c.DateTime(),
                        Level = c.Long(nullable: false),
                        EntitlementType = c.Int(nullable: false),
                        HasRisk = c.Boolean(nullable: false),
                        AccountStatus = c.Boolean(nullable: false),
                        RootSnapshotId = c.Long(),
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
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AttestationCampaign", t => t.AttestationCampaignId)
                .Index(t => t.AttestationCampaignId);
            
            CreateTable(
                "dbo.DI_M_AttestationSnapshotTaskStatus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttestationId = c.Long(nullable: false),
                        CampaignId = c.Long(nullable: false),
                        TaskId = c.Long(nullable: false),
                        UnSavedChangesAvailable = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AuditConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TableName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_AuditConfigurationDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TableId = c.Long(),
                        PropertyName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AuditConfiguration", t => t.TableId)
                .Index(t => t.TableId);
            
            CreateTable(
                "dbo.AuditLog",
                c => new
                    {
                        AuditLogId = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        EventDateUTC = c.DateTime(nullable: false),
                        EventType = c.Int(nullable: false),
                        TypeFullName = c.String(nullable: false, maxLength: 512),
                        RecordId = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.AuditLogId);
            
            CreateTable(
                "dbo.AuditLogDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PropertyName = c.String(nullable: false, maxLength: 256),
                        OriginalValue = c.String(),
                        NewValue = c.String(),
                        AuditLogId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditLog", t => t.AuditLogId)
                .Index(t => t.AuditLogId);
            
            CreateTable(
                "dbo.LogMetadata",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AuditLogId = c.Long(nullable: false),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditLog", t => t.AuditLogId)
                .Index(t => t.AuditLogId);
            
            CreateTable(
                "dbo.DI_HRMS_Company",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Text = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_CompliancePolicy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompliancePolicyCode = c.String(),
                        CompliancePolicyName = c.String(),
                        CompliancePolicyDescription = c.String(),
                        CompliancePolicyType = c.String(),
                        Status = c.String(),
                        OwnerId = c.Long(),
                        IsActive = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.DI_M_SODConflictFunctions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SodConflictCode = c.String(),
                        SodConflictDescription = c.String(),
                        CompliancePolicyType = c.String(),
                        RiskCodeId = c.Long(nullable: false),
                        LastGeneratedDate = c.DateTime(),
                        IsActive = c.Boolean(),
                        Status = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODRiskCode", t => t.RiskCodeId)
                .Index(t => t.RiskCodeId);
            
            CreateTable(
                "dbo.DI_M_SODFunction",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SodFunctionCode = c.String(),
                        SodFunctionDescription = c.String(),
                        IsActive = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_SODFunctionItem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SodFunctionId = c.Long(),
                        SodFunctionType = c.String(),
                        SodRoleId = c.String(),
                        SodRoleDesc = c.String(),
                        ObjectCode = c.String(),
                        FieldCode = c.String(),
                        FromValue = c.String(),
                        EndpointSystemTypeId = c.Long(),
                        Endpointsystemid = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
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
                        Id = c.Long(nullable: false, identity: true),
                        RiskCode = c.String(),
                        RiskDescription = c.String(),
                        RiskLevelId = c.Long(),
                        Status = c.Int(nullable: false),
                        UserId = c.Long(),
                        RiskScoreId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODRiskLevel", t => t.RiskLevelId)
                .ForeignKey("dbo.DI_M_SODRiskScore", t => t.RiskScoreId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.RiskLevelId)
                .Index(t => t.UserId)
                .Index(t => t.RiskScoreId);
            
            CreateTable(
                "dbo.DI_M_SODMitigation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        MitigationCode = c.String(),
                        MitigationDescription = c.String(),
                        MitigationSteps = c.String(),
                        ApproverUserId = c.Long(),
                        ApproverGroupId = c.Long(),
                        MonitorUserId = c.Long(),
                        MonitorGroupId = c.Long(),
                        IsActive = c.Boolean(),
                        DocumentName = c.String(),
                        DocumentUploadedPath = c.String(),
                        DocumentUploadedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.ApproverUserId)
                .ForeignKey("dbo.DI_M_User", t => t.MonitorUserId)
                .Index(t => t.ApproverUserId)
                .Index(t => t.MonitorUserId);
            
            CreateTable(
                "dbo.DI_M_SODRiskLevel",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RiskLevelDescription = c.String(),
                        RiskLevelCode = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_SODRiskScore",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RiskScore = c.Int(nullable: false),
                        RiskScoreDescription = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ComplianceProcess",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProcessName = c.String(),
                        Description = c.String(),
                        PolicyExecutionDefinitionId = c.Long(nullable: false),
                        Default = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", t => t.PolicyExecutionDefinitionId)
                .Index(t => t.PolicyExecutionDefinitionId);
            
            CreateTable(
                "dbo.DI_M_SODPolicyExceutionDefinition",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompliancePolicyId = c.Long(),
                        UniqueRunCode = c.String(),
                        OutputType = c.Int(nullable: false),
                        RunDate = c.DateTime(),
                        RunAtObjectLevel = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_CompliancePolicy", t => t.CompliancePolicyId)
                .Index(t => t.CompliancePolicyId);
            
            CreateTable(
                "dbo.DI_M_SODEndPointPolicySelection",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SODPolicyExceutionId = c.Long(nullable: false),
                        EndpointTypeName = c.String(),
                        LevelName = c.String(),
                        LevelValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", t => t.SODPolicyExceutionId)
                .Index(t => t.SODPolicyExceutionId);
            
            CreateTable(
                "dbo.DI_M_SODException",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ExceptionCode = c.String(),
                        ExceptionDesc = c.String(),
                        RiskOwnerId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.RiskOwnerId)
                .Index(t => t.RiskOwnerId);
            
            CreateTable(
                "dbo.DI_M_SODExceptionItem",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ExceptionId = c.Long(),
                        UserId = c.Long(),
                        FunctionId1 = c.Long(),
                        FunctionItemId1 = c.Long(),
                        FunctionId2 = c.Long(),
                        FunctionItemId2 = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId1)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId2)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItemId1)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItemId2)
                .ForeignKey("dbo.DI_M_SODException", t => t.ExceptionId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.ExceptionId)
                .Index(t => t.UserId)
                .Index(t => t.FunctionId1)
                .Index(t => t.FunctionItemId1)
                .Index(t => t.FunctionId2)
                .Index(t => t.FunctionItemId2);
            
            CreateTable(
                "dbo.DI_M_ComplianceProcessHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProcessId = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        Status = c.String(),
                        Message = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        RecordStatus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ComplianceReports",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ReportName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ComplianceType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PolicyTypeName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ConfigurationMasterHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigId = c.Short(nullable: false),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ComplianceConfigurationMaster",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsUiDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_DGMConfigurationMaster",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsUiDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_IACMConfigurationMaster",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsUiDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_IMConfigurationMaster",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsUiDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_PMConfigurationMaster",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsUiDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_PIMConfigurationMaster",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigKey = c.String(),
                        ConfigValue = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsUiDisplay = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_DashboardAccessRights",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleId = c.Long(nullable: false),
                        DashboardId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Dashboard", t => t.DashboardId)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.DashboardId);
            
            CreateTable(
                "dbo.DI_M_Dashboard",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DashboardUniqueName = c.String(),
                        DashboardName = c.String(),
                        DashboardCategory = c.String(),
                        DashBoardType = c.Byte(nullable: false),
                        TargetUrl = c.String(),
                        NavigateUrl = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_DefaultRoleDashboard",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleId = c.Long(nullable: false),
                        DashBoardId = c.Long(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Dashboard", t => t.DashBoardId)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.DashBoardId);
            
            CreateTable(
                "dbo.DI_M_DefaultRoleQuickLink",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleId = c.Long(nullable: false),
                        QuickLinkId = c.Long(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_QuickLink", t => t.QuickLinkId)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.QuickLinkId);
            
            CreateTable(
                "dbo.DI_M_QuickLink",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Category = c.String(),
                        QuickLinkId = c.Long(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Discovery",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LogLevel = c.String(),
                        DiscoveryProcessId = c.Long(),
                        EndpointSystemType = c.Int(nullable: false),
                        EndpointSystemName = c.String(),
                        EndpointSystemId = c.Long(nullable: false),
                        DiscoveryObjectType = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_DiscoveryProcessHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProcessId = c.Int(nullable: false),
                        HistoryId = c.Int(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        Status = c.String(),
                        Message = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        RecordStatus = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_EmailHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
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
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_Email",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
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
                        CreatedDateTime = c.DateTime(),
                        DispatchDateTime = c.DateTime(),
                        SentDateTime = c.DateTime(),
                        NoOfAttempts = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Email",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EmailId = c.Long(),
                        EmailSubject = c.String(),
                        LogLevel = c.String(),
                        LogType = c.String(),
                        Reviewer = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_EmployeeEventTrigger",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        EventType = c.Byte(nullable: false),
                        EffectiveDate = c.DateTime(nullable: false),
                        OldEffectiveDate = c.DateTime(),
                        IsRequestCreated = c.Boolean(nullable: false),
                        IsAdminCheckRequired = c.Boolean(nullable: false),
                        UserDetails = c.String(),
                        OldUserDetails = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        RequestStatus = c.Long(nullable: false),
                        Comments = c.String(),
                        IsMailSent = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        UserEventId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_EmployeeTerm",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EmployeeTermDesc = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_EndPointAutoExpiry",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Long(nullable: false),
                        EndPointSystemId = c.Long(),
                        IsAccountAutoExpirySupported = c.Boolean(nullable: false),
                        IsEntitlementAutoExpirySupported = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsAutoExpiryApprovalRequired = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_EndpointConfigurationDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointConfigurationId = c.Long(),
                        KeyId = c.Long(),
                        Value = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointConfiguration", t => t.EndpointConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointConfigurationKey", t => t.KeyId)
                .Index(t => t.EndpointConfigurationId)
                .Index(t => t.KeyId);
            
            CreateTable(
                "dbo.DI_M_EndpointConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(),
                        MapAll = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_EndpointConfigurationKey",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        KeyName = c.String(),
                        Caption = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_EndpointKeyConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Long(nullable: false),
                        KeyId = c.Long(),
                        Order = c.Int(nullable: false),
                        DataType = c.String(),
                        RegExValidationPattern = c.String(),
                        ClientScript = c.String(),
                        IsRequired = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointConfigurationKey", t => t.KeyId)
                .Index(t => t.KeyId);
            
            CreateTable(
                "dbo.DI_M_EndpointUserViewConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(),
                        KeyName = c.String(),
                        Caption = c.String(),
                        Value = c.String(),
                        Order = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId)
                .Index(t => t.EndpointSystemTypeId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_EntitlementReconcilation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AccountReconcilationId = c.Long(nullable: false),
                        EndpointType = c.Int(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        EndpointUserId = c.Long(nullable: false),
                        UserId = c.Long(),
                        EntitlementId = c.Long(nullable: false),
                        EntitlementType = c.Int(nullable: false),
                        AccountName = c.String(),
                        Entitlement = c.String(),
                        EntitlementDescription = c.String(),
                        EnteredDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        IsResolved = c.Boolean(nullable: false),
                        ResolvedBy = c.String(),
                        Comments = c.String(),
                        ExceptionType = c.Byte(nullable: false),
                        ResolvedStatus = c.Byte(nullable: false),
                        ProvisionedThrough = c.String(),
                        IsSODConflict = c.Boolean(nullable: false),
                        IsSensitiveAccess = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastAttestedOn = c.DateTime(),
                        ValidFromDate = c.DateTime(),
                        ValidToDate = c.DateTime(),
                        AccessRequestId = c.Long(),
                        ExceptionOccuredDate = c.DateTime(),
                        ResolvedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AccountReconcilation", t => t.AccountReconcilationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.AccountReconcilationId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_ReconcilationExceptionAttributeChanges",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ReconcilationId = c.Long(nullable: false),
                        ExceptionAttributeId = c.Long(nullable: false),
                        Attributes = c.String(),
                        OldValue = c.String(),
                        NewValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AccountReconcilation", t => t.ReconcilationId)
                .ForeignKey("dbo.DI_M_ReconcilationExceptionAttribute", t => t.ExceptionAttributeId)
                .Index(t => t.ReconcilationId)
                .Index(t => t.ExceptionAttributeId);
            
            CreateTable(
                "dbo.DI_M_ReconcilationExceptionAttribute",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Long(nullable: false),
                        Attributes = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_GlobalAnswer",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        Question = c.String(),
                        Answer = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_GlobalQuestion",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Question = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_GroupHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        GroupName = c.String(),
                        GroupType = c.String(),
                        GroupOwner = c.String(),
                        GroupDescription = c.String(),
                        IsDefaultGroup = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_IACM_GroupInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        UserDefinedDisplayName = c.String(),
                        UserDefinedDesc = c.String(),
                        CanRequest = c.Boolean(),
                        Level1Approver = c.Long(),
                        Level2Approver = c.Long(),
                        IsLevel1Group = c.Boolean(),
                        IsLevel2Group = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_IACM_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        NRIC = c.String(),
                        EmployeeId = c.String(),
                        Email = c.String(),
                        UserName = c.String(),
                        DisplayName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        ADID = c.String(),
                        AccountCreatedDate = c.DateTime(),
                        LastLoginDate = c.DateTime(),
                        LockedDate = c.DateTime(),
                        IsDisabled = c.Boolean(nullable: false),
                        DisabledDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_IACM_RoleInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        CanRequest = c.Boolean(),
                        UserDefinedDisplayName = c.String(),
                        UserDefinedDesc = c.String(),
                        Level1Approver = c.Long(),
                        Level2Approver = c.Long(),
                        IsLevel1Group = c.Boolean(),
                        IsLevel2Group = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_INH_Permission",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        InhouseSystemCode = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        UserDefinedDesc = c.String(),
                        CanRequest = c.Boolean(),
                        UserDefinedDisplayName = c.String(),
                        Level1Approver = c.Long(),
                        Level2Approver = c.Long(),
                        IsLevel1Group = c.Boolean(),
                        IsLevel2Group = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_INH_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        InhouseSystemCode = c.String(),
                        IdentificationType = c.String(),
                        IdentificationCountry = c.String(),
                        EmployeeId = c.String(),
                        NRIC = c.String(),
                        Email = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        DisplayName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        AccountCreatedDate = c.DateTime(),
                        LastLoginDate = c.DateTime(),
                        LockedDate = c.DateTime(),
                        IsLocked = c.Boolean(nullable: false),
                        IsDisabled = c.Boolean(nullable: false),
                        DisabledDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        DeletedDate = c.DateTime(),
                        Remarks1 = c.String(),
                        Remarks2 = c.String(),
                        Remarks3 = c.String(),
                        Remarks4 = c.String(),
                        Remarks5 = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_M_InterimStore",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InterimStoreItemType = c.Byte(nullable: false),
                        TaskId = c.Long(),
                        UserId = c.Long(),
                        XmlData = c.String(),
                        IsSaved = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_MailNotificationSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TriggerName = c.String(),
                        MailTriggerFromJob = c.Boolean(nullable: false),
                        MailTriggerOnEffectiveDate = c.Boolean(nullable: false),
                        From = c.String(),
                        To = c.String(),
                        Cc = c.String(),
                        PreConfigureNotificationDays = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_MailServerConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EmailID = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        HostName = c.String(),
                        PortNumber = c.String(),
                        EnableSSL = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_MailTemplate",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TemplateName = c.String(),
                        TemplateDescription = c.String(),
                        MailFrom = c.String(),
                        MailTo = c.String(),
                        MailCC = c.String(),
                        MailBcc = c.String(),
                        MailSubject = c.String(),
                        MailContent = c.String(),
                        ApplicableVariableScopes = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        TemplateScope = c.Byte(nullable: false),
                        MailTemplateName = c.Byte(nullable: false),
                        ActivityType = c.Int(nullable: false),
                        IsActive = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_MailTemplateVariable",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DisplayName = c.String(),
                        VariableValue = c.String(),
                        VariableValuePath1 = c.String(),
                        VariableValuePath2 = c.String(),
                        VariableScope = c.Byte(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_NCostCenter",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_HRMS_CostCenter",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        CompanyId = c.Long(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_HRMS_Designation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_NOrganisationHead",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrganisationId = c.Long(nullable: false),
                        HeadId = c.Long(nullable: false),
                        CoordinatorId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.HeadId)
                .ForeignKey("dbo.DI_M_NOrganisation", t => t.OrganisationId)
                .Index(t => t.OrganisationId)
                .Index(t => t.HeadId);
            
            CreateTable(
                "dbo.DI_M_NOrganisationRelationship",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrganisationId = c.Long(nullable: false),
                        ScopeId = c.Long(nullable: false),
                        Value = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_NOrganisation", t => t.OrganisationId)
                .Index(t => t.OrganisationId);
            
            CreateTable(
                "dbo.DI_M_NotificationAttachment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        NotificationId = c.Long(nullable: false),
                        ContentType = c.String(),
                        FileName = c.String(),
                        Attachment = c.Binary(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Notification", t => t.NotificationId)
                .Index(t => t.NotificationId);
            
            CreateTable(
                "dbo.DI_M_Notification",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ParentId = c.Long(),
                        Subject = c.String(),
                        Content = c.String(),
                        SenderId = c.Long(nullable: false),
                        ReceiverId = c.Long(nullable: false),
                        SenderTaskId = c.Long(),
                        ReceiverTaskId = c.Long(),
                        RequestId = c.Long(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        IsReplied = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.ReceiverId)
                .ForeignKey("dbo.DI_M_User", t => t.SenderId)
                .Index(t => t.SenderId)
                .Index(t => t.ReceiverId);
            
            CreateTable(
                "dbo.DI_M_NProvisioningField",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemTypeId = c.Long(nullable: false),
                        EndPointSystemId = c.Long(nullable: false),
                        AccountType = c.Byte(nullable: false),
                        FieldName = c.String(),
                        Description = c.String(),
                        Value = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndPointSystemTypeId)
                .Index(t => t.EndPointSystemTypeId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_M_NRulesetForApplication",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RuleId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        AccountType = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_NRuleset",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RuleId = c.Long(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        TypeId = c.Long(nullable: false),
                        Value = c.String(),
                        Operator = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_NRulesetType", t => t.TypeId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.DI_M_NRulesetType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RulesetType = c.String(),
                        Description = c.String(),
                        Score = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_NScopeFor",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ScopeFor = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_NUserDesignation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        DesignationId = c.Long(nullable: false),
                        Category = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_NDesignation", t => t.DesignationId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DesignationId);
            
            CreateTable(
                "dbo.DI_M_NUserOrganisation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        OrganisationId = c.Long(nullable: false),
                        TypeId = c.Long(nullable: false),
                        Category = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_NOrganisation", t => t.OrganisationId)
                .ForeignKey("dbo.DI_M_NScopeFor", t => t.TypeId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.OrganisationId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.DI_M_OffboardingAccountAction",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Desc = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_OUChangeSnapshot",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BatchId = c.String(),
                        UserId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        EndpointUserId = c.Long(nullable: false),
                        EntitlementType = c.Int(nullable: false),
                        Entitlement = c.String(),
                        EnteredDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_PasswordActionInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        EndpointUserId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        PasswordActionType = c.Byte(nullable: false),
                        ActionDate = c.DateTime(nullable: false),
                        IsSuccess = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_PasswordPolicy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        PasswordLength = c.Int(),
                        PasswordMinimumLength = c.Int(),
                        PasswordMaximumLength = c.Int(),
                        NoOfSpecialCharacters = c.Int(),
                        SpecialCharNotAllowed = c.Boolean(nullable: false),
                        NoOfLowerCaseAlphabets = c.Int(),
                        NoOfUpperCaseAlphabets = c.Int(),
                        AlphabetsNotAllowed = c.Boolean(nullable: false),
                        NoOfNumericCharacters = c.Int(),
                        NumericCharacterNotAllowed = c.Boolean(nullable: false),
                        NotAllowedCharacters = c.String(),
                        NoOfDaysPasswordChangeAllowed = c.Int(),
                        NotAllowedKeywords = c.String(),
                        UserNameNotAllowedInPassword = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_PasswordPolicyEndpoint",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PolicyId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .ForeignKey("dbo.DI_M_PasswordPolicy", t => t.PolicyId)
                .Index(t => t.PolicyId)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_M_PasswordSyncConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AllowPasswordSyncThruFilter = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_PasswordSyncConfigurationDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigId = c.Int(nullable: false),
                        EndpointSystemTypeId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        AllowSync = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        PasswordSyncConfiguration_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_PasswordSyncConfiguration", t => t.PasswordSyncConfiguration_Id)
                .Index(t => t.PasswordSyncConfiguration_Id);
            
            CreateTable(
                "dbo.DI_HRMS_PersonnelArea",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Text = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_HRMS_PersonnelSubArea",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PersonnelAreaId = c.Long(nullable: false),
                        Code = c.String(),
                        Text = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ProcessHistroy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProcessType = c.Int(nullable: false),
                        ProcessId = c.Long(nullable: false),
                        ProcessStartAt = c.DateTime(),
                        ProcessEndAt = c.DateTime(),
                        Status = c.String(),
                        ErrorMessage = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Process", t => t.ProcessId)
                .Index(t => t.ProcessId);
            
            CreateTable(
                "dbo.DI_M_ProvisioningConfigMatrixSAPEEGroup",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EmployeeGroupNo = c.String(),
                        EmployeeGroup = c.String(),
                        PArea = c.String(),
                        DefaultRole = c.String(),
                        Remark = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ProvisioningConfigMatrixSAPORG",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
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
                        personnelArea = c.String(),
                        PersonnelSubArea = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ProvisioningConfigMatrixSAPPosition",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PositionText = c.String(),
                        DefaultRole = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_Provisioning",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequestId = c.Long(),
                        RequestUserId = c.Long(),
                        RequestDetailId = c.Long(),
                        ReferenceType = c.Int(nullable: false),
                        Reference = c.String(),
                        LogLevel = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_PurgeConfig",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TableName = c.String(),
                        ObjectName = c.String(),
                        Description = c.String(),
                        DaysToKeep = c.Int(nullable: false),
                        Status = c.String(),
                        PurgeMode = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastRunOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_QuickLinkAccessRights",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleId = c.Long(nullable: false),
                        QuickLinkId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_QuickLink", t => t.QuickLinkId)
                .ForeignKey("dbo.DI_M_Role", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.QuickLinkId);
            
            CreateTable(
                "dbo.DI_M_Report",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Format = c.String(),
                        Name = c.String(),
                        ReportUrl = c.String(),
                        UserId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        MailTo = c.String(),
                        MailCC = c.String(),
                        process_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Process", t => t.process_Id)
                .Index(t => t.process_Id);
            
            CreateTable(
                "dbo.DI_M_ReportAttributes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ReportId = c.Long(nullable: false),
                        ParamKey = c.String(),
                        ParamValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Report", t => t.ReportId)
                .Index(t => t.ReportId);
            
            CreateTable(
                "dbo.DI_M_ReportHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ReportId = c.Long(nullable: false),
                        Report = c.Binary(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Report", t => t.ReportId)
                .Index(t => t.ReportId);
            
            CreateTable(
                "dbo.DI_M_RequestApprovalConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequestSource = c.Long(nullable: false),
                        IsApprovalRequired = c.Boolean(nullable: false),
                        WorkflowConfigurationId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId)
                .Index(t => t.WorkflowConfigurationId);
            
            CreateTable(
                "dbo.DI_M_RequestAttachment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequestType = c.Byte(nullable: false),
                        AttachmentNote = c.String(),
                        AccessRequestId = c.Long(),
                        AttestationId = c.Int(),
                        WorkflowStateId = c.Byte(nullable: false),
                        Attachment = c.Binary(),
                        IsActive = c.Boolean(nullable: false),
                        UploadedBy = c.Long(nullable: false),
                        UploadedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Attestation_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AccessRequest", t => t.AccessRequestId)
                .ForeignKey("dbo.DI_M_Attestation", t => t.Attestation_Id)
                .ForeignKey("dbo.DI_M_User", t => t.UploadedBy)
                .Index(t => t.AccessRequestId)
                .Index(t => t.UploadedBy)
                .Index(t => t.Attestation_Id);
            
            CreateTable(
                "dbo.DI_M_RoleHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        RoleName = c.String(),
                        RoleDescription = c.String(),
                        IsDefaultRole = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_RoleTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DisplayName = c.String(),
                        RoleTypeDesc = c.String(),
                        ApplicableApprovalUserTypes = c.String(),
                        ApplicableRequestTypes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_ActivityInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ActivityValue = c.String(),
                        ActivityDescription = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_AuthorizationProfile",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ProfileCode = c.String(),
                        ObjectCode = c.String(),
                        AuthorizationCode = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_AuthObjectMap",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AuthorizationCode = c.String(),
                        ObjectCode = c.String(),
                        FieldCode = c.String(),
                        FromValue = c.String(),
                        ToValue = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_BIRoleCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Remarks = c.String(maxLength: 250),
                        Application = c.String(maxLength: 250),
                        Module = c.String(maxLength: 250),
                        SubModule = c.String(maxLength: 250),
                        BusinessRole = c.String(maxLength: 250),
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
                "dbo.DI_SAP_COMPOSITEPROFILEINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        PROFILE_CODE = c.String(),
                        PROFILE_DESC = c.String(),
                        SUBProfile_Code = c.String(),
                        TargetSysId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_COMPOSITEROLEINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompositeRole_Code = c.String(),
                        CompositeRole_Desc = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        ModifiedBy = c.String(),
                        ModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_SINGLEROLEINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SingleRole_Code = c.String(),
                        SingleRole_Desc = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_TCODEINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TCODE_Code = c.String(),
                        TCODE_Desc = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPEP_GroupInfoHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        HistoryId = c.Int(nullable: false),
                        EndPointSystemId = c.Int(),
                        GroupId = c.String(),
                        GroupName = c.String(),
                        GroupDescription = c.String(),
                        Extar1 = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPEP_GroupInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(),
                        GroupId = c.String(),
                        GroupName = c.String(),
                        GroupDescription = c.String(),
                        GroupUniqueName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
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
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                        PwdResetCnt = c.Int(nullable: false),
                        Extra1 = c.String(),
                        Extra2 = c.String(),
                        Extra3 = c.String(),
                        IACMLastPasswordChange = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAPEP_RoleInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(),
                        RoleId = c.String(),
                        RoleName = c.String(),
                        RoleDescription = c.String(),
                        UniqueRoleName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                        IsXLUploaded = c.Boolean(nullable: false),
                        UserDefinedDesc = c.String(),
                        CanRequest = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPEP_RoleInfoHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        HistoryId = c.Int(nullable: false),
                        EndPointSystemId = c.Int(),
                        RoleId = c.String(),
                        RoleName = c.String(),
                        RoleDescription = c.String(),
                        Extar1 = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        RecordStatus = c.String(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPEP_RoleInfoTemp",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleUniqueName = c.String(),
                        RoleFullName = c.String(),
                        Version = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPEP_UserInfoHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EndPointSystemId = c.Long(nullable: false),
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
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_FIELDVALUEMAPPING",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleCode = c.String(),
                        FieldCode = c.String(),
                        FromValue = c.String(),
                        ToValue = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_GROUPHISTORYINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        HistoryId = c.Int(),
                        EndPointSystemId = c.Int(),
                        GROUP_CODE = c.String(),
                        GROUP_DESC = c.String(),
                        TargetSysId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_GROUPINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        GROUP_CODE = c.String(),
                        GROUP_DESC = c.String(),
                        TargetSysId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                        IACMLastPasswordChange = c.DateTime(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        LastModifiedBy = c.String(),
                        Position = c.String(),
                        CostCenter = c.String(),
                        validFromDate = c.DateTime(),
                        validToDate = c.DateTime(),
                        SapUserType = c.String(),
                        IsUserLocked = c.Boolean(nullable: false),
                        NeedEPGroupAssignment = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAP_PARAMETERINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PARAM_CODE = c.String(),
                        EndPointSystemId = c.Int(),
                        PARAM_DESC = c.String(),
                        PARAM_VALUE = c.String(),
                        TargetSysId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_PROFILEINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        PROFILE_CODE = c.String(),
                        PROFILE_DESC = c.String(),
                        TargetSysId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_ROLEINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        ROLE_CODE = c.String(),
                        ROLE_DESC = c.String(),
                        TargetSysId = c.String(),
                        RoleType = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                        UserDefinedDesc = c.String(),
                        CanRequest = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_SAPHRFLINSConflictingRole",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessRole = c.String(),
                        ConflictingBusinessRole = c.String(),
                        TechnicalRole = c.String(),
                        ConflictingTechnicalRole = c.String(),
                        IsModified = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_HRFLINSRoleMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Remarks = c.String(maxLength: 250),
                        Application = c.String(maxLength: 250),
                        Company = c.String(maxLength: 250),
                        CostCenter = c.String(maxLength: 250),
                        PurchasingOrg = c.String(maxLength: 250),
                        PurchasingGrp = c.String(maxLength: 250),
                        Plant = c.String(maxLength: 250),
                        MRPControl = c.String(maxLength: 250),
                        StorageLocation = c.String(maxLength: 250),
                        Module = c.String(maxLength: 250),
                        SubModule = c.String(maxLength: 250),
                        BusinessRole = c.String(maxLength: 250),
                        TechnicalRule = c.String(maxLength: 250),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        MainSupp = c.String(maxLength: 250),
                        IsModified = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        IsUpdatedbyExcelUpload = c.Boolean(),
                        ReferenceId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_HRFLINSSuppRoleMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Rule = c.String(maxLength: 250),
                        Company = c.String(maxLength: 250),
                        CostCenter = c.String(maxLength: 250),
                        PurchasingOrg = c.String(maxLength: 250),
                        PurchasingGrp = c.String(maxLength: 250),
                        Plant = c.String(maxLength: 250),
                        MRPControl = c.String(maxLength: 250),
                        StorageLocation = c.String(maxLength: 250),
                        Role = c.String(maxLength: 250),
                        RoleName = c.String(maxLength: 250),
                        IsModified = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        IsModifiedbyExcelUpdate = c.Boolean(),
                        ReferenceId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_SAPHRP1001",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        ActualObjectCode = c.String(),
                        RelatedObjectType = c.String(),
                        RelatedObjectCode = c.String(),
                        BeginDate = c.String(),
                        EndDate = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_Objects",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
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
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_ObjectValues",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SapObjectType = c.String(),
                        SapObjectCode = c.String(),
                        SapOkFlag = c.String(),
                        ModificationID = c.String(),
                        OrgName = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_PARAMETERHISTORYINFO",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PARAM_CODE = c.String(),
                        EndPointSystemId = c.Int(),
                        PARAM_DESC = c.String(),
                        PARAM_VALUE = c.String(),
                        TargetSysId = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_Position",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_RoleMandatoryFields",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BusinessRole = c.String(maxLength: 250),
                        MandatoryField1 = c.String(maxLength: 250),
                        MandatoryField2 = c.String(maxLength: 250),
                        IsModified = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        IsModifiedbyExcelUpload = c.Boolean(),
                        ReferenceId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_RoleMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Application = c.String(maxLength: 250),
                        EligibilityRule = c.String(maxLength: 250),
                        Company = c.String(maxLength: 250),
                        PersonnelArea = c.String(maxLength: 250),
                        PersonnelSubArea = c.String(maxLength: 250),
                        Position = c.String(maxLength: 250),
                        OrgUnit = c.String(maxLength: 250),
                        CostCenter = c.String(maxLength: 250),
                        Remarks = c.String(maxLength: 250),
                        IsModified = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        IsUpdatedbyExcel = c.Boolean(),
                        ReferenceId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_ROLEPROFILE",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleName = c.String(),
                        ProfileName = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_ROLETCODEMAPPING",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleCode = c.String(),
                        TCode = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_RoleToAuthMap",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleName = c.String(),
                        ObjectCode = c.String(),
                        AuthCode = c.String(),
                        Variant = c.String(),
                        BiwMenuCounter = c.String(),
                        FieldCode = c.String(),
                        ProfileGeneratorVariant = c.String(),
                        FromValue = c.String(),
                        ToValue = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_TCodeObject",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ObjectType = c.String(),
                        FieldValue = c.String(),
                        ObjectCode = c.String(),
                        FieldCode = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_UserHistoryInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        EndPointSystemId = c.Long(nullable: false),
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
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_UserProfile",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        ProfileName = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAP_USEROBJECTMAPPING",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserLogonName = c.String(),
                        RoleCode = c.String(),
                        ProfileCode = c.String(),
                        AuthorizationCode = c.String(),
                        ObjectCode = c.String(),
                        FieldCode = c.String(),
                        FromValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_ServiceAccountQuestion",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_ServiceLog",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ReferenceId = c.Long(),
                        ReferenceType = c.Int(nullable: false),
                        LogLevel = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_SODReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SODPolicyID = c.Long(),
                        FunctionId1 = c.Long(),
                        Role1 = c.String(),
                        FunctionItem1 = c.Long(),
                        FunctionType1 = c.String(),
                        FunctionId2 = c.Long(),
                        Role2 = c.String(),
                        FunctionItem2 = c.Long(),
                        FunctionType2 = c.String(),
                        AuthorizationCode1 = c.String(),
                        AuthorizationCode2 = c.String(),
                        ProfileCode1 = c.String(),
                        ProfileCode2 = c.String(),
                        RoleCode1 = c.String(),
                        RoleCode2 = c.String(),
                        ConflictId = c.Long(),
                        ConflictCode = c.String(),
                        UserId = c.Long(nullable: false),
                        UserName = c.String(),
                        RunCode = c.String(),
                        PassedObjectLevelLeft = c.Boolean(nullable: false),
                        PassedObjectLevelRight = c.Boolean(nullable: false),
                        TotalMatching = c.Int(nullable: false),
                        MitigationCode = c.String(),
                        SODRunPolicyId = c.Long(),
                        ExceptionId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
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
                        Id = c.Long(nullable: false, identity: true),
                        CompliancePolicyId = c.Long(),
                        UniqueRunCode = c.String(),
                        RiskLevelId = c.Int(),
                        RiskScoreFrom = c.Int(),
                        RiskScoreTo = c.Int(),
                        RunDate = c.DateTime(nullable: false),
                        RunAtObjectLevel = c.Boolean(),
                        WorkflowConfigurationId = c.Long(),
                        ExceptionCode = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        RiskLevel_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_CompliancePolicy", t => t.CompliancePolicyId)
                .ForeignKey("dbo.DI_M_SODRiskLevel", t => t.RiskLevel_Id)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId)
                .Index(t => t.CompliancePolicyId)
                .Index(t => t.WorkflowConfigurationId)
                .Index(t => t.RiskLevel_Id);
            
            CreateTable(
                "dbo.DI_M_SODRunPolicySelection",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SODRunPolicyId = c.Int(nullable: false),
                        EndpointTypeName = c.String(),
                        LevelName = c.String(),
                        LevelValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        SODRunPolicy_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODRunPolicy", t => t.SODRunPolicy_Id)
                .Index(t => t.SODRunPolicy_Id);
            
            CreateTable(
                "dbo.DI_M_SODConflictFunctionMap",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SodConflictFunctionId = c.Long(nullable: false),
                        SodFunctionId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.SodFunctionId)
                .Index(t => t.SodFunctionId);
            
            CreateTable(
                "dbo.DI_M_SODConflict",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SODConflictCode = c.String(),
                        SODConflictDescription = c.String(),
                        FunctionId1 = c.Long(),
                        FunctionId2 = c.Long(),
                        RiskCodeId = c.Long(nullable: false),
                        CompliancePolicyId = c.Int(nullable: false),
                        SodConflictType = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId1)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId2)
                .ForeignKey("dbo.DI_M_SODRiskCode", t => t.RiskCodeId)
                .Index(t => t.FunctionId1)
                .Index(t => t.FunctionId2)
                .Index(t => t.RiskCodeId);
            
            CreateTable(
                "dbo.DI_M_SODExecutionConfig",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemTypeId = c.Long(),
                        Level1 = c.String(),
                        Level2 = c.String(),
                        Level3 = c.String(),
                        Level4 = c.String(),
                        SODRunPolicyId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId)
                .Index(t => t.EndpointSystemTypeId);
            
            CreateTable(
                "dbo.DI_M_SODPolicy",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SODPolicyName = c.String(),
                        SODPolicyDescription = c.String(),
                        PolicyTypeId = c.Long(nullable: false),
                        BindType = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_ComplianceType", t => t.PolicyTypeId)
                .Index(t => t.PolicyTypeId);
            
            CreateTable(
                "dbo.DI_M_SODPolicyConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RiskLevelId = c.Long(nullable: false),
                        Operator = c.String(),
                        RiskScore = c.Long(nullable: false),
                        RiskScoreOperator = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODRiskLevel", t => t.RiskLevelId)
                .Index(t => t.RiskLevelId);
            
            CreateTable(
                "dbo.DI_M_SODPolicyReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PolicyExecutionDefinitionId = c.Long(),
                        UniqueRunCode = c.String(),
                        RunDate = c.DateTime(nullable: false),
                        FunctionId1 = c.Long(),
                        Role1 = c.String(),
                        FunctionItem1 = c.Long(),
                        FunctionType1 = c.String(),
                        FunctionId2 = c.Long(),
                        Role2 = c.String(),
                        FunctionItem2 = c.Long(),
                        FunctionType2 = c.String(),
                        AuthorizationCode1 = c.String(),
                        AuthorizationCode2 = c.String(),
                        ProfileCode1 = c.String(),
                        ProfileCode2 = c.String(),
                        RoleCode1 = c.String(),
                        RoleCode2 = c.String(),
                        ConflictId = c.Long(),
                        ConflictCode = c.String(),
                        UserId = c.Long(nullable: false),
                        UserName = c.String(),
                        RunCode = c.String(),
                        PassedObjectLevelLeft = c.Boolean(nullable: false),
                        PassedObjectLevelRight = c.Boolean(nullable: false),
                        TotalMatching = c.Int(nullable: false),
                        MitigationCode = c.String(),
                        SODRunPolicyId = c.Long(),
                        ExceptionId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_SODConflictFunctions", t => t.ConflictId)
                .ForeignKey("dbo.DI_M_SODException", t => t.ExceptionId)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId1)
                .ForeignKey("dbo.DI_M_SODFunction", t => t.FunctionId2)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItem1)
                .ForeignKey("dbo.DI_M_SODFunctionItem", t => t.FunctionItem2)
                .ForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", t => t.PolicyExecutionDefinitionId)
                .ForeignKey("dbo.DI_M_SODRunPolicy", t => t.SODRunPolicyId)
                .Index(t => t.PolicyExecutionDefinitionId)
                .Index(t => t.FunctionId1)
                .Index(t => t.FunctionItem1)
                .Index(t => t.FunctionId2)
                .Index(t => t.FunctionItem2)
                .Index(t => t.ConflictId)
                .Index(t => t.SODRunPolicyId)
                .Index(t => t.ExceptionId);
            
            CreateTable(
                "dbo.DI_M_SqlConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ServerName = c.String(),
                        Database = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        SqlAuthentication = c.Boolean(nullable: false),
                        MapAll = c.Boolean(),
                        EndPointSystemId = c.Long(),
                        DatabaseType = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_M_SubTask",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_TaskDelegation",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ActionUser = c.Long(nullable: false),
                        DelegatedTo = c.Long(nullable: false),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.DelegatedTo)
                .Index(t => t.DelegatedTo);
            
            CreateTable(
                "dbo.DI_M_Task",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RequestTypeId = c.Byte(nullable: false),
                        AttestationId = c.Long(),
                        AccessRequestId = c.Long(),
                        SubTaskId = c.Long(),
                        ActionUserId = c.Long(),
                        ActionGroupId = c.Long(),
                        WorkflowStateId = c.Byte(nullable: false),
                        WorkflowConfigurationId = c.Long(nullable: false),
                        WorkflowInstanceId = c.Guid(nullable: false),
                        IsEscalated = c.Boolean(nullable: false),
                        IsNotResponded = c.Boolean(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ActionDate = c.DateTime(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        Action = c.Byte(nullable: false),
                        Approver = c.String(),
                        Comments = c.String(),
                        Remarks = c.String(),
                        IsDelegated = c.Boolean(nullable: false),
                        DelegatedFrom = c.Long(),
                        DelegatedBy = c.String(),
                        DelegationComments = c.String(),
                        DelegatedDate = c.DateTime(),
                        IsActionOnlyRequiredTask = c.Boolean(nullable: false),
                        IsGroupTask = c.Boolean(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        AssignedTo = c.Long(),
                        AssignedBy = c.Long(),
                        GroupName = c.String(),
                        AttestationCampaignId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_AccessRequest", t => t.AccessRequestId)
                .ForeignKey("dbo.DI_M_User", t => t.ActionUserId)
                .ForeignKey("dbo.DI_M_Attestation", t => t.AttestationId)
                .ForeignKey("dbo.DI_M_AttestationCampaign", t => t.AttestationCampaignId)
                .ForeignKey("dbo.DI_M_User", t => t.DelegatedFrom)
                .ForeignKey("dbo.DI_M_SubTask", t => t.SubTaskId)
                .ForeignKey("dbo.DI_M_WorkflowConfiguration", t => t.WorkflowConfigurationId)
                .Index(t => t.AttestationId)
                .Index(t => t.AccessRequestId)
                .Index(t => t.SubTaskId)
                .Index(t => t.ActionUserId)
                .Index(t => t.WorkflowConfigurationId)
                .Index(t => t.DelegatedFrom)
                .Index(t => t.AttestationCampaignId);
            
            CreateTable(
                "dbo.DI_M_Config_General_TrustedSource",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AttributeName = c.String(),
                        TrustedSource = c.Int(nullable: false),
                        TrustedSourceAttribute = c.String(),
                        SelfUpdate = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Log_UI",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(),
                        LogLevel = c.String(),
                        UserName = c.String(),
                        IP = c.String(),
                        Message = c.String(),
                        ExceptionMessage = c.String(),
                        StackTrace = c.String(),
                        ExceptionSource = c.String(),
                        InnerExceptionMessage = c.String(),
                        LogDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_UserAttachment",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        FileName = c.String(),
                        AttachmentFile = c.Binary(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsAmendAttachment = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserCulturePreference",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CultureName = c.String(),
                        DateFormat = c.String(),
                        TimeFormat = c.String(),
                        LongDateFormat = c.String(),
                        LongTimeFormat = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LandingPageMenuName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_UserDashboardPreference",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        Name = c.String(),
                        DashboardId = c.Long(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Dashboard", t => t.DashboardId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DashboardId);
            
            CreateTable(
                "dbo.DI_M_UserExclusion",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        UserName = c.String(),
                        EventType = c.Byte(nullable: false),
                        EffectiveDate = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        FileName = c.String(),
                        AttachmentFile = c.Binary(),
                        ContentType = c.String(),
                        Remarks = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserGridColumnInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PagePath = c.String(),
                        GridId = c.String(),
                        UserId = c.Long(nullable: false),
                        ColumnName = c.String(),
                        OrderIndex = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserGridFilterInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PagePath = c.String(),
                        GridId = c.String(),
                        UserId = c.Long(nullable: false),
                        Operand = c.String(),
                        ColumnName = c.String(),
                        FilterValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserGridFilterSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        PagePath = c.String(),
                        GridID = c.String(),
                        Operand = c.String(),
                        ColumnName = c.String(),
                        FilterValue = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserGridInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PagePath = c.String(),
                        GridId = c.String(),
                        UserId = c.Long(nullable: false),
                        SortDirection = c.String(),
                        SortColumnName = c.String(),
                        PageSize = c.Int(nullable: false),
                        PageIndex = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserGridSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PagePath = c.String(),
                        GridID = c.String(),
                        SortColumnName = c.String(),
                        SortDirection = c.String(),
                        FilterCondition = c.String(),
                        PageSize = c.Int(nullable: false),
                        PageIndex = c.Int(nullable: false),
                        UserId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_M_UserImage",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Image = c.Binary(),
                        UserId = c.Long(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_M_UserQuickLink",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        Name = c.String(),
                        QuickLinkId = c.Long(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_QuickLink", t => t.QuickLinkId)
                .ForeignKey("dbo.DI_M_User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.QuickLinkId);
            
            CreateTable(
                "dbo.DI_M_UserSODComplianceReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        CompliancePolicyCode = c.String(),
                        CompliancePolicyName = c.String(),
                        CompliancePolicyType = c.String(),
                        ConflictName = c.String(),
                        ConflictDescription = c.String(),
                        Function1Name = c.String(),
                        Function1Description = c.String(),
                        Function2Name = c.String(),
                        Function2Description = c.String(),
                        Type = c.String(),
                        Endpoint = c.String(),
                        ConflictEntitlement1 = c.String(),
                        ConflictEntitlement2 = c.String(),
                        ConflictEntitlement1Endpoint = c.String(),
                        ConflictEntitlement2Endpoint = c.String(),
                        Risk = c.String(),
                        RiskDescription = c.String(),
                        RiskLevel = c.String(),
                        RiskScore = c.Int(nullable: false),
                        IsSensitiveRole = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        isFromAccessRequest = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DiscoveryProcessEndpointObjects",
                c => new
                    {
                        DiscoveryProcess_Id = c.Long(nullable: false),
                        EndpointObjects_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.DiscoveryProcess_Id, t.EndpointObjects_Id })
                .ForeignKey("dbo.DI_M_DiscoveryProcess", t => t.DiscoveryProcess_Id)
                .ForeignKey("dbo.DI_M_EndpointObjects", t => t.EndpointObjects_Id)
                .Index(t => t.DiscoveryProcess_Id)
                .Index(t => t.EndpointObjects_Id);
            
            CreateTable(
                "dbo.UserGroup",
                c => new
                    {
                        Group_GroupId = c.Long(nullable: false),
                        User_UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Group_GroupId, t.User_UserId })
                .ForeignKey("dbo.DI_M_Group", t => t.Group_GroupId)
                .ForeignKey("dbo.DI_M_User", t => t.User_UserId)
                .Index(t => t.Group_GroupId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Role_RoleId = c.Long(nullable: false),
                        User_UserId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleId, t.User_UserId })
                .ForeignKey("dbo.DI_M_Role", t => t.Role_RoleId)
                .ForeignKey("dbo.DI_M_User", t => t.User_UserId)
                .Index(t => t.Role_RoleId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.ADSUserInfoADSGroupInfo",
                c => new
                    {
                        ADSUserInfo_Id = c.Long(nullable: false),
                        ADSGroupInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ADSUserInfo_Id, t.ADSGroupInfo_Id })
                .ForeignKey("dbo.DI_ADS_UserInfo", t => t.ADSUserInfo_Id)
                .ForeignKey("dbo.DI_ADS_GroupInfo", t => t.ADSGroupInfo_Id)
                .Index(t => t.ADSUserInfo_Id)
                .Index(t => t.ADSGroupInfo_Id);
            
            CreateTable(
                "dbo.SODConflictFunctionsCompliancePolicy",
                c => new
                    {
                        SODConflictFunctions_Id = c.Long(nullable: false),
                        CompliancePolicy_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SODConflictFunctions_Id, t.CompliancePolicy_Id })
                .ForeignKey("dbo.DI_M_SODConflictFunctions", t => t.SODConflictFunctions_Id)
                .ForeignKey("dbo.DI_M_CompliancePolicy", t => t.CompliancePolicy_Id)
                .Index(t => t.SODConflictFunctions_Id)
                .Index(t => t.CompliancePolicy_Id);
            
            CreateTable(
                "dbo.SODFunctionSODConflictFunctions",
                c => new
                    {
                        SODFunction_Id = c.Long(nullable: false),
                        SODConflictFunctions_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SODFunction_Id, t.SODConflictFunctions_Id })
                .ForeignKey("dbo.DI_M_SODFunction", t => t.SODFunction_Id)
                .ForeignKey("dbo.DI_M_SODConflictFunctions", t => t.SODConflictFunctions_Id)
                .Index(t => t.SODFunction_Id)
                .Index(t => t.SODConflictFunctions_Id);
            
            CreateTable(
                "dbo.SODMitigationSODRiskCode",
                c => new
                    {
                        SODMitigation_Id = c.Long(nullable: false),
                        SODRiskCode_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SODMitigation_Id, t.SODRiskCode_Id })
                .ForeignKey("dbo.DI_M_SODMitigation", t => t.SODMitigation_Id)
                .ForeignKey("dbo.DI_M_SODRiskCode", t => t.SODRiskCode_Id)
                .Index(t => t.SODMitigation_Id)
                .Index(t => t.SODRiskCode_Id);
            
            CreateTable(
                "dbo.SODExceptionSODPolicyExecutionDefinition",
                c => new
                    {
                        SODException_Id = c.Long(nullable: false),
                        SODPolicyExecutionDefinition_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SODException_Id, t.SODPolicyExecutionDefinition_Id })
                .ForeignKey("dbo.DI_M_SODException", t => t.SODException_Id)
                .ForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", t => t.SODPolicyExecutionDefinition_Id)
                .Index(t => t.SODException_Id)
                .Index(t => t.SODPolicyExecutionDefinition_Id);
            
            CreateTable(
                "dbo.IACMUserInfoIACMGroupInfo",
                c => new
                    {
                        IACMUserInfo_Id = c.Long(nullable: false),
                        IACMGroupInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.IACMUserInfo_Id, t.IACMGroupInfo_Id })
                .ForeignKey("dbo.DI_IACM_UserInfo", t => t.IACMUserInfo_Id)
                .ForeignKey("dbo.DI_IACM_GroupInfo", t => t.IACMGroupInfo_Id)
                .Index(t => t.IACMUserInfo_Id)
                .Index(t => t.IACMGroupInfo_Id);
            
            CreateTable(
                "dbo.IACMRoleInfoIACMUserInfo",
                c => new
                    {
                        IACMRoleInfo_Id = c.Long(nullable: false),
                        IACMUserInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.IACMRoleInfo_Id, t.IACMUserInfo_Id })
                .ForeignKey("dbo.DI_IACM_RoleInfo", t => t.IACMRoleInfo_Id)
                .ForeignKey("dbo.DI_IACM_UserInfo", t => t.IACMUserInfo_Id)
                .Index(t => t.IACMRoleInfo_Id)
                .Index(t => t.IACMUserInfo_Id);
            
            CreateTable(
                "dbo.InhouseUserInfoInhousePermission",
                c => new
                    {
                        InhouseUserInfo_Id = c.Long(nullable: false),
                        InhousePermission_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.InhouseUserInfo_Id, t.InhousePermission_Id })
                .ForeignKey("dbo.DI_INH_UserInfo", t => t.InhouseUserInfo_Id)
                .ForeignKey("dbo.DI_INH_Permission", t => t.InhousePermission_Id)
                .Index(t => t.InhouseUserInfo_Id)
                .Index(t => t.InhousePermission_Id);
            
            CreateTable(
                "dbo.SAPSingleRoleInfoSAPCompositeRoleInfo",
                c => new
                    {
                        SAPSingleRoleInfo_Id = c.Long(nullable: false),
                        SAPCompositeRoleInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPSingleRoleInfo_Id, t.SAPCompositeRoleInfo_Id })
                .ForeignKey("dbo.DI_SAP_SINGLEROLEINFO", t => t.SAPSingleRoleInfo_Id)
                .ForeignKey("dbo.DI_SAP_COMPOSITEROLEINFO", t => t.SAPCompositeRoleInfo_Id)
                .Index(t => t.SAPSingleRoleInfo_Id)
                .Index(t => t.SAPCompositeRoleInfo_Id);
            
            CreateTable(
                "dbo.SAPTCodeInfoSAPSingleRoleInfo",
                c => new
                    {
                        SAPTCodeInfo_Id = c.Long(nullable: false),
                        SAPSingleRoleInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPTCodeInfo_Id, t.SAPSingleRoleInfo_Id })
                .ForeignKey("dbo.DI_SAP_TCODEINFO", t => t.SAPTCodeInfo_Id)
                .ForeignKey("dbo.DI_SAP_SINGLEROLEINFO", t => t.SAPSingleRoleInfo_Id)
                .Index(t => t.SAPTCodeInfo_Id)
                .Index(t => t.SAPSingleRoleInfo_Id);
            
            CreateTable(
                "dbo.SAPEPUserInfoSAPEPGroupInfo",
                c => new
                    {
                        SAPEPUserInfo_Id = c.Long(nullable: false),
                        SAPEPGroupInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPEPUserInfo_Id, t.SAPEPGroupInfo_Id })
                .ForeignKey("dbo.DI_SAPEP_UserInfo", t => t.SAPEPUserInfo_Id)
                .ForeignKey("dbo.DI_SAPEP_GroupInfo", t => t.SAPEPGroupInfo_Id)
                .Index(t => t.SAPEPUserInfo_Id)
                .Index(t => t.SAPEPGroupInfo_Id);
            
            CreateTable(
                "dbo.SAPEPRoleInfoSAPEPUserInfo",
                c => new
                    {
                        SAPEPRoleInfo_Id = c.Long(nullable: false),
                        SAPEPUserInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPEPRoleInfo_Id, t.SAPEPUserInfo_Id })
                .ForeignKey("dbo.DI_SAPEP_RoleInfo", t => t.SAPEPRoleInfo_Id)
                .ForeignKey("dbo.DI_SAPEP_UserInfo", t => t.SAPEPUserInfo_Id)
                .Index(t => t.SAPEPRoleInfo_Id)
                .Index(t => t.SAPEPUserInfo_Id);
            
            CreateTable(
                "dbo.SAPUserinfoSAPGroupInfo",
                c => new
                    {
                        SAPUserinfo_Id = c.Long(nullable: false),
                        SAPGroupInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPUserinfo_Id, t.SAPGroupInfo_Id })
                .ForeignKey("dbo.DI_SAP_UserInfo", t => t.SAPUserinfo_Id)
                .ForeignKey("dbo.DI_SAP_GROUPINFO", t => t.SAPGroupInfo_Id)
                .Index(t => t.SAPUserinfo_Id)
                .Index(t => t.SAPGroupInfo_Id);
            
            CreateTable(
                "dbo.SAPParameterInfoSAPUserinfo",
                c => new
                    {
                        SAPParameterInfo_Id = c.Long(nullable: false),
                        SAPUserinfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPParameterInfo_Id, t.SAPUserinfo_Id })
                .ForeignKey("dbo.DI_SAP_PARAMETERINFO", t => t.SAPParameterInfo_Id)
                .ForeignKey("dbo.DI_SAP_UserInfo", t => t.SAPUserinfo_Id)
                .Index(t => t.SAPParameterInfo_Id)
                .Index(t => t.SAPUserinfo_Id);
            
            CreateTable(
                "dbo.SAPProfileInfoSAPUserinfo",
                c => new
                    {
                        SAPProfileInfo_Id = c.Long(nullable: false),
                        SAPUserinfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPProfileInfo_Id, t.SAPUserinfo_Id })
                .ForeignKey("dbo.DI_SAP_PROFILEINFO", t => t.SAPProfileInfo_Id)
                .ForeignKey("dbo.DI_SAP_UserInfo", t => t.SAPUserinfo_Id)
                .Index(t => t.SAPProfileInfo_Id)
                .Index(t => t.SAPUserinfo_Id);
            
            CreateTable(
                "dbo.SAPRoleInfoSAPUserinfo",
                c => new
                    {
                        SAPRoleInfo_Id = c.Long(nullable: false),
                        SAPUserinfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPRoleInfo_Id, t.SAPUserinfo_Id })
                .ForeignKey("dbo.DI_SAP_ROLEINFO", t => t.SAPRoleInfo_Id)
                .ForeignKey("dbo.DI_SAP_UserInfo", t => t.SAPUserinfo_Id)
                .Index(t => t.SAPRoleInfo_Id)
                .Index(t => t.SAPUserinfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_UserQuickLink", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserQuickLink", "QuickLinkId", "dbo.DI_M_QuickLink");
            DropForeignKey("dbo.DI_M_UserGridSettings", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserGridInfo", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserGridFilterSettings", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserGridFilterInfo", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserGridColumnInfo", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserExclusion", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserDashboardPreference", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_UserDashboardPreference", "DashboardId", "dbo.DI_M_Dashboard");
            DropForeignKey("dbo.DI_M_UserAttachment", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_Task", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropForeignKey("dbo.DI_M_Task", "SubTaskId", "dbo.DI_M_SubTask");
            DropForeignKey("dbo.DI_M_Task", "DelegatedFrom", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_Task", "AttestationCampaignId", "dbo.DI_M_AttestationCampaign");
            DropForeignKey("dbo.DI_M_Task", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_Task", "ActionUserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_Task", "AccessRequestId", "dbo.DI_M_AccessRequest");
            DropForeignKey("dbo.DI_M_TaskDelegation", "DelegatedTo", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SqlConfiguration", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "PolicyExecutionDefinitionId", "dbo.DI_M_SODPolicyExceutionDefinition");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "FunctionItem2", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "FunctionItem1", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "FunctionId2", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "FunctionId1", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "ExceptionId", "dbo.DI_M_SODException");
            DropForeignKey("dbo.DI_M_SODPolicyReport", "ConflictId", "dbo.DI_M_SODConflictFunctions");
            DropForeignKey("dbo.DI_M_SODPolicyConfiguration", "RiskLevelId", "dbo.DI_M_SODRiskLevel");
            DropForeignKey("dbo.DI_M_SODPolicy", "PolicyTypeId", "dbo.DI_M_ComplianceType");
            DropForeignKey("dbo.DI_M_SODExecutionConfig", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_SODConflict", "RiskCodeId", "dbo.DI_M_SODRiskCode");
            DropForeignKey("dbo.DI_M_SODConflict", "FunctionId2", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODConflict", "FunctionId1", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODConflictFunctionMap", "SodFunctionId", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODRunPolicy", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropForeignKey("dbo.DI_M_SODRunPolicy", "RiskLevel_Id", "dbo.DI_M_SODRiskLevel");
            DropForeignKey("dbo.DI_M_SODReport", "SODRunPolicyId", "dbo.DI_M_SODRunPolicy");
            DropForeignKey("dbo.DI_M_SODRunPolicySelection", "SODRunPolicy_Id", "dbo.DI_M_SODRunPolicy");
            DropForeignKey("dbo.DI_M_SODRunPolicy", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.DI_M_SODReport", "FunctionItem2", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODReport", "FunctionItem1", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODReport", "FunctionId2", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODReport", "FunctionId1", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODReport", "ExceptionId", "dbo.DI_M_SODException");
            DropForeignKey("dbo.DI_M_SODReport", "ConflictId", "dbo.DI_M_SODConflictFunctions");
            DropForeignKey("dbo.SAPRoleInfoSAPUserinfo", "SAPUserinfo_Id", "dbo.DI_SAP_UserInfo");
            DropForeignKey("dbo.SAPRoleInfoSAPUserinfo", "SAPRoleInfo_Id", "dbo.DI_SAP_ROLEINFO");
            DropForeignKey("dbo.SAPProfileInfoSAPUserinfo", "SAPUserinfo_Id", "dbo.DI_SAP_UserInfo");
            DropForeignKey("dbo.SAPProfileInfoSAPUserinfo", "SAPProfileInfo_Id", "dbo.DI_SAP_PROFILEINFO");
            DropForeignKey("dbo.SAPParameterInfoSAPUserinfo", "SAPUserinfo_Id", "dbo.DI_SAP_UserInfo");
            DropForeignKey("dbo.SAPParameterInfoSAPUserinfo", "SAPParameterInfo_Id", "dbo.DI_SAP_PARAMETERINFO");
            DropForeignKey("dbo.SAPUserinfoSAPGroupInfo", "SAPGroupInfo_Id", "dbo.DI_SAP_GROUPINFO");
            DropForeignKey("dbo.SAPUserinfoSAPGroupInfo", "SAPUserinfo_Id", "dbo.DI_SAP_UserInfo");
            DropForeignKey("dbo.DI_SAP_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.SAPEPRoleInfoSAPEPUserInfo", "SAPEPUserInfo_Id", "dbo.DI_SAPEP_UserInfo");
            DropForeignKey("dbo.SAPEPRoleInfoSAPEPUserInfo", "SAPEPRoleInfo_Id", "dbo.DI_SAPEP_RoleInfo");
            DropForeignKey("dbo.SAPEPUserInfoSAPEPGroupInfo", "SAPEPGroupInfo_Id", "dbo.DI_SAPEP_GroupInfo");
            DropForeignKey("dbo.SAPEPUserInfoSAPEPGroupInfo", "SAPEPUserInfo_Id", "dbo.DI_SAPEP_UserInfo");
            DropForeignKey("dbo.DI_SAPEP_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.SAPTCodeInfoSAPSingleRoleInfo", "SAPSingleRoleInfo_Id", "dbo.DI_SAP_SINGLEROLEINFO");
            DropForeignKey("dbo.SAPTCodeInfoSAPSingleRoleInfo", "SAPTCodeInfo_Id", "dbo.DI_SAP_TCODEINFO");
            DropForeignKey("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", "SAPCompositeRoleInfo_Id", "dbo.DI_SAP_COMPOSITEROLEINFO");
            DropForeignKey("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", "SAPSingleRoleInfo_Id", "dbo.DI_SAP_SINGLEROLEINFO");
            DropForeignKey("dbo.DI_M_RequestAttachment", "UploadedBy", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_RequestAttachment", "Attestation_Id", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_RequestAttachment", "AccessRequestId", "dbo.DI_M_AccessRequest");
            DropForeignKey("dbo.DI_M_RequestApprovalConfiguration", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropForeignKey("dbo.DI_M_ReportHistory", "ReportId", "dbo.DI_M_Report");
            DropForeignKey("dbo.DI_M_ReportAttributes", "ReportId", "dbo.DI_M_Report");
            DropForeignKey("dbo.DI_M_Report", "process_Id", "dbo.DI_M_Process");
            DropForeignKey("dbo.DI_M_QuickLinkAccessRights", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_QuickLinkAccessRights", "QuickLinkId", "dbo.DI_M_QuickLink");
            DropForeignKey("dbo.DI_M_ProcessHistroy", "ProcessId", "dbo.DI_M_Process");
            DropForeignKey("dbo.DI_M_PasswordSyncConfigurationDetail", "PasswordSyncConfiguration_Id", "dbo.DI_M_PasswordSyncConfiguration");
            DropForeignKey("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId", "dbo.DI_M_PasswordPolicy");
            DropForeignKey("dbo.DI_M_PasswordPolicyEndpoint", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_NUserOrganisation", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_NUserOrganisation", "TypeId", "dbo.DI_M_NScopeFor");
            DropForeignKey("dbo.DI_M_NUserOrganisation", "OrganisationId", "dbo.DI_M_NOrganisation");
            DropForeignKey("dbo.DI_M_NUserDesignation", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_NUserDesignation", "DesignationId", "dbo.DI_M_NDesignation");
            DropForeignKey("dbo.DI_M_NRuleset", "TypeId", "dbo.DI_M_NRulesetType");
            DropForeignKey("dbo.DI_M_NRulesetForApplication", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_NProvisioningField", "EndPointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_NProvisioningField", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_Notification", "SenderId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_Notification", "ReceiverId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_NotificationAttachment", "NotificationId", "dbo.DI_M_Notification");
            DropForeignKey("dbo.DI_M_NOrganisationRelationship", "OrganisationId", "dbo.DI_M_NOrganisation");
            DropForeignKey("dbo.DI_M_NOrganisationHead", "OrganisationId", "dbo.DI_M_NOrganisation");
            DropForeignKey("dbo.DI_M_NOrganisationHead", "HeadId", "dbo.DI_M_User");
            DropForeignKey("dbo.InhouseUserInfoInhousePermission", "InhousePermission_Id", "dbo.DI_INH_Permission");
            DropForeignKey("dbo.InhouseUserInfoInhousePermission", "InhouseUserInfo_Id", "dbo.DI_INH_UserInfo");
            DropForeignKey("dbo.DI_INH_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_INH_Permission", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.IACMRoleInfoIACMUserInfo", "IACMUserInfo_Id", "dbo.DI_IACM_UserInfo");
            DropForeignKey("dbo.IACMRoleInfoIACMUserInfo", "IACMRoleInfo_Id", "dbo.DI_IACM_RoleInfo");
            DropForeignKey("dbo.DI_IACM_RoleInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.IACMUserInfoIACMGroupInfo", "IACMGroupInfo_Id", "dbo.DI_IACM_GroupInfo");
            DropForeignKey("dbo.IACMUserInfoIACMGroupInfo", "IACMUserInfo_Id", "dbo.DI_IACM_UserInfo");
            DropForeignKey("dbo.DI_IACM_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_IACM_GroupInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_GlobalAnswer", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_ReconcilationExceptionAttributeChanges", "ExceptionAttributeId", "dbo.DI_M_ReconcilationExceptionAttribute");
            DropForeignKey("dbo.DI_M_ReconcilationExceptionAttributeChanges", "ReconcilationId", "dbo.DI_M_AccountReconcilation");
            DropForeignKey("dbo.DI_M_EntitlementReconcilation", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_EntitlementReconcilation", "AccountReconcilationId", "dbo.DI_M_AccountReconcilation");
            DropForeignKey("dbo.DI_M_EndpointUserViewConfiguration", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_EndpointUserViewConfiguration", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_EndpointKeyConfiguration", "KeyId", "dbo.DI_M_EndpointConfigurationKey");
            DropForeignKey("dbo.DI_M_EndpointConfigurationDetail", "KeyId", "dbo.DI_M_EndpointConfigurationKey");
            DropForeignKey("dbo.DI_M_EndpointConfiguration", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_EndpointConfigurationDetail", "EndpointConfigurationId", "dbo.DI_M_EndpointConfiguration");
            DropForeignKey("dbo.DI_M_EmployeeEventTrigger", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_DefaultRoleQuickLink", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_DefaultRoleQuickLink", "QuickLinkId", "dbo.DI_M_QuickLink");
            DropForeignKey("dbo.DI_M_DefaultRoleDashboard", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_DefaultRoleDashboard", "DashBoardId", "dbo.DI_M_Dashboard");
            DropForeignKey("dbo.DI_M_DashboardAccessRights", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_DashboardAccessRights", "DashboardId", "dbo.DI_M_Dashboard");
            DropForeignKey("dbo.DI_M_ComplianceProcess", "PolicyExecutionDefinitionId", "dbo.DI_M_SODPolicyExceutionDefinition");
            DropForeignKey("dbo.SODExceptionSODPolicyExecutionDefinition", "SODPolicyExecutionDefinition_Id", "dbo.DI_M_SODPolicyExceutionDefinition");
            DropForeignKey("dbo.SODExceptionSODPolicyExecutionDefinition", "SODException_Id", "dbo.DI_M_SODException");
            DropForeignKey("dbo.DI_M_SODException", "RiskOwnerId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "ExceptionId", "dbo.DI_M_SODException");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "FunctionItemId2", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "FunctionItemId1", "dbo.DI_M_SODFunctionItem");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "FunctionId2", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODExceptionItem", "FunctionId1", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODEndPointPolicySelection", "SODPolicyExceutionId", "dbo.DI_M_SODPolicyExceutionDefinition");
            DropForeignKey("dbo.DI_M_SODPolicyExceutionDefinition", "CompliancePolicyId", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.DI_M_CompliancePolicy", "OwnerId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODConflictFunctions", "RiskCodeId", "dbo.DI_M_SODRiskCode");
            DropForeignKey("dbo.DI_M_SODRiskCode", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODRiskCode", "RiskScoreId", "dbo.DI_M_SODRiskScore");
            DropForeignKey("dbo.DI_M_SODRiskCode", "RiskLevelId", "dbo.DI_M_SODRiskLevel");
            DropForeignKey("dbo.SODMitigationSODRiskCode", "SODRiskCode_Id", "dbo.DI_M_SODRiskCode");
            DropForeignKey("dbo.SODMitigationSODRiskCode", "SODMitigation_Id", "dbo.DI_M_SODMitigation");
            DropForeignKey("dbo.DI_M_SODMitigation", "MonitorUserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODMitigation", "ApproverUserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODFunctionItem", "SodFunctionId", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.DI_M_SODFunctionItem", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_SODFunctionItem", "Endpointsystemid", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.SODFunctionSODConflictFunctions", "SODConflictFunctions_Id", "dbo.DI_M_SODConflictFunctions");
            DropForeignKey("dbo.SODFunctionSODConflictFunctions", "SODFunction_Id", "dbo.DI_M_SODFunction");
            DropForeignKey("dbo.SODConflictFunctionsCompliancePolicy", "CompliancePolicy_Id", "dbo.DI_M_CompliancePolicy");
            DropForeignKey("dbo.SODConflictFunctionsCompliancePolicy", "SODConflictFunctions_Id", "dbo.DI_M_SODConflictFunctions");
            DropForeignKey("dbo.LogMetadata", "AuditLogId", "dbo.AuditLog");
            DropForeignKey("dbo.AuditLogDetail", "AuditLogId", "dbo.AuditLog");
            DropForeignKey("dbo.DI_M_AuditConfigurationDetail", "TableId", "dbo.DI_M_AuditConfiguration");
            DropForeignKey("dbo.DI_M_AttestationSnapshotActionInterim", "AttestationSnapshotActionId", "dbo.DI_M_AttestationSnapshotAction");
            DropForeignKey("dbo.DI_M_AttestationSnapshotAction", "AttestationSnapshotId", "dbo.DI_M_AttestationSnapshot");
            DropForeignKey("dbo.DI_M_AttestationSnapshot", "AttestationCampaignId", "dbo.DI_M_AttestationCampaign");
            DropForeignKey("dbo.DI_M_AttestationProcessHistory", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_AttestationCampaignAttachment", "AttestationCampaignId", "dbo.DI_M_AttestationCampaign");
            DropForeignKey("dbo.DI_M_AttestationCampaign", "AttestationID", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_Attestation", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropForeignKey("dbo.DI_M_WorkflowConfigurationDetail", "WorkflowConfigurationId", "dbo.DI_M_WorkflowConfiguration");
            DropForeignKey("dbo.DI_M_WorkflowConfiguration", "RequestTypeId", "dbo.DI_M_RequestTypes");
            DropForeignKey("dbo.DI_M_WorkflowConfiguration", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_AttestationScope", "OrganisationScopeId", "dbo.DI_M_NOrganisation");
            DropForeignKey("dbo.DI_M_AttestationScope", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_AttestationProcess", "process_Id", "dbo.DI_M_Process");
            DropForeignKey("dbo.DI_M_AttestationProcess", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_AttestationHierarchy", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_M_AttestationHierarchy", "EndpointHierarchyId", "dbo.DI_M_EndpointHierarchy");
            DropForeignKey("dbo.DI_M_Attestation_Endpoint", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_Attestation_Endpoint", "AttestationId", "dbo.DI_M_Attestation");
            DropForeignKey("dbo.DI_ADS_UserInfoHistory", "UserId", "dbo.DI_ADS_UserInfo");
            DropForeignKey("dbo.DI_ADS_OuInfoHistory", "OuId", "dbo.DI_ADS_OUInfo");
            DropForeignKey("dbo.DI_ADS_NestedGroup", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_ADS_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.ADSUserInfoADSGroupInfo", "ADSGroupInfo_Id", "dbo.DI_ADS_GroupInfo");
            DropForeignKey("dbo.ADSUserInfoADSGroupInfo", "ADSUserInfo_Id", "dbo.DI_ADS_UserInfo");
            DropForeignKey("dbo.DI_M_AdminModeItem", "TabMenuId", "dbo.DI_M_TabMenu");
            DropForeignKey("dbo.DI_M_TabMenu", "PageId", "dbo.DI_M_Page");
            DropForeignKey("dbo.DI_M_AccountReconcilation", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_AccessRights", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_AccessRights", "PagePermissionId", "dbo.DI_M_PagePermission");
            DropForeignKey("dbo.DI_M_PagePermission", "PageId", "dbo.DI_M_Page");
            DropForeignKey("dbo.DI_M_Page", "PageUrlId", "dbo.DI_M_PageUrl");
            DropForeignKey("dbo.DI_M_PagePermission", "PermissionId", "dbo.DI_M_Permission");
            DropForeignKey("dbo.DI_M_PagePermission", "UrlId", "dbo.DI_M_PageUrl");
            DropForeignKey("dbo.DI_M_AccessRequestConfig", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_AccessRequest", "UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_AccessRequest", "RequesterId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_AccessRequest", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_AccessRequest", "AdHocAccessRequestId", "dbo.DI_M_AdHocAccessRequest");
            DropForeignKey("dbo.DI_M_AdHocAccessRequest", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_EndpointSystem", "OwnerUserId", "dbo.DI_M_User");
            DropForeignKey("dbo.UserRole", "User_UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.UserRole", "Role_RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_Role", "RoleOwner", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_User", "NOrganisationId", "dbo.DI_M_NOrganisation");
            DropForeignKey("dbo.DI_M_NOrganisation", "ScopeId", "dbo.DI_M_NOrganisationScope");
            DropForeignKey("dbo.DI_M_User", "NBusinessRoleId", "dbo.DI_M_NDesignation");
            DropForeignKey("dbo.UserGroup", "User_UserId", "dbo.DI_M_User");
            DropForeignKey("dbo.UserGroup", "Group_GroupId", "dbo.DI_M_Group");
            DropForeignKey("dbo.DI_M_Group", "GroupOwner", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_AttributeMapping", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_AttributeMapping", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_EndpointSystem", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_EndpointObjects", "EndPointSystemType_Id", "dbo.DI_M_EndpointSystemType");
            DropForeignKey("dbo.DI_M_DiscoveryProcess", "process_Id", "dbo.DI_M_Process");
            DropForeignKey("dbo.DI_M_Process", "Scheduler_SchedulerId", "dbo.DI_M_Scheduler");
            DropForeignKey("dbo.DiscoveryProcessEndpointObjects", "EndpointObjects_Id", "dbo.DI_M_EndpointObjects");
            DropForeignKey("dbo.DiscoveryProcessEndpointObjects", "DiscoveryProcess_Id", "dbo.DI_M_DiscoveryProcess");
            DropForeignKey("dbo.DI_M_DiscoveryProcess", "Endpointsystemid", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.SAPRoleInfoSAPUserinfo", new[] { "SAPUserinfo_Id" });
            DropIndex("dbo.SAPRoleInfoSAPUserinfo", new[] { "SAPRoleInfo_Id" });
            DropIndex("dbo.SAPProfileInfoSAPUserinfo", new[] { "SAPUserinfo_Id" });
            DropIndex("dbo.SAPProfileInfoSAPUserinfo", new[] { "SAPProfileInfo_Id" });
            DropIndex("dbo.SAPParameterInfoSAPUserinfo", new[] { "SAPUserinfo_Id" });
            DropIndex("dbo.SAPParameterInfoSAPUserinfo", new[] { "SAPParameterInfo_Id" });
            DropIndex("dbo.SAPUserinfoSAPGroupInfo", new[] { "SAPGroupInfo_Id" });
            DropIndex("dbo.SAPUserinfoSAPGroupInfo", new[] { "SAPUserinfo_Id" });
            DropIndex("dbo.SAPEPRoleInfoSAPEPUserInfo", new[] { "SAPEPUserInfo_Id" });
            DropIndex("dbo.SAPEPRoleInfoSAPEPUserInfo", new[] { "SAPEPRoleInfo_Id" });
            DropIndex("dbo.SAPEPUserInfoSAPEPGroupInfo", new[] { "SAPEPGroupInfo_Id" });
            DropIndex("dbo.SAPEPUserInfoSAPEPGroupInfo", new[] { "SAPEPUserInfo_Id" });
            DropIndex("dbo.SAPTCodeInfoSAPSingleRoleInfo", new[] { "SAPSingleRoleInfo_Id" });
            DropIndex("dbo.SAPTCodeInfoSAPSingleRoleInfo", new[] { "SAPTCodeInfo_Id" });
            DropIndex("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", new[] { "SAPCompositeRoleInfo_Id" });
            DropIndex("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", new[] { "SAPSingleRoleInfo_Id" });
            DropIndex("dbo.InhouseUserInfoInhousePermission", new[] { "InhousePermission_Id" });
            DropIndex("dbo.InhouseUserInfoInhousePermission", new[] { "InhouseUserInfo_Id" });
            DropIndex("dbo.IACMRoleInfoIACMUserInfo", new[] { "IACMUserInfo_Id" });
            DropIndex("dbo.IACMRoleInfoIACMUserInfo", new[] { "IACMRoleInfo_Id" });
            DropIndex("dbo.IACMUserInfoIACMGroupInfo", new[] { "IACMGroupInfo_Id" });
            DropIndex("dbo.IACMUserInfoIACMGroupInfo", new[] { "IACMUserInfo_Id" });
            DropIndex("dbo.SODExceptionSODPolicyExecutionDefinition", new[] { "SODPolicyExecutionDefinition_Id" });
            DropIndex("dbo.SODExceptionSODPolicyExecutionDefinition", new[] { "SODException_Id" });
            DropIndex("dbo.SODMitigationSODRiskCode", new[] { "SODRiskCode_Id" });
            DropIndex("dbo.SODMitigationSODRiskCode", new[] { "SODMitigation_Id" });
            DropIndex("dbo.SODFunctionSODConflictFunctions", new[] { "SODConflictFunctions_Id" });
            DropIndex("dbo.SODFunctionSODConflictFunctions", new[] { "SODFunction_Id" });
            DropIndex("dbo.SODConflictFunctionsCompliancePolicy", new[] { "CompliancePolicy_Id" });
            DropIndex("dbo.SODConflictFunctionsCompliancePolicy", new[] { "SODConflictFunctions_Id" });
            DropIndex("dbo.ADSUserInfoADSGroupInfo", new[] { "ADSGroupInfo_Id" });
            DropIndex("dbo.ADSUserInfoADSGroupInfo", new[] { "ADSUserInfo_Id" });
            DropIndex("dbo.UserRole", new[] { "User_UserId" });
            DropIndex("dbo.UserRole", new[] { "Role_RoleId" });
            DropIndex("dbo.UserGroup", new[] { "User_UserId" });
            DropIndex("dbo.UserGroup", new[] { "Group_GroupId" });
            DropIndex("dbo.DiscoveryProcessEndpointObjects", new[] { "EndpointObjects_Id" });
            DropIndex("dbo.DiscoveryProcessEndpointObjects", new[] { "DiscoveryProcess_Id" });
            DropIndex("dbo.DI_M_UserQuickLink", new[] { "QuickLinkId" });
            DropIndex("dbo.DI_M_UserQuickLink", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserGridSettings", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserGridInfo", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserGridFilterSettings", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserGridFilterInfo", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserGridColumnInfo", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserExclusion", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserDashboardPreference", new[] { "DashboardId" });
            DropIndex("dbo.DI_M_UserDashboardPreference", new[] { "UserId" });
            DropIndex("dbo.DI_M_UserAttachment", new[] { "UserId" });
            DropIndex("dbo.DI_M_Task", new[] { "AttestationCampaignId" });
            DropIndex("dbo.DI_M_Task", new[] { "DelegatedFrom" });
            DropIndex("dbo.DI_M_Task", new[] { "WorkflowConfigurationId" });
            DropIndex("dbo.DI_M_Task", new[] { "ActionUserId" });
            DropIndex("dbo.DI_M_Task", new[] { "SubTaskId" });
            DropIndex("dbo.DI_M_Task", new[] { "AccessRequestId" });
            DropIndex("dbo.DI_M_Task", new[] { "AttestationId" });
            DropIndex("dbo.DI_M_TaskDelegation", new[] { "DelegatedTo" });
            DropIndex("dbo.DI_M_SqlConfiguration", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "ExceptionId" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "SODRunPolicyId" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "ConflictId" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "FunctionItem2" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "FunctionId2" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "FunctionItem1" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "FunctionId1" });
            DropIndex("dbo.DI_M_SODPolicyReport", new[] { "PolicyExecutionDefinitionId" });
            DropIndex("dbo.DI_M_SODPolicyConfiguration", new[] { "RiskLevelId" });
            DropIndex("dbo.DI_M_SODPolicy", new[] { "PolicyTypeId" });
            DropIndex("dbo.DI_M_SODExecutionConfig", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_SODConflict", new[] { "RiskCodeId" });
            DropIndex("dbo.DI_M_SODConflict", new[] { "FunctionId2" });
            DropIndex("dbo.DI_M_SODConflict", new[] { "FunctionId1" });
            DropIndex("dbo.DI_M_SODConflictFunctionMap", new[] { "SodFunctionId" });
            DropIndex("dbo.DI_M_SODRunPolicySelection", new[] { "SODRunPolicy_Id" });
            DropIndex("dbo.DI_M_SODRunPolicy", new[] { "RiskLevel_Id" });
            DropIndex("dbo.DI_M_SODRunPolicy", new[] { "WorkflowConfigurationId" });
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
            DropIndex("dbo.DI_M_RequestAttachment", new[] { "Attestation_Id" });
            DropIndex("dbo.DI_M_RequestAttachment", new[] { "UploadedBy" });
            DropIndex("dbo.DI_M_RequestAttachment", new[] { "AccessRequestId" });
            DropIndex("dbo.DI_M_RequestApprovalConfiguration", new[] { "WorkflowConfigurationId" });
            DropIndex("dbo.DI_M_ReportHistory", new[] { "ReportId" });
            DropIndex("dbo.DI_M_ReportAttributes", new[] { "ReportId" });
            DropIndex("dbo.DI_M_Report", new[] { "process_Id" });
            DropIndex("dbo.DI_M_QuickLinkAccessRights", new[] { "QuickLinkId" });
            DropIndex("dbo.DI_M_QuickLinkAccessRights", new[] { "RoleId" });
            DropIndex("dbo.DI_M_ProcessHistroy", new[] { "ProcessId" });
            DropIndex("dbo.DI_M_PasswordSyncConfigurationDetail", new[] { "PasswordSyncConfiguration_Id" });
            DropIndex("dbo.DI_M_PasswordPolicyEndpoint", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_PasswordPolicyEndpoint", new[] { "PolicyId" });
            DropIndex("dbo.DI_M_NUserOrganisation", new[] { "TypeId" });
            DropIndex("dbo.DI_M_NUserOrganisation", new[] { "OrganisationId" });
            DropIndex("dbo.DI_M_NUserOrganisation", new[] { "UserId" });
            DropIndex("dbo.DI_M_NUserDesignation", new[] { "DesignationId" });
            DropIndex("dbo.DI_M_NUserDesignation", new[] { "UserId" });
            DropIndex("dbo.DI_M_NRuleset", new[] { "TypeId" });
            DropIndex("dbo.DI_M_NRulesetForApplication", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_NProvisioningField", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_M_NProvisioningField", new[] { "EndPointSystemTypeId" });
            DropIndex("dbo.DI_M_Notification", new[] { "ReceiverId" });
            DropIndex("dbo.DI_M_Notification", new[] { "SenderId" });
            DropIndex("dbo.DI_M_NotificationAttachment", new[] { "NotificationId" });
            DropIndex("dbo.DI_M_NOrganisationRelationship", new[] { "OrganisationId" });
            DropIndex("dbo.DI_M_NOrganisationHead", new[] { "HeadId" });
            DropIndex("dbo.DI_M_NOrganisationHead", new[] { "OrganisationId" });
            DropIndex("dbo.DI_INH_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_INH_Permission", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_IACM_RoleInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_IACM_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_IACM_GroupInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_M_GlobalAnswer", new[] { "UserId" });
            DropIndex("dbo.DI_M_ReconcilationExceptionAttributeChanges", new[] { "ExceptionAttributeId" });
            DropIndex("dbo.DI_M_ReconcilationExceptionAttributeChanges", new[] { "ReconcilationId" });
            DropIndex("dbo.DI_M_EntitlementReconcilation", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_EntitlementReconcilation", new[] { "AccountReconcilationId" });
            DropIndex("dbo.DI_M_EndpointUserViewConfiguration", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_EndpointUserViewConfiguration", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_EndpointKeyConfiguration", new[] { "KeyId" });
            DropIndex("dbo.DI_M_EndpointConfiguration", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_EndpointConfigurationDetail", new[] { "KeyId" });
            DropIndex("dbo.DI_M_EndpointConfigurationDetail", new[] { "EndpointConfigurationId" });
            DropIndex("dbo.DI_M_EmployeeEventTrigger", new[] { "UserId" });
            DropIndex("dbo.DI_M_DefaultRoleQuickLink", new[] { "QuickLinkId" });
            DropIndex("dbo.DI_M_DefaultRoleQuickLink", new[] { "RoleId" });
            DropIndex("dbo.DI_M_DefaultRoleDashboard", new[] { "DashBoardId" });
            DropIndex("dbo.DI_M_DefaultRoleDashboard", new[] { "RoleId" });
            DropIndex("dbo.DI_M_DashboardAccessRights", new[] { "DashboardId" });
            DropIndex("dbo.DI_M_DashboardAccessRights", new[] { "RoleId" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "FunctionItemId2" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "FunctionId2" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "FunctionItemId1" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "FunctionId1" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "UserId" });
            DropIndex("dbo.DI_M_SODExceptionItem", new[] { "ExceptionId" });
            DropIndex("dbo.DI_M_SODException", new[] { "RiskOwnerId" });
            DropIndex("dbo.DI_M_SODEndPointPolicySelection", new[] { "SODPolicyExceutionId" });
            DropIndex("dbo.DI_M_SODPolicyExceutionDefinition", new[] { "CompliancePolicyId" });
            DropIndex("dbo.DI_M_ComplianceProcess", new[] { "PolicyExecutionDefinitionId" });
            DropIndex("dbo.DI_M_SODMitigation", new[] { "MonitorUserId" });
            DropIndex("dbo.DI_M_SODMitigation", new[] { "ApproverUserId" });
            DropIndex("dbo.DI_M_SODRiskCode", new[] { "RiskScoreId" });
            DropIndex("dbo.DI_M_SODRiskCode", new[] { "UserId" });
            DropIndex("dbo.DI_M_SODRiskCode", new[] { "RiskLevelId" });
            DropIndex("dbo.DI_M_SODFunctionItem", new[] { "Endpointsystemid" });
            DropIndex("dbo.DI_M_SODFunctionItem", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_SODFunctionItem", new[] { "SodFunctionId" });
            DropIndex("dbo.DI_M_SODConflictFunctions", new[] { "RiskCodeId" });
            DropIndex("dbo.DI_M_CompliancePolicy", new[] { "OwnerId" });
            DropIndex("dbo.LogMetadata", new[] { "AuditLogId" });
            DropIndex("dbo.AuditLogDetail", new[] { "AuditLogId" });
            DropIndex("dbo.DI_M_AuditConfigurationDetail", new[] { "TableId" });
            DropIndex("dbo.DI_M_AttestationSnapshot", new[] { "AttestationCampaignId" });
            DropIndex("dbo.DI_M_AttestationSnapshotAction", new[] { "AttestationSnapshotId" });
            DropIndex("dbo.DI_M_AttestationSnapshotActionInterim", new[] { "AttestationSnapshotActionId" });
            DropIndex("dbo.DI_M_AttestationProcessHistory", new[] { "AttestationId" });
            DropIndex("dbo.DI_M_WorkflowConfigurationDetail", new[] { "WorkflowConfigurationId" });
            DropIndex("dbo.DI_M_WorkflowConfiguration", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_WorkflowConfiguration", new[] { "RequestTypeId" });
            DropIndex("dbo.DI_M_AttestationScope", new[] { "OrganisationScopeId" });
            DropIndex("dbo.DI_M_AttestationScope", new[] { "AttestationId" });
            DropIndex("dbo.DI_M_AttestationProcess", new[] { "process_Id" });
            DropIndex("dbo.DI_M_AttestationProcess", new[] { "AttestationId" });
            DropIndex("dbo.DI_M_AttestationHierarchy", new[] { "EndpointHierarchyId" });
            DropIndex("dbo.DI_M_AttestationHierarchy", new[] { "AttestationId" });
            DropIndex("dbo.DI_M_Attestation_Endpoint", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_Attestation_Endpoint", new[] { "AttestationId" });
            DropIndex("dbo.DI_M_Attestation", new[] { "WorkflowConfigurationId" });
            DropIndex("dbo.DI_M_AttestationCampaign", new[] { "AttestationID" });
            DropIndex("dbo.DI_M_AttestationCampaignAttachment", new[] { "AttestationCampaignId" });
            DropIndex("dbo.DI_ADS_UserInfoHistory", new[] { "UserId" });
            DropIndex("dbo.DI_ADS_OuInfoHistory", new[] { "OuId" });
            DropIndex("dbo.DI_ADS_NestedGroup", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_ADS_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_M_TabMenu", new[] { "PageId" });
            DropIndex("dbo.DI_M_AdminModeItem", new[] { "TabMenuId" });
            DropIndex("dbo.DI_M_AccountReconcilation", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_Page", new[] { "PageUrlId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "UrlId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "PermissionId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "PageId" });
            DropIndex("dbo.DI_M_AccessRights", new[] { "PagePermissionId" });
            DropIndex("dbo.DI_M_AccessRights", new[] { "RoleId" });
            DropIndex("dbo.DI_M_AccessRequestConfig", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_Role", new[] { "RoleOwner" });
            DropIndex("dbo.DI_M_NOrganisation", new[] { "ScopeId" });
            DropIndex("dbo.DI_M_Group", new[] { "GroupOwner" });
            DropIndex("dbo.DI_M_User", new[] { "NBusinessRoleId" });
            DropIndex("dbo.DI_M_User", new[] { "NOrganisationId" });
            DropIndex("dbo.DI_M_AttributeMapping", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_AttributeMapping", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_Process", new[] { "Scheduler_SchedulerId" });
            DropIndex("dbo.DI_M_DiscoveryProcess", new[] { "process_Id" });
            DropIndex("dbo.DI_M_DiscoveryProcess", new[] { "Endpointsystemid" });
            DropIndex("dbo.DI_M_EndpointObjects", new[] { "EndPointSystemType_Id" });
            DropIndex("dbo.DI_M_EndpointSystem", new[] { "OwnerUserId" });
            DropIndex("dbo.DI_M_EndpointSystem", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_AdHocAccessRequest", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_AccessRequest", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_AccessRequest", new[] { "AdHocAccessRequestId" });
            DropIndex("dbo.DI_M_AccessRequest", new[] { "UserId" });
            DropIndex("dbo.DI_M_AccessRequest", new[] { "RequesterId" });
            DropTable("dbo.SAPRoleInfoSAPUserinfo");
            DropTable("dbo.SAPProfileInfoSAPUserinfo");
            DropTable("dbo.SAPParameterInfoSAPUserinfo");
            DropTable("dbo.SAPUserinfoSAPGroupInfo");
            DropTable("dbo.SAPEPRoleInfoSAPEPUserInfo");
            DropTable("dbo.SAPEPUserInfoSAPEPGroupInfo");
            DropTable("dbo.SAPTCodeInfoSAPSingleRoleInfo");
            DropTable("dbo.SAPSingleRoleInfoSAPCompositeRoleInfo");
            DropTable("dbo.InhouseUserInfoInhousePermission");
            DropTable("dbo.IACMRoleInfoIACMUserInfo");
            DropTable("dbo.IACMUserInfoIACMGroupInfo");
            DropTable("dbo.SODExceptionSODPolicyExecutionDefinition");
            DropTable("dbo.SODMitigationSODRiskCode");
            DropTable("dbo.SODFunctionSODConflictFunctions");
            DropTable("dbo.SODConflictFunctionsCompliancePolicy");
            DropTable("dbo.ADSUserInfoADSGroupInfo");
            DropTable("dbo.UserRole");
            DropTable("dbo.UserGroup");
            DropTable("dbo.DiscoveryProcessEndpointObjects");
            DropTable("dbo.DI_M_UserSODComplianceReport");
            DropTable("dbo.DI_M_UserQuickLink");
            DropTable("dbo.DI_M_UserImage");
            DropTable("dbo.DI_M_UserGridSettings");
            DropTable("dbo.DI_M_UserGridInfo");
            DropTable("dbo.DI_M_UserGridFilterSettings");
            DropTable("dbo.DI_M_UserGridFilterInfo");
            DropTable("dbo.DI_M_UserGridColumnInfo");
            DropTable("dbo.DI_M_UserExclusion");
            DropTable("dbo.DI_M_UserDashboardPreference");
            DropTable("dbo.DI_M_UserCulturePreference");
            DropTable("dbo.DI_M_UserAttachment");
            DropTable("dbo.DI_Log_UI");
            DropTable("dbo.DI_M_Config_General_TrustedSource");
            DropTable("dbo.DI_M_Task");
            DropTable("dbo.DI_M_TaskDelegation");
            DropTable("dbo.DI_M_SubTask");
            DropTable("dbo.DI_M_SqlConfiguration");
            DropTable("dbo.DI_M_SODPolicyReport");
            DropTable("dbo.DI_M_SODPolicyConfiguration");
            DropTable("dbo.DI_M_SODPolicy");
            DropTable("dbo.DI_M_SODExecutionConfig");
            DropTable("dbo.DI_M_SODConflict");
            DropTable("dbo.DI_M_SODConflictFunctionMap");
            DropTable("dbo.DI_M_SODRunPolicySelection");
            DropTable("dbo.DI_M_SODRunPolicy");
            DropTable("dbo.DI_M_SODReport");
            DropTable("dbo.DI_Log_ServiceLog");
            DropTable("dbo.DI_M_ServiceAccountQuestion");
            DropTable("dbo.DI_SAP_USEROBJECTMAPPING");
            DropTable("dbo.DI_SAP_UserProfile");
            DropTable("dbo.DI_SAP_UserHistoryInfo");
            DropTable("dbo.DI_SAP_TCodeObject");
            DropTable("dbo.DI_SAP_RoleToAuthMap");
            DropTable("dbo.DI_SAP_ROLETCODEMAPPING");
            DropTable("dbo.DI_SAP_ROLEPROFILE");
            DropTable("dbo.DI_SAP_RoleMatrix");
            DropTable("dbo.DI_SAP_RoleMandatoryFields");
            DropTable("dbo.DI_SAP_Position");
            DropTable("dbo.DI_SAP_PARAMETERHISTORYINFO");
            DropTable("dbo.DI_SAP_ObjectValues");
            DropTable("dbo.DI_SAP_Objects");
            DropTable("dbo.DI_SAP_SAPHRP1001");
            DropTable("dbo.DI_SAP_HRFLINSSuppRoleMatrix");
            DropTable("dbo.DI_SAP_HRFLINSRoleMatrix");
            DropTable("dbo.DI_SAP_SAPHRFLINSConflictingRole");
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
            DropTable("dbo.DI_SAP_TCODEINFO");
            DropTable("dbo.DI_SAP_SINGLEROLEINFO");
            DropTable("dbo.DI_SAP_COMPOSITEROLEINFO");
            DropTable("dbo.DI_SAP_COMPOSITEPROFILEINFO");
            DropTable("dbo.DI_SAP_BIRoleCategories");
            DropTable("dbo.DI_SAP_AuthObjectMap");
            DropTable("dbo.DI_SAP_AuthorizationProfile");
            DropTable("dbo.DI_SAP_ActivityInfo");
            DropTable("dbo.DI_M_RoleTypes");
            DropTable("dbo.DI_M_RoleHistory");
            DropTable("dbo.DI_M_RequestAttachment");
            DropTable("dbo.DI_M_RequestApprovalConfiguration");
            DropTable("dbo.DI_M_ReportHistory");
            DropTable("dbo.DI_M_ReportAttributes");
            DropTable("dbo.DI_M_Report");
            DropTable("dbo.DI_M_QuickLinkAccessRights");
            DropTable("dbo.DI_M_PurgeConfig");
            DropTable("dbo.DI_Log_Provisioning");
            DropTable("dbo.DI_M_ProvisioningConfigMatrixSAPPosition");
            DropTable("dbo.DI_M_ProvisioningConfigMatrixSAPORG");
            DropTable("dbo.DI_M_ProvisioningConfigMatrixSAPEEGroup");
            DropTable("dbo.DI_M_ProcessHistroy");
            DropTable("dbo.DI_HRMS_PersonnelSubArea");
            DropTable("dbo.DI_HRMS_PersonnelArea");
            DropTable("dbo.DI_M_PasswordSyncConfigurationDetail");
            DropTable("dbo.DI_M_PasswordSyncConfiguration");
            DropTable("dbo.DI_M_PasswordPolicyEndpoint");
            DropTable("dbo.DI_M_PasswordPolicy");
            DropTable("dbo.DI_M_PasswordActionInfo");
            DropTable("dbo.DI_M_OUChangeSnapshot");
            DropTable("dbo.DI_M_OffboardingAccountAction");
            DropTable("dbo.DI_M_NUserOrganisation");
            DropTable("dbo.DI_M_NUserDesignation");
            DropTable("dbo.DI_M_NScopeFor");
            DropTable("dbo.DI_M_NRulesetType");
            DropTable("dbo.DI_M_NRuleset");
            DropTable("dbo.DI_M_NRulesetForApplication");
            DropTable("dbo.DI_M_NProvisioningField");
            DropTable("dbo.DI_M_Notification");
            DropTable("dbo.DI_M_NotificationAttachment");
            DropTable("dbo.DI_M_NOrganisationRelationship");
            DropTable("dbo.DI_M_NOrganisationHead");
            DropTable("dbo.DI_HRMS_Designation");
            DropTable("dbo.DI_HRMS_CostCenter");
            DropTable("dbo.DI_M_NCostCenter");
            DropTable("dbo.DI_M_MailTemplateVariable");
            DropTable("dbo.DI_M_MailTemplate");
            DropTable("dbo.DI_M_MailServerConfiguration");
            DropTable("dbo.DI_M_MailNotificationSettings");
            DropTable("dbo.DI_M_InterimStore");
            DropTable("dbo.DI_INH_UserInfo");
            DropTable("dbo.DI_INH_Permission");
            DropTable("dbo.DI_IACM_RoleInfo");
            DropTable("dbo.DI_IACM_UserInfo");
            DropTable("dbo.DI_IACM_GroupInfo");
            DropTable("dbo.DI_M_GroupHistory");
            DropTable("dbo.DI_M_GlobalQuestion");
            DropTable("dbo.DI_M_GlobalAnswer");
            DropTable("dbo.DI_M_ReconcilationExceptionAttribute");
            DropTable("dbo.DI_M_ReconcilationExceptionAttributeChanges");
            DropTable("dbo.DI_M_EntitlementReconcilation");
            DropTable("dbo.DI_M_EndpointUserViewConfiguration");
            DropTable("dbo.DI_M_EndpointKeyConfiguration");
            DropTable("dbo.DI_M_EndpointConfigurationKey");
            DropTable("dbo.DI_M_EndpointConfiguration");
            DropTable("dbo.DI_M_EndpointConfigurationDetail");
            DropTable("dbo.DI_M_EndPointAutoExpiry");
            DropTable("dbo.DI_M_EmployeeTerm");
            DropTable("dbo.DI_M_EmployeeEventTrigger");
            DropTable("dbo.DI_Log_Email");
            DropTable("dbo.DI_M_Email");
            DropTable("dbo.DI_M_EmailHistory");
            DropTable("dbo.DI_M_DiscoveryProcessHistory");
            DropTable("dbo.DI_Log_Discovery");
            DropTable("dbo.DI_M_QuickLink");
            DropTable("dbo.DI_M_DefaultRoleQuickLink");
            DropTable("dbo.DI_M_DefaultRoleDashboard");
            DropTable("dbo.DI_M_Dashboard");
            DropTable("dbo.DI_M_DashboardAccessRights");
            DropTable("dbo.DI_M_PIMConfigurationMaster");
            DropTable("dbo.DI_M_PMConfigurationMaster");
            DropTable("dbo.DI_M_IMConfigurationMaster");
            DropTable("dbo.DI_M_IACMConfigurationMaster");
            DropTable("dbo.DI_M_DGMConfigurationMaster");
            DropTable("dbo.DI_M_ComplianceConfigurationMaster");
            DropTable("dbo.DI_M_ConfigurationMasterHistory");
            DropTable("dbo.DI_M_ComplianceType");
            DropTable("dbo.DI_M_ComplianceReports");
            DropTable("dbo.DI_M_ComplianceProcessHistory");
            DropTable("dbo.DI_M_SODExceptionItem");
            DropTable("dbo.DI_M_SODException");
            DropTable("dbo.DI_M_SODEndPointPolicySelection");
            DropTable("dbo.DI_M_SODPolicyExceutionDefinition");
            DropTable("dbo.DI_M_ComplianceProcess");
            DropTable("dbo.DI_M_SODRiskScore");
            DropTable("dbo.DI_M_SODRiskLevel");
            DropTable("dbo.DI_M_SODMitigation");
            DropTable("dbo.DI_M_SODRiskCode");
            DropTable("dbo.DI_M_SODFunctionItem");
            DropTable("dbo.DI_M_SODFunction");
            DropTable("dbo.DI_M_SODConflictFunctions");
            DropTable("dbo.DI_M_CompliancePolicy");
            DropTable("dbo.DI_HRMS_Company");
            DropTable("dbo.LogMetadata");
            DropTable("dbo.AuditLogDetail");
            DropTable("dbo.AuditLog");
            DropTable("dbo.DI_M_AuditConfigurationDetail");
            DropTable("dbo.DI_M_AuditConfiguration");
            DropTable("dbo.DI_M_AttestationSnapshotTaskStatus");
            DropTable("dbo.DI_M_AttestationSnapshot");
            DropTable("dbo.DI_M_AttestationSnapshotAction");
            DropTable("dbo.DI_M_AttestationSnapshotActionInterim");
            DropTable("dbo.DI_M_AttestationSnapshotAction_History");
            DropTable("dbo.DI_M_AttestationProcessHistory");
            DropTable("dbo.DI_Log_Attestation");
            DropTable("dbo.DI_M_AttestationCampaignReport");
            DropTable("dbo.DI_M_WorkflowConfigurationDetail");
            DropTable("dbo.DI_M_RequestTypes");
            DropTable("dbo.DI_M_WorkflowConfiguration");
            DropTable("dbo.DI_M_AttestationScope");
            DropTable("dbo.DI_M_AttestationProcess");
            DropTable("dbo.DI_M_EndpointHierarchy");
            DropTable("dbo.DI_M_AttestationHierarchy");
            DropTable("dbo.DI_M_Attestation_Endpoint");
            DropTable("dbo.DI_M_Attestation");
            DropTable("dbo.DI_M_AttestationCampaign");
            DropTable("dbo.DI_M_AttestationCampaignAttachment");
            DropTable("dbo.DI_M_Attachment");
            DropTable("dbo.DI_M_XLApproverExceptions");
            DropTable("dbo.DI_M_ApproverDelegationHistory");
            DropTable("dbo.DI_M_ApprovalUserTypes");
            DropTable("dbo.DI_ADS_UserInfoHistory");
            DropTable("dbo.DI_ADS_SharedFolderInfo");
            DropTable("dbo.DI_ADS_PrinterInfo");
            DropTable("dbo.DI_ADS_OUInfo");
            DropTable("dbo.DI_ADS_OuInfoHistory");
            DropTable("dbo.DI_ADS_NestedGroup");
            DropTable("dbo.DI_ADS_UserInfo");
            DropTable("dbo.DI_ADS_GroupInfo");
            DropTable("dbo.DI_ADS_GroupInfoHistory");
            DropTable("dbo.DI_ADS_ComputerInfo");
            DropTable("dbo.DI_ADS_ComputerGroup");
            DropTable("dbo.DI_M_TabMenu");
            DropTable("dbo.DI_M_AdminModeItem");
            DropTable("dbo.DI_M_AccountReconcilation");
            DropTable("dbo.DI_M_AccountCategory");
            DropTable("dbo.DI_M_AccountActionCategory");
            DropTable("dbo.DI_M_Page");
            DropTable("dbo.DI_M_Permission");
            DropTable("dbo.DI_M_PageUrl");
            DropTable("dbo.DI_M_PagePermission");
            DropTable("dbo.DI_M_AccessRights");
            DropTable("dbo.DI_M_AccessRightHistory");
            DropTable("dbo.DI_M_AccessRequestStatus");
            DropTable("dbo.DI_M_AccessRequestSource");
            DropTable("dbo.DI_M_AccessRequestProgress");
            DropTable("dbo.DI_M_AccessRequestConfig");
            DropTable("dbo.DI_M_AccessRequestCategory");
            DropTable("dbo.DI_M_Role");
            DropTable("dbo.DI_M_NOrganisationScope");
            DropTable("dbo.DI_M_NOrganisation");
            DropTable("dbo.DI_M_NDesignation");
            DropTable("dbo.DI_M_Group");
            DropTable("dbo.DI_M_User");
            DropTable("dbo.DI_M_AttributeMapping");
            DropTable("dbo.DI_M_Scheduler");
            DropTable("dbo.DI_M_Process");
            DropTable("dbo.DI_M_DiscoveryProcess");
            DropTable("dbo.DI_M_EndpointObjects");
            DropTable("dbo.DI_M_EndpointSystemType");
            DropTable("dbo.DI_M_EndpointSystem");
            DropTable("dbo.DI_M_AdHocAccessRequest");
            DropTable("dbo.DI_M_AccessRequest");
        }
    }
}
