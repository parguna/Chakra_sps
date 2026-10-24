namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DB_Configuration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_SqlConfiguration", "MapAll", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_SqlConfiguration", "MapAll");
        }
    }
}
