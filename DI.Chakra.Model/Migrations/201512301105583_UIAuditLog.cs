namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UIAuditLog : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_Log_UI", "SessionId");
            DropColumn("dbo.DI_Log_UI", "TabName");
            DropColumn("dbo.DI_Log_UI", "MenuName");
            DropColumn("dbo.DI_Log_UI", "UIAction");
            DropColumn("dbo.DI_Log_UI", "MessageText");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_Log_UI", "MessageText", c => c.String());
            AddColumn("dbo.DI_Log_UI", "UIAction", c => c.String());
            AddColumn("dbo.DI_Log_UI", "MenuName", c => c.String());
            AddColumn("dbo.DI_Log_UI", "TabName", c => c.String());
            AddColumn("dbo.DI_Log_UI", "SessionId", c => c.String());
        }
    }
}
