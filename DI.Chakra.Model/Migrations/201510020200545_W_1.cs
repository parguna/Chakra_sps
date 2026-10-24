namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W_1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DI_M_PreferenceItem", newName: "DI_M_AdminModeItem");
            AddColumn("dbo.DI_M_AdminModeItem", "TabMenuId", c => c.Int(nullable: false));
            CreateIndex("dbo.DI_M_AdminModeItem", "TabMenuId");
            AddForeignKey("dbo.DI_M_AdminModeItem", "TabMenuId", "dbo.DI_M_TabMenu", "Id", cascadeDelete: true);
            DropColumn("dbo.DI_M_AdminModeItem", "Title");
            DropColumn("dbo.DI_M_AdminModeItem", "TargetUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AdminModeItem", "TargetUrl", c => c.String());
            AddColumn("dbo.DI_M_AdminModeItem", "Title", c => c.String());
            DropForeignKey("dbo.DI_M_AdminModeItem", "TabMenuId", "dbo.DI_M_TabMenu");
            DropIndex("dbo.DI_M_AdminModeItem", new[] { "TabMenuId" });
            DropColumn("dbo.DI_M_AdminModeItem", "TabMenuId");
            RenameTable(name: "dbo.DI_M_AdminModeItem", newName: "DI_M_PreferenceItem");
        }
    }
}
