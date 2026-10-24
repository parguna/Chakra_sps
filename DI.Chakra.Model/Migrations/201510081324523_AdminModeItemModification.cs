namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminModeItemModification : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_M_AdminModeItem", "Number");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AdminModeItem", "Number", c => c.String());
        }
    }
}
