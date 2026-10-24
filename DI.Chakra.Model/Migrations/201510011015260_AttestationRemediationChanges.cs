namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttestationRemediationChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_AttestationReview", "ReadyForRemediation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_AttestationReview", "ReadyForRemediation");
        }
    }
}
