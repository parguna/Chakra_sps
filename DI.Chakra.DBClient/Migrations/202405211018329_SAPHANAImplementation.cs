namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPHANAImplementation : DbMigration
    {
        public override void Up()
        {
            
            CreateTable(
                "dbo.DI_SAPHANA_AuthorizationProfile",
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
                "dbo.DI_SAPHANA_AuthObjectMap",
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
                "dbo.DI_SAPHANA_COMPOSITEPROFILEINFO",
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
                "dbo.DI_SAPHANA_COMPOSITEROLEINFO",
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
                "dbo.DI_SAPHANA_SINGLEROLEINFO",
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
                "dbo.DI_SAPHANA_TCODEINFO",
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
                "dbo.DI_SAPHANA_ConflictingRoleReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        EndpointSystemName = c.String(),
                        AccountName = c.String(),
                        Displayname = c.String(),
                        BusinessRole = c.String(),
                        TechnicalRole = c.String(),
                        From_Position = c.String(),
                        ConflictingPosition = c.String(),
                        ConflictingRole = c.String(),
                        ConflictingTechnicalRole = c.String(),
                        SoDRemarks = c.String(),
                        AccountType = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        AccountOwnerId = c.Long(),
                        SODOwner = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAPHANA_XLCriticalTCodeMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        Application = c.String(),
                        Transaction = c.String(),
                        TransactionDescription = c.String(),
                        TransactionOwner = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPHANA_CriticalTcodeReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        EndpointSystemName = c.String(),
                        AccountName = c.String(),
                        Displayname = c.String(),
                        Transaction = c.String(),
                        Description = c.String(),
                        BusinessRole = c.String(),
                        TechnicalRole = c.String(),
                        FromPosition = c.String(),
                        AccountType = c.Byte(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        AccountOwnerId = c.Long(),
                        TransactionOwner = c.String(),
                        UserName = c.String(),
                        UserDisplayName = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAPHANA_FIELDVALUEMAPPING",
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
                "dbo.DI_SAPHANA_GROUPINFO",
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
                "dbo.DI_SAPHANA_UserInfo",
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
                        SAPUserSNC = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SAPHANA_PARAMETERINFO",
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
                "dbo.DI_SAPHANA_PROFILEINFO",
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
                "dbo.DI_SAPHANA_ROLEINFO",
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
                "dbo.DI_SAPHANA_SAPHRP1001",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        ActualObjectType = c.String(),
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
                "dbo.DI_SAPHANA_Objects",
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
                "dbo.DI_SAPHANA_ObjectValues",
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
                "dbo.DI_SAPHANA_PARAMETERHISTORYINFO",
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
                "dbo.DI_SAPHANA_Position",
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
                "dbo.DI_SAPHANA_ROLEPROFILE",
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
                "dbo.DI_SAPHANA_ROLETCODEMAPPING",
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
                "dbo.DI_SAPHANA_RoleToAuthMap",
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
                "dbo.DI_SAPHANA_RoleToTCodes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleCode = c.String(),
                        TCodes = c.String(),
                        EndPointSystemId = c.Long(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPHANA_UserProfile",
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
                "dbo.DI_SAPHANA_XLPositionsMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        Application = c.String(),
                        Remarks = c.String(),
                        PositionText = c.String(),
                        Position = c.String(),
                        Prefix_Employees = c.String(),
                        Prefix_TempStaff = c.String(),
                        User_Group = c.String(),
                        Department_Field = c.String(),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPHANA_XLRolesMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        Remarks = c.String(),
                        Application = c.String(),
                        Module = c.String(),
                        SubModule = c.String(),
                        RoleName = c.String(),
                        TechnicalRole = c.String(),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SAPHANASingleRoleInfoSAPHANACompositeRoleInfo",
                c => new
                    {
                        SAPHANASingleRoleInfo_Id = c.Long(nullable: false),
                        SAPHANACompositeRoleInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPHANASingleRoleInfo_Id, t.SAPHANACompositeRoleInfo_Id })
                .ForeignKey("dbo.DI_SAPHANA_SINGLEROLEINFO", t => t.SAPHANASingleRoleInfo_Id)
                .ForeignKey("dbo.DI_SAPHANA_COMPOSITEROLEINFO", t => t.SAPHANACompositeRoleInfo_Id)
                .Index(t => t.SAPHANASingleRoleInfo_Id)
                .Index(t => t.SAPHANACompositeRoleInfo_Id);
            
            CreateTable(
                "dbo.SAPHANATCodeInfoSAPHANASingleRoleInfo",
                c => new
                    {
                        SAPHANATCodeInfo_Id = c.Long(nullable: false),
                        SAPHANASingleRoleInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPHANATCodeInfo_Id, t.SAPHANASingleRoleInfo_Id })
                .ForeignKey("dbo.DI_SAPHANA_TCODEINFO", t => t.SAPHANATCodeInfo_Id)
                .ForeignKey("dbo.DI_SAPHANA_SINGLEROLEINFO", t => t.SAPHANASingleRoleInfo_Id)
                .Index(t => t.SAPHANATCodeInfo_Id)
                .Index(t => t.SAPHANASingleRoleInfo_Id);
            
            CreateTable(
                "dbo.SAPHANAUserinfoSAPHANAGroupInfo",
                c => new
                    {
                        SAPHANAUserinfo_Id = c.Long(nullable: false),
                        SAPHANAGroupInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPHANAUserinfo_Id, t.SAPHANAGroupInfo_Id })
                .ForeignKey("dbo.DI_SAPHANA_UserInfo", t => t.SAPHANAUserinfo_Id)
                .ForeignKey("dbo.DI_SAPHANA_GROUPINFO", t => t.SAPHANAGroupInfo_Id)
                .Index(t => t.SAPHANAUserinfo_Id)
                .Index(t => t.SAPHANAGroupInfo_Id);
            
            CreateTable(
                "dbo.SAPHANAParameterInfoSAPHANAUserinfo",
                c => new
                    {
                        SAPHANAParameterInfo_Id = c.Long(nullable: false),
                        SAPHANAUserinfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPHANAParameterInfo_Id, t.SAPHANAUserinfo_Id })
                .ForeignKey("dbo.DI_SAPHANA_PARAMETERINFO", t => t.SAPHANAParameterInfo_Id)
                .ForeignKey("dbo.DI_SAPHANA_UserInfo", t => t.SAPHANAUserinfo_Id)
                .Index(t => t.SAPHANAParameterInfo_Id)
                .Index(t => t.SAPHANAUserinfo_Id);
            
            CreateTable(
                "dbo.SAPHANAProfileInfoSAPHANAUserinfo",
                c => new
                    {
                        SAPHANAProfileInfo_Id = c.Long(nullable: false),
                        SAPHANAUserinfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPHANAProfileInfo_Id, t.SAPHANAUserinfo_Id })
                .ForeignKey("dbo.DI_SAPHANA_PROFILEINFO", t => t.SAPHANAProfileInfo_Id)
                .ForeignKey("dbo.DI_SAPHANA_UserInfo", t => t.SAPHANAUserinfo_Id)
                .Index(t => t.SAPHANAProfileInfo_Id)
                .Index(t => t.SAPHANAUserinfo_Id);
            
            CreateTable(
                "dbo.SAPHANARoleInfoSAPHANAUserinfo",
                c => new
                    {
                        SAPHANARoleInfo_Id = c.Long(nullable: false),
                        SAPHANAUserinfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SAPHANARoleInfo_Id, t.SAPHANAUserinfo_Id })
                .ForeignKey("dbo.DI_SAPHANA_ROLEINFO", t => t.SAPHANARoleInfo_Id)
                .ForeignKey("dbo.DI_SAPHANA_UserInfo", t => t.SAPHANAUserinfo_Id)
                .Index(t => t.SAPHANARoleInfo_Id)
                .Index(t => t.SAPHANAUserinfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SAPHANARoleInfoSAPHANAUserinfo", "SAPHANAUserinfo_Id", "dbo.DI_SAPHANA_UserInfo");
            DropForeignKey("dbo.SAPHANARoleInfoSAPHANAUserinfo", "SAPHANARoleInfo_Id", "dbo.DI_SAPHANA_ROLEINFO");
            DropForeignKey("dbo.SAPHANAProfileInfoSAPHANAUserinfo", "SAPHANAUserinfo_Id", "dbo.DI_SAPHANA_UserInfo");
            DropForeignKey("dbo.SAPHANAProfileInfoSAPHANAUserinfo", "SAPHANAProfileInfo_Id", "dbo.DI_SAPHANA_PROFILEINFO");
            DropForeignKey("dbo.SAPHANAParameterInfoSAPHANAUserinfo", "SAPHANAUserinfo_Id", "dbo.DI_SAPHANA_UserInfo");
            DropForeignKey("dbo.SAPHANAParameterInfoSAPHANAUserinfo", "SAPHANAParameterInfo_Id", "dbo.DI_SAPHANA_PARAMETERINFO");
            DropForeignKey("dbo.SAPHANAUserinfoSAPHANAGroupInfo", "SAPHANAGroupInfo_Id", "dbo.DI_SAPHANA_GROUPINFO");
            DropForeignKey("dbo.SAPHANAUserinfoSAPHANAGroupInfo", "SAPHANAUserinfo_Id", "dbo.DI_SAPHANA_UserInfo");
            DropForeignKey("dbo.DI_SAPHANA_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SAPHANA_CriticalTcodeReport", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SAPHANA_ConflictingRoleReport", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.SAPHANATCodeInfoSAPHANASingleRoleInfo", "SAPHANASingleRoleInfo_Id", "dbo.DI_SAPHANA_SINGLEROLEINFO");
            DropForeignKey("dbo.SAPHANATCodeInfoSAPHANASingleRoleInfo", "SAPHANATCodeInfo_Id", "dbo.DI_SAPHANA_TCODEINFO");
            DropForeignKey("dbo.SAPHANASingleRoleInfoSAPHANACompositeRoleInfo", "SAPHANACompositeRoleInfo_Id", "dbo.DI_SAPHANA_COMPOSITEROLEINFO");
            DropForeignKey("dbo.SAPHANASingleRoleInfoSAPHANACompositeRoleInfo", "SAPHANASingleRoleInfo_Id", "dbo.DI_SAPHANA_SINGLEROLEINFO");
            DropIndex("dbo.SAPHANARoleInfoSAPHANAUserinfo", new[] { "SAPHANAUserinfo_Id" });
            DropIndex("dbo.SAPHANARoleInfoSAPHANAUserinfo", new[] { "SAPHANARoleInfo_Id" });
            DropIndex("dbo.SAPHANAProfileInfoSAPHANAUserinfo", new[] { "SAPHANAUserinfo_Id" });
            DropIndex("dbo.SAPHANAProfileInfoSAPHANAUserinfo", new[] { "SAPHANAProfileInfo_Id" });
            DropIndex("dbo.SAPHANAParameterInfoSAPHANAUserinfo", new[] { "SAPHANAUserinfo_Id" });
            DropIndex("dbo.SAPHANAParameterInfoSAPHANAUserinfo", new[] { "SAPHANAParameterInfo_Id" });
            DropIndex("dbo.SAPHANAUserinfoSAPHANAGroupInfo", new[] { "SAPHANAGroupInfo_Id" });
            DropIndex("dbo.SAPHANAUserinfoSAPHANAGroupInfo", new[] { "SAPHANAUserinfo_Id" });
            DropIndex("dbo.SAPHANATCodeInfoSAPHANASingleRoleInfo", new[] { "SAPHANASingleRoleInfo_Id" });
            DropIndex("dbo.SAPHANATCodeInfoSAPHANASingleRoleInfo", new[] { "SAPHANATCodeInfo_Id" });
            DropIndex("dbo.SAPHANASingleRoleInfoSAPHANACompositeRoleInfo", new[] { "SAPHANACompositeRoleInfo_Id" });
            DropIndex("dbo.SAPHANASingleRoleInfoSAPHANACompositeRoleInfo", new[] { "SAPHANASingleRoleInfo_Id" });
            DropIndex("dbo.DI_SAPHANA_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SAPHANA_CriticalTcodeReport", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SAPHANA_ConflictingRoleReport", new[] { "EndPointSystemId" });
            DropTable("dbo.SAPHANARoleInfoSAPHANAUserinfo");
            DropTable("dbo.SAPHANAProfileInfoSAPHANAUserinfo");
            DropTable("dbo.SAPHANAParameterInfoSAPHANAUserinfo");
            DropTable("dbo.SAPHANAUserinfoSAPHANAGroupInfo");
            DropTable("dbo.SAPHANATCodeInfoSAPHANASingleRoleInfo");
            DropTable("dbo.SAPHANASingleRoleInfoSAPHANACompositeRoleInfo");
            DropTable("dbo.DI_SAPHANA_XLRolesMatrix");
            DropTable("dbo.DI_SAPHANA_XLPositionsMatrix");
            DropTable("dbo.DI_SAPHANA_UserProfile");
            DropTable("dbo.DI_SAPHANA_RoleToTCodes");
            DropTable("dbo.DI_SAPHANA_RoleToAuthMap");
            DropTable("dbo.DI_SAPHANA_ROLETCODEMAPPING");
            DropTable("dbo.DI_SAPHANA_ROLEPROFILE");
            DropTable("dbo.DI_SAPHANA_Position");
            DropTable("dbo.DI_SAPHANA_PARAMETERHISTORYINFO");
            DropTable("dbo.DI_SAPHANA_ObjectValues");
            DropTable("dbo.DI_SAPHANA_Objects");
            DropTable("dbo.DI_SAPHANA_SAPHRP1001");
            DropTable("dbo.DI_SAPHANA_ROLEINFO");
            DropTable("dbo.DI_SAPHANA_PROFILEINFO");
            DropTable("dbo.DI_SAPHANA_PARAMETERINFO");
            DropTable("dbo.DI_SAPHANA_UserInfo");
            DropTable("dbo.DI_SAPHANA_GROUPINFO");
            DropTable("dbo.DI_SAPHANA_FIELDVALUEMAPPING");
            DropTable("dbo.DI_SAPHANA_CriticalTcodeReport");
            DropTable("dbo.DI_SAPHANA_XLCriticalTCodeMatrix");
            DropTable("dbo.DI_SAPHANA_ConflictingRoleReport");
            DropTable("dbo.DI_SAPHANA_TCODEINFO");
            DropTable("dbo.DI_SAPHANA_SINGLEROLEINFO");
            DropTable("dbo.DI_SAPHANA_COMPOSITEROLEINFO");
            DropTable("dbo.DI_SAPHANA_COMPOSITEPROFILEINFO");
            DropTable("dbo.DI_SAPHANA_AuthObjectMap");
            DropTable("dbo.DI_SAPHANA_AuthorizationProfile");
        }
    }
}
