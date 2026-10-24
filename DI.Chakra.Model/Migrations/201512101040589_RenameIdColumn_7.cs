namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_7 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DI_ADS_Configuration", name: "EndpointSystem_EndpointSystemId", newName: "EndpointSystemId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.DI_ADS_Configuration", name: "EndpointSystemId", newName: "EndpointSystem_EndpointSystemId");
        }
    }
}
