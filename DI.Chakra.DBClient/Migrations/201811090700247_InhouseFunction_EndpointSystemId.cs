namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InhouseFunction_EndpointSystemId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_INH_Functions", "EndpointSystemId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_INH_Functions", "EndpointSystemId");
        }
    }
}
