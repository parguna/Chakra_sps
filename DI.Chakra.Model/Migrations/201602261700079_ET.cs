namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ET : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.DI_M_MailTemplateVariable", "VariableValuePath1", c => c.String());
            //AddColumn("dbo.DI_M_MailTemplateVariable", "VariableValuePath2", c => c.String());
            //DropColumn("dbo.DI_M_MailTemplateVariable", "VariableValuePath");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.DI_M_MailTemplateVariable", "VariableValuePath", c => c.String());
            //DropColumn("dbo.DI_M_MailTemplateVariable", "VariableValuePath2");
            //DropColumn("dbo.DI_M_MailTemplateVariable", "VariableValuePath1");
        }
    }
}
