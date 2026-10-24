namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserImageChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_EntitlementReconcilation", "EndpointType", c => c.Int(nullable: false));
            AddColumn("dbo.DI_M_UserImage", "UserId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_UserImage", "UserId");
            DropColumn("dbo.DI_M_EntitlementReconcilation", "EndpointType");
        }
    }
}

