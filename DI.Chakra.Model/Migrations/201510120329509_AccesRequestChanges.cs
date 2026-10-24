namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccesRequestChanges : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_M_AccessRequest", "RequestType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AccessRequest", "RequestType", c => c.Byte(nullable: false));
        }
    }
}
