namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class O365Model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_O365_GroupInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        displayName = c.String(),
                        description = c.String(),
                        member = c.String(),
                        distinguishedName = c.String(),
                        instanceType = c.String(),
                        whenCreated = c.DateTime(),
                        whenChanged = c.DateTime(),
                        name = c.String(),
                        objectSid = c.String(),
                        sAMAccountName = c.String(),
                        sAMAccountType = c.String(),
                        groupType = c.String(),
                        groupTypeDesc = c.String(),
                        objectCategory = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_O365_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        City = c.String(),
                        Company = c.String(),
                        ConsumerNetID = c.String(),
                        CountryOrRegion = c.String(),
                        Department = c.String(),
                        DisplayName = c.String(),
                        DistinguishedName = c.String(),
                        ExchangeVersion = c.String(),
                        Fax = c.String(),
                        FirstName = c.String(),
                        HomePhone = c.String(),
                        Id1 = c.String(),
                        Identity = c.String(),
                        IsLinked = c.Boolean(),
                        IsSecurityPrincipal = c.Boolean(),
                        IsValid = c.Boolean(),
                        LastName = c.String(),
                        LinkedMasterAccount = c.String(),
                        Manager = c.String(),
                        MicrosoftOnlineServicesID = c.String(),
                        MobilePhone = c.String(),
                        Name = c.String(),
                        NetID = c.String(),
                        Office = c.String(),
                        OrganizationalUnit = c.String(),
                        OrganizationId = c.String(),
                        Phone = c.String(),
                        SamAccountName = c.String(),
                        Sid = c.String(),
                        SimpleDisplayName = c.String(),
                        SKUAssigned = c.String(),
                        Title = c.String(),
                        UserAccountControl = c.String(),
                        UserPrincipalName = c.String(),
                        WhenCreated = c.DateTime(),
                        WindowsEmailAddress = c.String(),
                        WindowsLiveID = c.String(),
                        PwdResetCnt = c.Int(nullable: false),
                        mail = c.String(),
                        MappingType = c.String(),
                        DIUserId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IACMLastPasswordChange = c.DateTime(),
                        IfUnmappedMapTo = c.String(),
                        Version = c.Int(nullable: false),
                        MailboxUsage = c.String(),
                        LastLogon = c.DateTime(),
                        ValidationStatus = c.String(),
                        LicenseReconciliationNeeded = c.Boolean(),
                        UsageLocation = c.String(),
                        Alias = c.String(),
                        RetentionPolicy = c.String(),
                        LitigationHoldEnabled = c.Boolean(),
                        RetentionHoldEnabled = c.Boolean(),
                        ForwardingSmtpAddress = c.String(),
                        LitigationHoldDuration = c.String(),
                        IsMailboxEnabled = c.Boolean(nullable: false),
                        Languages = c.String(),
                        IsShared = c.Boolean(nullable: false),
                        RoleAssignmentPolicy = c.String(),
                        SharingPolicy = c.String(),
                        ArchiveName = c.String(),
                        ArchiveStatus = c.String(),
                        RecipientType = c.String(),
                        SendAs = c.String(),
                        SendonBehalf = c.String(),
                        FullAccess = c.String(),
                        AccountSkuId = c.String(),
                        ServicePlanInfo = c.String(),
                        IsSkypeOnPrem = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.O365UserInfoO365GroupInfo",
                c => new
                    {
                        O365UserInfo_Id = c.Long(nullable: false),
                        O365GroupInfo_Id = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: true),
                        Version = c.Int(nullable: true),
                })
                .PrimaryKey(t => new { t.O365UserInfo_Id, t.O365GroupInfo_Id })
                .ForeignKey("dbo.DI_O365_UserInfo", t => t.O365UserInfo_Id)
                .ForeignKey("dbo.DI_O365_GroupInfo", t => t.O365GroupInfo_Id)
                .Index(t => t.O365UserInfo_Id)
                .Index(t => t.O365GroupInfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.O365UserInfoO365GroupInfo", "O365GroupInfo_Id", "dbo.DI_O365_GroupInfo");
            DropForeignKey("dbo.O365UserInfoO365GroupInfo", "O365UserInfo_Id", "dbo.DI_O365_UserInfo");
            DropForeignKey("dbo.DI_O365_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.O365UserInfoO365GroupInfo", new[] { "O365GroupInfo_Id" });
            DropIndex("dbo.O365UserInfoO365GroupInfo", new[] { "O365UserInfo_Id" });
            DropIndex("dbo.DI_O365_UserInfo", new[] { "EndPointSystemId" });
            DropTable("dbo.O365UserInfoO365GroupInfo");
            DropTable("dbo.DI_O365_UserInfo");
            DropTable("dbo.DI_O365_GroupInfo");
        }
    }
}
