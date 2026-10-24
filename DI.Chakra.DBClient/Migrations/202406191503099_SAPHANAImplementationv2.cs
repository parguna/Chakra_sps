namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPHANAImplementationv2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAPHANA_GROUPHISTORYINFO",
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
                "dbo.DI_SAPHANA_SectionMappingInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        HRMSSectionCode = c.String(),
                        SAPSectionCode = c.String(),
                        VBUKR = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_SAPHANA_TCodeObject",
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
                "dbo.DI_SAPHANA_UserHistoryInfo",
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
            
          
        }
        
        public override void Down()
        {

            DropTable("dbo.DI_SAPHANA_UserHistoryInfo");
            DropTable("dbo.DI_SAPHANA_TCodeObject");
            DropTable("dbo.DI_SAPHANA_SectionMappingInfo");
            DropTable("dbo.DI_SAPHANA_GROUPHISTORYINFO");
        }
    }
}
