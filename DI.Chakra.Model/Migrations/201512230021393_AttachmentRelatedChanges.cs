namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttachmentRelatedChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_RequestAttachment", "RequestType", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_RequestAttachment", "AttachmentNote", c => c.String());
            AddColumn("dbo.DI_M_RequestAttachment", "AttestationId", c => c.Int());
            AddColumn("dbo.DI_M_RequestAttachment", "UploadedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.DI_M_RequestAttachment", "AttestationId");
            AddForeignKey("dbo.DI_M_RequestAttachment", "AttestationId", "dbo.DI_M_Attestation", "Id");
            DropColumn("dbo.DI_M_RequestAttachment", "CreatedDate");
            DropColumn("dbo.DI_M_RequestAttachment", "CreatedBy");
            DropColumn("dbo.DI_M_RequestAttachment", "LastModifiedDate");
            DropColumn("dbo.DI_M_RequestAttachment", "LastModifiedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_RequestAttachment", "LastModifiedBy", c => c.String());
            AddColumn("dbo.DI_M_RequestAttachment", "LastModifiedDate", c => c.DateTime());
            AddColumn("dbo.DI_M_RequestAttachment", "CreatedBy", c => c.String());
            AddColumn("dbo.DI_M_RequestAttachment", "CreatedDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.DI_M_RequestAttachment", "AttestationId", "dbo.DI_M_Attestation");
            DropIndex("dbo.DI_M_RequestAttachment", new[] { "AttestationId" });
            DropColumn("dbo.DI_M_RequestAttachment", "UploadedDate");
            DropColumn("dbo.DI_M_RequestAttachment", "AttestationId");
            DropColumn("dbo.DI_M_RequestAttachment", "AttachmentNote");
            DropColumn("dbo.DI_M_RequestAttachment", "RequestType");
        }
    }
}
