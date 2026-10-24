namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_8 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DI_M_AttributeMapping", name: "EndPoint_EndpointSystemId", newName: "EndpointSystemId");
            RenameColumn(table: "dbo.DI_M_AttributeMapping", name: "EndPointSystemType_EndpointSystemTypeId", newName: "EndpointSystemTypeId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.DI_M_AttributeMapping", name: "EndpointSystemId", newName: "EndPoint_EndpointSystemId");
            RenameColumn(table: "dbo.DI_M_AttributeMapping", name: "EndpointSystemTypeId", newName: "EndPointSystemType_EndpointSystemTypeId");
        }
    }
}
