namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class linux : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_Unix_UserInfo", "DisplayName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_Unix_UserInfo", "DisplayName");
        }
    }
}
