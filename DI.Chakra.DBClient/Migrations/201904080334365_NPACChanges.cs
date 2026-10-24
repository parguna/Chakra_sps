namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NPACChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_NPAC_UserInfo", "UserADID", c => c.String());
            AddColumn("dbo.DI_NPAC_UserInfo", "EmailId", c => c.String());
            AddColumn("dbo.DI_NPAC_UserInfo", "CompanyName", c => c.String());
            AddColumn("dbo.DI_NPAC_UserInfo", "SectionName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_NPAC_UserInfo", "SectionName");
            DropColumn("dbo.DI_NPAC_UserInfo", "CompanyName");
            DropColumn("dbo.DI_NPAC_UserInfo", "EmailId");
            DropColumn("dbo.DI_NPAC_UserInfo", "UserADID");
        }
    }
}
