namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RulesetTableRename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Ruleset", newName: "DI_M_Ruleset");
            RenameTable(name: "dbo.RunTimeCodeSnippet", newName: "DI_M_RunTimeCodeSnippet");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DI_M_RunTimeCodeSnippet", newName: "RunTimeCodeSnippet");
            RenameTable(name: "dbo.DI_M_Ruleset", newName: "Ruleset");
        }
    }
}
