namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChangeRuleset : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_Ruleset");
            AlterColumn("dbo.DI_M_Ruleset", "ID", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.DI_M_Ruleset", "ID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_Ruleset");
            AlterColumn("dbo.DI_M_Ruleset", "ID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.DI_M_Ruleset", "ID");
        }
    }
}
