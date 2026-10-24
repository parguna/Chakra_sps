namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renametablesod1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "SODConflictFunctionCompliancePolicy", newName: "SODConflictFunctionsCompliancePolicy");
            RenameColumn("SODConflictFunctionsCompliancePolicy", "SODConflictFunction_Id", "SODConflictFunctions_Id");
        }
        
        public override void Down()
        {
            RenameColumn("SODFunctionSODConflictFunctions", "SODConflictFunctions_Id", "SODConflictFunction_Id");
            RenameTable(name: "SODConflictFunctionsCompliancePolicy", newName: "SODConflictFunctionCompliancePolicy");
        }
    }
}
