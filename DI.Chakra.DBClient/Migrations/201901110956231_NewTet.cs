namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTet : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute1", c => c.String());
            //AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute2", c => c.String());
            //AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute3", c => c.String());
           // AddColumn("dbo.DI_ADS_UserInfo", "extensionAttribute5", c => c.String());
            //AddColumn("dbo.DI_SMS_UserInfo", "Extension", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.DI_SMS_UserInfo", "Extension");
            //DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute5");
            //DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute3");
            //DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute2");
            //DropColumn("dbo.DI_ADS_UserInfo", "extensionAttribute1");
        }
    }
}
