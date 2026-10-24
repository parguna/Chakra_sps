namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DashboardType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_Dashboard", "DashBoardType", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_Dashboard", "DashBoardType", c => c.Int(nullable: false));
        }
    }
}
