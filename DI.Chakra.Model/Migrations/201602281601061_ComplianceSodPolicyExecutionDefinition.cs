namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplianceSodPolicyExecutionDefinition : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.SODExceptionSODPolicyExecutionDefinition", name: "SODPolicyExceutionDefinition_Id", newName: "SODPolicyExecutionDefinition_Id");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.SODExceptionSODPolicyExecutionDefinition", name: "SODPolicyExecutionDefinition_Id", newName: "SODPolicyExceutionDefinition_Id");
        }
    }
}
