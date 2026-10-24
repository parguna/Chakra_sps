namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Oracle11gMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_ProvisioningFormOracle11g",
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_ProvisioningFormOracle11g", "ProvisioningFormId", "dbo.DI_M_ProvisioningForm");
            DropIndex("dbo.DI_M_ProvisioningFormOracle11g", new[] { "ProvisioningFormId" });
            DropTable("dbo.DI_M_ProvisioningFormOracle11g");
        }
    }
}
