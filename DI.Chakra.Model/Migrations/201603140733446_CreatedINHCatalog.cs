namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedINHCatalog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_INH_Catalog",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(nullable: false),
                        Application = c.String(),
                        BusinessRole = c.String(),
                        Description = c.String(),
                        TechnicalRole = c.String(),
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
            DropTable("dbo.DI_INH_Catalog");
        }
    }
}
