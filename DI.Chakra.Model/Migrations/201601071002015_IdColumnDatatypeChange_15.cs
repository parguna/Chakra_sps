namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_15 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_TabMenu", "ParentTabMenuId", c => c.Long());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_TabMenu", "ParentTabMenuId", c => c.Int());
        }
    }
}
