namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedADAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute6", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute7", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute8", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute9", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute10", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute10");
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute9");
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute8");
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute7");
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute6");
        }
    }
}
