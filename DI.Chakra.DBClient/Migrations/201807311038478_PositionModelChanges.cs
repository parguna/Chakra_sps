namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PositionModelChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_SAP_SAPHRP1001", "ActualObjectType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_SAP_SAPHRP1001", "ActualObjectType");
        }
    }
}
