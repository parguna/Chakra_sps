namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADExtension13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute13", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute13");
        }
    }
}
