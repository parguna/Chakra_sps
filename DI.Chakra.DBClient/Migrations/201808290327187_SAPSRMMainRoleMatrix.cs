namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPSRMMainRoleMatrix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_SRMMainRoleMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        RoleType = c.String(),
                        BusinessRole = c.String(),
                        SRMRoles = c.String(),
                        SUSRoles = c.String(),
                        SRMEnablerRule = c.String(),
                        BIRoles = c.String(),
                        BIEnablerRule = c.String(),
                        InternalPortalRoles = c.String(),
                        InternalPortalGroups = c.String(),
                        ExternalPortalRoles = c.String(),
                        ExternalPortalGroups = c.String(),
                        ADGroups = c.String(),
                        DepartmentCode = c.String(),
                        Level1Approver = c.String(),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SAP_SRMMainRoleMatrix");
        }
    }
}
