namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDiscoveryServiceInstanceType : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.DI_M_Process", "DiscoveryInstance", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_Process", "DiscoveryInstance");
        }
    }
}
