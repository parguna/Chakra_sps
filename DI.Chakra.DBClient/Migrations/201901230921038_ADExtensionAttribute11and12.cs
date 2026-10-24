namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADExtensionAttribute11and12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute11", c => c.String());
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute12", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute12");
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute11");
        }
    }
}
