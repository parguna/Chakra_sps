namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSAPGrcRole : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_GRCRole",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Remarks = c.String(maxLength: 250),
                        Application = c.String(maxLength: 250),
                        Module = c.String(maxLength: 250),
                        SubModule = c.String(maxLength: 250),
                        BusinessRoleName = c.String(maxLength: 250),
                        TechnicalRole = c.String(maxLength: 250),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
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
            DropTable("dbo.DI_SAP_GRCRole");
        }
    }
}
