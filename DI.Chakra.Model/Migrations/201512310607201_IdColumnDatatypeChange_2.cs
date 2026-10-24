namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_MailTemplateVariable");
            AlterColumn("dbo.DI_M_MailTemplateVariable", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_MailTemplateVariable", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_MailTemplateVariable");
            AlterColumn("dbo.DI_M_MailTemplateVariable", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_MailTemplateVariable", "Id");
        }
    }
}
