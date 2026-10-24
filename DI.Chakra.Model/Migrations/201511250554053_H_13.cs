namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_13 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DI_M_SODMitigation", "ApproverUserId");
            CreateIndex("dbo.DI_M_SODMitigation", "MonitorUserId");
            AddForeignKey("dbo.DI_M_SODMitigation", "ApproverUserId", "dbo.DI_M_User", "UserId");
            AddForeignKey("dbo.DI_M_SODMitigation", "MonitorUserId", "dbo.DI_M_User", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_SODMitigation", "MonitorUserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_SODMitigation", "ApproverUserId", "dbo.DI_M_User");
            DropIndex("dbo.DI_M_SODMitigation", new[] { "MonitorUserId" });
            DropIndex("dbo.DI_M_SODMitigation", new[] { "ApproverUserId" });
        }
    }
}
