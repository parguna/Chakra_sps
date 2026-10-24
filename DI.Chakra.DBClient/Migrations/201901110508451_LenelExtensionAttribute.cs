namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LenelExtensionAttribute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_SMS_UserInfo", "Extension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_SMS_UserInfo", "Extension");
        }
    }
}
