namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountTypeChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_Unix_UserInfoHistory", "AccountType", c => c.Byte(nullable: false));
            DropColumn("dbo.DI_Unix_UserInfo", "SpecialAccount");
            DropColumn("dbo.DI_Unix_UserInfo", "IsUserAccount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_Unix_UserInfo", "IsUserAccount", c => c.Boolean(nullable: false));
            AddColumn("dbo.DI_Unix_UserInfo", "SpecialAccount", c => c.Boolean(nullable: false));
            DropColumn("dbo.DI_Unix_UserInfoHistory", "AccountType");
        }
    }
}
