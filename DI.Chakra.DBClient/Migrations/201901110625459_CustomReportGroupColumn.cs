namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomReportGroupColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_CustomReport", "IsGroupByEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.DI_M_CustomReport", "GroupByColumn", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_CustomReport", "GroupByColumn");
            DropColumn("dbo.DI_M_CustomReport", "IsGroupByEnabled");
        }
    }
}
