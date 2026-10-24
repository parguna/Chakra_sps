namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationRelatedChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_Notification", "ReceiverId", "dbo.DI_M_User");
            DropIndex("dbo.DI_M_Notification", new[] { "ReceiverId" });
            RenameColumn(table: "dbo.DI_M_Notification", name: "ReceiverId", newName: "ReceiverUserId");
            AddColumn("dbo.DI_M_Notification", "Subject", c => c.String());
            AddColumn("dbo.DI_M_Notification", "Content", c => c.String());
            AddColumn("dbo.DI_M_Notification", "ReceiverGroupId", c => c.Int());
            AlterColumn("dbo.DI_M_Notification", "ReceiverUserId", c => c.Int());
            CreateIndex("dbo.DI_M_Notification", "ReceiverUserId");
            CreateIndex("dbo.DI_M_Notification", "ReceiverGroupId");
            AddForeignKey("dbo.DI_M_Notification", "ReceiverGroupId", "dbo.DI_M_Group", "Id");
            AddForeignKey("dbo.DI_M_Notification", "ReceiverUserId", "dbo.DI_M_User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_Notification", "ReceiverUserId", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_Notification", "ReceiverGroupId", "dbo.DI_M_Group");
            DropIndex("dbo.DI_M_Notification", new[] { "ReceiverGroupId" });
            DropIndex("dbo.DI_M_Notification", new[] { "ReceiverUserId" });
            AlterColumn("dbo.DI_M_Notification", "ReceiverUserId", c => c.Int(nullable: false));
            DropColumn("dbo.DI_M_Notification", "ReceiverGroupId");
            DropColumn("dbo.DI_M_Notification", "Content");
            DropColumn("dbo.DI_M_Notification", "Subject");
            RenameColumn(table: "dbo.DI_M_Notification", name: "ReceiverUserId", newName: "ReceiverId");
            CreateIndex("dbo.DI_M_Notification", "ReceiverId");
            AddForeignKey("dbo.DI_M_Notification", "ReceiverId", "dbo.DI_M_User", "Id", cascadeDelete: true);
        }
    }
}
