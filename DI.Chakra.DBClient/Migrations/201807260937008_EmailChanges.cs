namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailChanges : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_M_MailTemplate", "ApplicableVariableScopes");
            DropColumn("dbo.DI_M_MailTemplate", "TemplateScope");
            DropColumn("dbo.DI_M_MailTemplate", "MailTemplateName");
            DropColumn("dbo.DI_M_MailTemplate", "ActivityType");
            DropColumn("dbo.DI_M_MailTemplate", "IsActive");
            DropTable("dbo.DI_M_MailTemplateVariable");
        }
        
        public override void Down()
        {
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
            
            AddColumn("dbo.DI_M_MailTemplate", "IsActive", c => c.Boolean());
            AddColumn("dbo.DI_M_MailTemplate", "ActivityType", c => c.Int(nullable: false));
            AddColumn("dbo.DI_M_MailTemplate", "MailTemplateName", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_MailTemplate", "TemplateScope", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_MailTemplate", "ApplicableVariableScopes", c => c.String());
        }
    }
}
