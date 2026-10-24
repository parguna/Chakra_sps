namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W18 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_LNS_Catalog", "EndpointType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_LNS_Catalog", "EndpointType", c => c.Int(nullable: false));
        }
    }
}
