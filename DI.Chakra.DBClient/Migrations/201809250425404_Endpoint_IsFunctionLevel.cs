namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Endpoint_IsFunctionLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_EndpointSystem", "IsFunctionLevel", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_EndpointSystem", "IsFunctionLevel");
        }
    }
}
