namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkflowReminder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_TaskReminder",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TaskId = c.Long(nullable: false),
                        RequestType = c.Byte(nullable: false),
                        ReminderCount = c.Int(nullable: false),
                        IsEsclation = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_Task", t => t.TaskId)
                .Index(t => t.TaskId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_TaskReminder", "TaskId", "dbo.DI_M_Task");
            DropIndex("dbo.DI_M_TaskReminder", new[] { "TaskId" });
            DropTable("dbo.DI_M_TaskReminder");
        }
    }
}
