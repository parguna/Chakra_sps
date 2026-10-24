namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MailTemplateVariable1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_MailTemplate", "ApplicableVariableScopes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_MailTemplate", "ApplicableVariableScopes");
        }
    }
}
