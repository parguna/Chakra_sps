namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPSSOImplementation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_SAP_UserInfo", "SAPUserSNC", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_SAP_UserInfo", "SAPUserSNC");
        }
    }
}
