namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removepoppulateiacmandmapiacm : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_M_EndpointSystem", "PopulateIacm");
            DropColumn("dbo.DI_M_EndpointSystem", "MapIacm");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_EndpointSystem", "MapIacm", c => c.Boolean());
            AddColumn("dbo.DI_M_EndpointSystem", "PopulateIacm", c => c.Boolean());
        }
    }
}
