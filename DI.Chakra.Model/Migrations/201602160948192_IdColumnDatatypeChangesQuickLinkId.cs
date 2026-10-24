namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChangesQuickLinkId : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DI_M_QuickLink] DROP CONSTRAINT DF__DI_M_Quic__Quick__7993056A");
            AlterColumn("dbo.DI_M_QuickLink", "QuickLinkId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_QuickLink", "QuickLinkId", c => c.Int(nullable: false));
        }
    }
}
