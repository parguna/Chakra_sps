namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SFBNewFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_SFB_UserInfo", "WaitingAccountSync", c => c.Boolean());
            AddColumn("dbo.DI_SFB_UserInfo", "AccessRequestId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_SFB_UserInfo", "AccessRequestId");
            DropColumn("dbo.DI_SFB_UserInfo", "WaitingAccountSync");
        }
    }
}
