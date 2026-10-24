namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_14 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_RunTimeCodeSnippet", "CodeSnippetId", "Id");

            RenameColumn("dbo.DI_M_Process", "ProcessId", "Id");
            RenameColumn("dbo.DI_M_ProcessHistroy", "HistroyID", "Id");
            RenameColumn("dbo.DI_M_Scheduler", "SchedulerId", "Id");

            RenameColumn("dbo.DI_M_GlobalQuestion", "GlobalQuestionId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_RunTimeCodeSnippet", "Id", "CodeSnippetId");

            RenameColumn("dbo.DI_M_Process", "Id", "ProcessId");
            RenameColumn("dbo.DI_M_ProcessHistroy", "Id", "HistroyID");
            RenameColumn("dbo.DI_M_Scheduler", "Id", "SchedulerId");

            RenameColumn("dbo.DI_M_GlobalQuestion", "Id", "GlobalQuestionId");
        }
    }
}
