namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_3 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_PurgeConfig");
            DropPrimaryKey("dbo.DI_M_RunTimeCodeSnippet");
            AlterColumn("dbo.DI_M_PurgeConfig", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_RunTimeCodeSnippet", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_PurgeConfig", "Id");
            AddPrimaryKey("dbo.DI_M_RunTimeCodeSnippet", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_RunTimeCodeSnippet");
            DropPrimaryKey("dbo.DI_M_PurgeConfig");
            AlterColumn("dbo.DI_M_RunTimeCodeSnippet", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_PurgeConfig", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_RunTimeCodeSnippet", "Id");
            AddPrimaryKey("dbo.DI_M_PurgeConfig", "Id");
        }
    }
}
