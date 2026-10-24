namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_Notification",
                c => new
                    {
                        NotificationId = c.Long(nullable: false, identity: true),
                        TaskId = c.Long(),
                        ParentId = c.Long(),
                        SenderId = c.Int(nullable: false),
                        ReceiverId = c.Int(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        IsReplied = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.NotificationId)
                .ForeignKey("dbo.DI_M_User", t => t.ReceiverId, cascadeDelete: false)
                .ForeignKey("dbo.DI_M_User", t => t.SenderId, cascadeDelete: false)
                .ForeignKey("dbo.DI_M_Task", t => t.TaskId)
                .Index(t => t.TaskId)
                .Index(t => t.SenderId)
                .Index(t => t.ReceiverId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_Notification", "TaskId", "dbo.DI_M_Task");
            DropForeignKey("dbo.DI_M_Notification", "SenderId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_Notification", "ReceiverId", "dbo.DI_M_User");
            DropIndex("dbo.DI_M_Notification", new[] { "ReceiverId" });
            DropIndex("dbo.DI_M_Notification", new[] { "SenderId" });
            DropIndex("dbo.DI_M_Notification", new[] { "TaskId" });
            DropTable("dbo.DI_M_Notification");
        }
    }
}
