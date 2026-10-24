namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_OffboardingAccountAction",
                c => new
                    {
                        OffboardingAccountActionId = c.Byte(nullable: false),
                        Desc = c.String(),
                    })
                .PrimaryKey(t => t.OffboardingAccountActionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_OffboardingAccountAction");
        }
    }
}
