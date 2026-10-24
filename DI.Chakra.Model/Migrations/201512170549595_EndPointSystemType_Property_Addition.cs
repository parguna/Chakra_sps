namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EndPointSystemType_Property_Addition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_EndpointSystemType", "ObjectName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_EndpointSystemType", "ObjectName");
        }
    }
}
