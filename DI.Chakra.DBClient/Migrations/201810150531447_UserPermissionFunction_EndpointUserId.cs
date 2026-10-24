namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPermissionFunction_EndpointUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_INH_UserPermissionFunction", "EndpointUserId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_INH_UserPermissionFunction", "EndpointUserId");
        }
    }
}
