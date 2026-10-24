namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YP_TaskIdChange : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.DI_M_ReviewEndpointSystems", "TaskId");
            AddForeignKey("dbo.DI_M_ReviewEndpointSystems", "TaskId", "dbo.DI_M_Task", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_ReviewEndpointSystems", "TaskId", "dbo.DI_M_Task");
            DropIndex("dbo.DI_M_ReviewEndpointSystems", new[] { "TaskId" });
        }
    }
}
