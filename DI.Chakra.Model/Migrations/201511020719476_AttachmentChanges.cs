namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttachmentChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_RequestAttachment",
                c => new
                    {
                        AttachmentId = c.Long(nullable: false, identity: true),
                        AccessRequestId = c.Long(),
                        UploadedBy = c.Int(nullable: false),
                        WorkflowStateId = c.Byte(nullable: false),
                        Attachment = c.Binary(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AttachmentId)
                .ForeignKey("dbo.DI_M_AccessRequest", t => t.AccessRequestId)
                .ForeignKey("dbo.DI_M_User", t => t.UploadedBy, cascadeDelete: false)
                .Index(t => t.AccessRequestId)
                .Index(t => t.UploadedBy);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_RequestAttachment", "UploadedBy", "dbo.DI_M_User");
            DropForeignKey("dbo.DI_M_RequestAttachment", "AccessRequestId", "dbo.DI_M_AccessRequest");
            DropIndex("dbo.DI_M_RequestAttachment", new[] { "UploadedBy" });
            DropIndex("dbo.DI_M_RequestAttachment", new[] { "AccessRequestId" });
            DropTable("dbo.DI_M_RequestAttachment");
        }
    }
}
