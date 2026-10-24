namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TemplateEnumAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_MailTemplate", "MailTemplateName", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_MailTemplate", "MailTemplateName");
        }
    }
}
