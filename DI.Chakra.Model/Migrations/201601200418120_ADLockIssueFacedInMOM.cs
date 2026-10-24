namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADLockIssueFacedInMOM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_ADS_UserInfo", "IsAccountLocked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_ADS_UserInfo", "IsAccountLocked");
        }
    }
}
