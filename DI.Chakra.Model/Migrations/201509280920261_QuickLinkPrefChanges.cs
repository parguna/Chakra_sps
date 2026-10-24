namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuickLinkPrefChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_QuickLink", "Name", c => c.String());
            AlterColumn("dbo.DI_M_DefaultRoleMenu", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_DefaultRoleMenu", "CreatedDate", c => c.DateTime());
            DropColumn("dbo.DI_M_QuickLink", "Name");
        }
    }
}
