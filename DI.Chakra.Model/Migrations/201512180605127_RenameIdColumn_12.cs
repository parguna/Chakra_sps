namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_12 : DbMigration
    {
        public override void Up()
        {
            //RenameColumn("dbo.UserGroup", "User_UserId", "User_Id");
            //RenameColumn("dbo.UserRole", "User_UserId", "User_Id");
            RenameColumn("dbo.DI_M_InterimStore", "InterimStoreId", "Id");
            RenameColumn("dbo.DI_M_Notification", "NotificationId", "Id");
            RenameColumn("dbo.DI_M_RequestAttachment", "AttachmentId", "Id");
            RenameColumn("dbo.DI_M_ReviewEndpointSystems", "ReviewEndpointSystemId", "Id");
            RenameColumn("dbo.DI_M_SubTask", "SubTaskId", "Id");
            RenameColumn("dbo.DI_M_TaskDelegation", "DelegationId", "Id");
            RenameColumn("dbo.DI_M_TaskReviews", "TaskReviewId", "Id");
            RenameColumn("dbo.DI_M_Task", "TaskId", "Id");
            RenameColumn("dbo.DI_M_User", "UserId", "Id");
            RenameColumn("dbo.DI_M_WorkflowConfiguration", "WorkflowConfigurationId", "Id");
            RenameColumn("dbo.DI_M_WorkflowConfigurationDetail", "WorkflowConfigurationDetailId", "Id");
            RenameColumn("dbo.DI_M_AdHocAccessRequest", "AdHocAccessRequestId", "Id");
        }
        
        public override void Down()
        {
            //RenameColumn("dbo.UserGroup", "User_Id", "User_UserId");
            //RenameColumn("dbo.UserRole", "User_Id", "User_UserId");
            RenameColumn("dbo.DI_M_InterimStore", "Id", "InterimStoreId");
            RenameColumn("dbo.DI_M_Notification", "Id", "NotificationId");
            RenameColumn("dbo.DI_M_RequestAttachment", "Id", "AttachmentId");
            RenameColumn("dbo.DI_M_ReviewEndpointSystems", "Id", "ReviewEndpointSystemId");
            RenameColumn("dbo.DI_M_SubTask", "Id", "SubTaskId");
            RenameColumn("dbo.DI_M_TaskDelegation", "Id", "DelegationId");
            RenameColumn("dbo.DI_M_TaskReviews", "Id", "TaskReviewId");
            RenameColumn("dbo.DI_M_Task", "Id", "TaskId");
            RenameColumn("dbo.DI_M_User", "Id", "UserId");
            RenameColumn("dbo.DI_M_WorkflowConfiguration", "Id", "WorkflowConfigurationId");
            RenameColumn("dbo.DI_M_WorkflowConfigurationDetail", "Id", "WorkflowConfigurationDetailId");
            RenameColumn("dbo.DI_M_AdHocAccessRequest", "Id", "AdHocAccessRequestId");
        }
    }
}
