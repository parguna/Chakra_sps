namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SkypeForBusinessProvisioningMatrix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_SFB_LineUrIExclusion", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_SFB_LineUrIExclusion", new[] { "EndPointSystemId" });
            CreateTable(
                "dbo.DI_SFB_ProvisioningMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        RequestType = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        ADID = c.String(),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DI_SFB_UnAssignedNumberMatrix", "Reserved", c => c.Boolean(nullable: false));
            DropTable("dbo.DI_SFB_LineUrIExclusion");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DI_SFB_LineUrIExclusion",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Assigned = c.Boolean(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndPointSystemId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.DI_SFB_UnAssignedNumberMatrix", "Reserved");
            DropTable("dbo.DI_SFB_ProvisioningMatrix");
            CreateIndex("dbo.DI_SFB_LineUrIExclusion", "EndPointSystemId");
            AddForeignKey("dbo.DI_SFB_LineUrIExclusion", "EndPointSystemId", "dbo.DI_M_EndpointSystem", "Id");
        }
    }
}
