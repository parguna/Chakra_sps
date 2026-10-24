namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameDynamicTableSOD : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "SODPolicyExceutionDefinitionSODException", newName: "SODExceptionSODPolicyExecutionDefinition");
        }
        
        public override void Down()
        {
            RenameTable(name: "SODExceptionSODPolicyExecutionDefinition", newName: "SODPolicyExceutionDefinitionSODException");
        }
    }
}
