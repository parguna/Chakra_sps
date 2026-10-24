namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_GlobalQuestion");
            DropPrimaryKey("dbo.DI_M_MailServerConfiguration");
            AlterColumn("dbo.DI_M_GlobalQuestion", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_MailServerConfiguration", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_GlobalQuestion", "Id");
            AddPrimaryKey("dbo.DI_M_MailServerConfiguration", "Id");
            DropPrimaryKey("dbo.DI_M_GlobalAnswer");
            AlterColumn("dbo.DI_M_GlobalAnswer", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_GlobalAnswer", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_MailServerConfiguration");
            DropPrimaryKey("dbo.DI_M_GlobalQuestion");
            AlterColumn("dbo.DI_M_MailServerConfiguration", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_GlobalQuestion", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_MailServerConfiguration", "Id");
            AddPrimaryKey("dbo.DI_M_GlobalQuestion", "Id");
            DropPrimaryKey("dbo.DI_M_GlobalAnswer");
            AlterColumn("dbo.DI_M_GlobalAnswer", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_GlobalAnswer", "Id");
        }
    }
}
