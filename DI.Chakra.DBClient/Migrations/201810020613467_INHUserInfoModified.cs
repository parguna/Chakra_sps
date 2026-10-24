namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INHUserInfoModified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_INH_UserInfo", "LockScenario", c => c.String());
            AddColumn("dbo.DI_INH_UserInfo", "AccountType", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_INH_UserInfo", "AccountType");
            DropColumn("dbo.DI_INH_UserInfo", "LockScenario");
        }
    }
}
