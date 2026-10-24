namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordPolicyChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_ProvisioningForm", "ProvisioningPassword_Id", "dbo.DI_M_ProvisioningPassword");
            DropForeignKey("dbo.DI_M_PasswordPolicy", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_M_PasswordPolicy", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropIndex("dbo.DI_M_ProvisioningForm", new[] { "ProvisioningPassword_Id" });
            DropIndex("dbo.DI_M_PasswordPolicy", new[] { "EndpointSystemTypeId" });
            DropIndex("dbo.DI_M_PasswordPolicy", new[] { "EndpointSystemId" });
            DropPrimaryKey("dbo.DI_M_PasswordPolicy");
            CreateTable(
                "dbo.DI_M_PasswordPolicyEndpoint",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PolicyId = c.Int(nullable: false),
                        EndpointSystemId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId, cascadeDelete: true)
                .ForeignKey("dbo.DI_M_PasswordPolicy", t => t.PolicyId, cascadeDelete: true)
                .Index(t => t.PolicyId)
                .Index(t => t.EndpointSystemId);

            DropColumn("dbo.DI_M_PasswordPolicy", "Id");
            AddColumn("dbo.DI_M_PasswordPolicy", "PolicyId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.DI_M_PasswordPolicy", "NoOfLowerCaseAlphabets", c => c.Int());
            AddColumn("dbo.DI_M_PasswordPolicy", "NoOfUpperCaseAlphabets", c => c.Int());
            AddColumn("dbo.DI_M_PasswordPolicy", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.DI_M_ProvisioningFormAD", "PasswordSource", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_ProvisioningFormAD", "PasswordExpression", c => c.String());
            AddColumn("dbo.DI_M_ProvisioningFormLINUX", "PasswordSource", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_ProvisioningFormLNS", "PasswordSource", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_ProvisioningFormLNS", "PasswordExpression", c => c.String());
            AddColumn("dbo.DI_M_ProvisioningFormORCLUM", "PasswordSource", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_ProvisioningFormOracleEBS", "PasswordSource", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_ProvisioningFormSAPEP", "PasswordSource", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_ProvisioningFormSAPEP", "PasswordExpression", c => c.String());
            AddColumn("dbo.DI_M_ProvisioningFormSAP", "PasswordSource", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_ProvisioningFormSAP", "PasswordExpression", c => c.String());
            AddPrimaryKey("dbo.DI_M_PasswordPolicy", "PolicyId");
            DropColumn("dbo.DI_M_ProvisioningForm", "ProvisioningPassword_Id");
            DropColumn("dbo.DI_M_PasswordPolicy", "EndpointSystemTypeId");
            DropColumn("dbo.DI_M_PasswordPolicy", "EndpointSystemId");
            DropColumn("dbo.DI_M_PasswordPolicy", "NoOfAlphabets");
            DropColumn("dbo.DI_M_ProvisioningFormLINUX", "PasswordLength");
            DropColumn("dbo.DI_M_ProvisioningFormLINUX", "NoOfUppercaseChar");
            DropColumn("dbo.DI_M_ProvisioningFormLINUX", "NoOfLowercaseChar");
            DropColumn("dbo.DI_M_ProvisioningFormLINUX", "NoOfDigitChar");
            DropColumn("dbo.DI_M_ProvisioningFormLINUX", "SpecialCharacter");
            DropColumn("dbo.DI_M_ProvisioningFormORCLUM", "PasswordLength");
            DropColumn("dbo.DI_M_ProvisioningFormORCLUM", "NoOfUppercaseChar");
            DropColumn("dbo.DI_M_ProvisioningFormORCLUM", "NoOfLowercaseChar");
            DropColumn("dbo.DI_M_ProvisioningFormORCLUM", "NoOfDigitChar");
            DropColumn("dbo.DI_M_ProvisioningFormORCLUM", "SpecialCharacter");
            DropColumn("dbo.DI_M_ProvisioningFormOracleEBS", "PasswordLength");
            DropColumn("dbo.DI_M_ProvisioningFormOracleEBS", "NoOfUppercaseChar");
            DropColumn("dbo.DI_M_ProvisioningFormOracleEBS", "NoOfLowercaseChar");
            DropColumn("dbo.DI_M_ProvisioningFormOracleEBS", "NoOfDigitChar");
            DropColumn("dbo.DI_M_ProvisioningFormOracleEBS", "SpecialCharacter");
            DropTable("dbo.DI_M_ProvisioningPassword");
        }
        
        public override void Down()
        {
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
            
            AddColumn("dbo.DI_M_ProvisioningFormOracleEBS", "SpecialCharacter", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormOracleEBS", "NoOfDigitChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormOracleEBS", "NoOfLowercaseChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormOracleEBS", "NoOfUppercaseChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormOracleEBS", "PasswordLength", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormORCLUM", "SpecialCharacter", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormORCLUM", "NoOfDigitChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormORCLUM", "NoOfLowercaseChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormORCLUM", "NoOfUppercaseChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormORCLUM", "PasswordLength", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLINUX", "SpecialCharacter", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLINUX", "NoOfDigitChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLINUX", "NoOfLowercaseChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLINUX", "NoOfUppercaseChar", c => c.Int());
            AddColumn("dbo.DI_M_ProvisioningFormLINUX", "PasswordLength", c => c.Int());
            AddColumn("dbo.DI_M_PasswordPolicy", "NoOfAlphabets", c => c.Int());
            AddColumn("dbo.DI_M_PasswordPolicy", "EndpointSystemId", c => c.Int(nullable: false));
            AddColumn("dbo.DI_M_PasswordPolicy", "EndpointSystemTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.DI_M_PasswordPolicy", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.DI_M_ProvisioningForm", "ProvisioningPassword_Id", c => c.Int());
            DropForeignKey("dbo.DI_M_PasswordPolicyEndpoint", "PolicyId", "dbo.DI_M_PasswordPolicy");
            DropForeignKey("dbo.DI_M_PasswordPolicyEndpoint", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_M_PasswordPolicyEndpoint", new[] { "EndpointSystemId" });
            DropIndex("dbo.DI_M_PasswordPolicyEndpoint", new[] { "PolicyId" });
            DropPrimaryKey("dbo.DI_M_PasswordPolicy");
            DropColumn("dbo.DI_M_ProvisioningFormSAP", "PasswordExpression");
            DropColumn("dbo.DI_M_ProvisioningFormSAP", "PasswordSource");
            DropColumn("dbo.DI_M_ProvisioningFormSAPEP", "PasswordExpression");
            DropColumn("dbo.DI_M_ProvisioningFormSAPEP", "PasswordSource");
            DropColumn("dbo.DI_M_ProvisioningFormOracleEBS", "PasswordSource");
            DropColumn("dbo.DI_M_ProvisioningFormORCLUM", "PasswordSource");
            DropColumn("dbo.DI_M_ProvisioningFormLNS", "PasswordExpression");
            DropColumn("dbo.DI_M_ProvisioningFormLNS", "PasswordSource");
            DropColumn("dbo.DI_M_ProvisioningFormLINUX", "PasswordSource");
            DropColumn("dbo.DI_M_ProvisioningFormAD", "PasswordExpression");
            DropColumn("dbo.DI_M_ProvisioningFormAD", "PasswordSource");
            DropColumn("dbo.DI_M_PasswordPolicy", "IsActive");
            DropColumn("dbo.DI_M_PasswordPolicy", "NoOfUpperCaseAlphabets");
            DropColumn("dbo.DI_M_PasswordPolicy", "NoOfLowerCaseAlphabets");
            DropColumn("dbo.DI_M_PasswordPolicy", "PolicyId");
            DropTable("dbo.DI_M_PasswordPolicyEndpoint");
            AddPrimaryKey("dbo.DI_M_PasswordPolicy", "Id");
            CreateIndex("dbo.DI_M_PasswordPolicy", "EndpointSystemId");
            CreateIndex("dbo.DI_M_PasswordPolicy", "EndpointSystemTypeId");
            CreateIndex("dbo.DI_M_ProvisioningForm", "ProvisioningPassword_Id");
            AddForeignKey("dbo.DI_M_PasswordPolicy", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType", "EndpointSystemTypeId", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_PasswordPolicy", "EndpointSystemId", "dbo.DI_M_EndpointSystem", "EndpointSystemId", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_ProvisioningForm", "ProvisioningPassword_Id", "dbo.DI_M_ProvisioningPassword", "Id");
        }
    }
}
