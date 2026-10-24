namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "SODFunctionSODConflictFunction", newName: "SODFunctionSODConflictFunctions");
            RenameColumn("SODFunctionSODConflictFunctions", "SODConflictFunction_Id", "SODConflictFunctions_Id");
        }
        
        public override void Down()
        {
            RenameColumn("SODFunctionSODConflictFunctions", "SODConflictFunctions_Id", "SODConflictFunction_Id");
            RenameTable(name: "SODFunctionSODConflictFunctions", newName: "SODFunctionSODConflictFunction");
        }
    }
}
