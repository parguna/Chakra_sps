namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SMSRoleCatalogue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SMS_XLRoleCatalogue",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Building = c.String(),
                        Section = c.String(),
                        RoleName = c.String(),
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
            DropTable("dbo.DI_SMS_XLRoleCatalogue");
        }
    }
}
