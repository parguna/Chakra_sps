namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W_7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_SqlConfiguration", "DatabaseType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_SqlConfiguration", "DatabaseType");
        }
    }
}
