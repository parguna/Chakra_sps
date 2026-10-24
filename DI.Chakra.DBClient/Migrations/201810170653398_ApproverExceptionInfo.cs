namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApproverExceptionInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_XLApproverExceptionInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Person = c.String(),
                        Approver = c.String(),
                        ApprovalLevel = c.Int(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedDate = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DI_INH_UserPermissionFunction", "PermissionId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_INH_UserPermissionFunction", "PermissionId");
            DropTable("dbo.DI_M_XLApproverExceptionInfo");
        }
    }
}
