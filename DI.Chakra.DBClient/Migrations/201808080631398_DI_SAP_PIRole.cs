namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DI_SAP_PIRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_PIRole",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Remarks = c.String(),
                        Application = c.String(),
                        Module = c.String(),
                        SubModule = c.String(),
                        BusinessRoleName = c.String(),
                        TechnicalRole = c.String(),
                        Level1Approver = c.String(),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(),
                        Level3ApproverType = c.Byte(nullable: false),
                        IsModified = c.Boolean(),
                        IsDeleted = c.Boolean(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SAP_PIRole");
        }
    }
}
