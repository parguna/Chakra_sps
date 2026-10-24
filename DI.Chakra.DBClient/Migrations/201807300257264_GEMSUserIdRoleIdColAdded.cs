namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GEMSUserIdRoleIdColAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_GEMS_RoleInfo", "RoleId", c => c.Int(nullable: false));
            AddColumn("dbo.DI_GEMS_UserInfo", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_GEMS_UserInfo", "UserId");
            DropColumn("dbo.DI_GEMS_RoleInfo", "RoleId");
        }
    }
}
