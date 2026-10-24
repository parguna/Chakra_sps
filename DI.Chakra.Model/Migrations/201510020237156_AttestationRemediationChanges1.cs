namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttestationRemediationChanges1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_AttestationReview", "RemediationStatus", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_AttestationReview", "AccessRequestId", c => c.Long());
            CreateIndex("dbo.DI_M_AttestationReview", "AccessRequestId");
            AddForeignKey("dbo.DI_M_AttestationReview", "AccessRequestId", "dbo.DI_M_AccessRequest", "AccessRequestId");
            DropColumn("dbo.DI_M_AttestationReview", "ReadyForRemediation");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AttestationReview", "ReadyForRemediation", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.DI_M_AttestationReview", "AccessRequestId", "dbo.DI_M_AccessRequest");
            DropIndex("dbo.DI_M_AttestationReview", new[] { "AccessRequestId" });
            DropColumn("dbo.DI_M_AttestationReview", "AccessRequestId");
            DropColumn("dbo.DI_M_AttestationReview", "RemediationStatus");
        }
    }
}
