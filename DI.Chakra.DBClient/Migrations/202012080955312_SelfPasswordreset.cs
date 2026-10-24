namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SelfPasswordreset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_EndpointSystem", "IsUnlockAllowed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_EndpointSystem", "IsUnlockAllowed");
        }
    }
}
