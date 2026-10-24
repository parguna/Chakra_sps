namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EndpointChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_EndpointSystem", "SupportSecondaryAccount", c => c.Boolean(nullable: false, defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_EndpointSystem", "SupportSecondaryAccount");
        }
    }
}
