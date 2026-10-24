namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RunTimeCodeSnippet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RunTimeCodeSnippet",
                c => new
                    {
                        CodeSnippetId = c.Int(nullable: false, identity: true),
                        CodeSnippetName = c.String(),
                        CodeSnippet = c.Binary(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CodeSnippetId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RunTimeCodeSnippet");
        }
    }
}
