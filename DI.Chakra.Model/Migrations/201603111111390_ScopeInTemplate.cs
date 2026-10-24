namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScopeInTemplate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_MailTemplate", "TemplateScope", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_MailTemplate", "TemplateScope");
        }
    }
}
