namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminModeItemChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_AdminModeItem", "TableName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_AdminModeItem", "TableName");
        }
    }
}
