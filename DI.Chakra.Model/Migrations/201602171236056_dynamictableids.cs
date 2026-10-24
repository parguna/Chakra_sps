namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dynamictableids : DbMigration
    {
        public override void Up()
        {

            RenameColumn(table: "dbo.DiscoveryProcessEndpointObjects", name: "DiscoveryProcess_DiscoveryProcessId", newName: "DiscoveryProcess_Id");
            RenameColumn(table: "dbo.DiscoveryProcessEndpointObjects", name: "EndpointObjects_EndpointObjectId", newName: "EndpointObjects_Id");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.DiscoveryProcessEndpointObjects", name: "DiscoveryProcess_Id", newName: "DiscoveryProcess_DiscoveryProcessId");
            RenameColumn(table: "dbo.DiscoveryProcessEndpointObjects", name: "EndpointObjects_Id", newName: "EndpointObjects_EndpointObjectId");
        }
    }
}
