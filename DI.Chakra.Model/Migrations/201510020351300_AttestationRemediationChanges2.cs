namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttestationRemediationChanges2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_AttestationReview", "AccessRequestId", "dbo.DI_M_AccessRequest");
            DropIndex("dbo.DI_M_AttestationReview", new[] { "AccessRequestId" });
            AddColumn("dbo.DI_M_AccessRequest", "AttestationReviewId", c => c.Long());
            CreateIndex("dbo.DI_M_AccessRequest", "AttestationReviewId");
            AddForeignKey("dbo.DI_M_AccessRequest", "AttestationReviewId", "dbo.DI_M_AttestationReview", "ReviewId");
            DropColumn("dbo.DI_M_AttestationReview", "AccessRequestId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AttestationReview", "AccessRequestId", c => c.Long());
            DropForeignKey("dbo.DI_M_AccessRequest", "AttestationReviewId", "dbo.DI_M_AttestationReview");
            DropIndex("dbo.DI_M_AccessRequest", new[] { "AttestationReviewId" });
            DropColumn("dbo.DI_M_AccessRequest", "AttestationReviewId");
            CreateIndex("dbo.DI_M_AttestationReview", "AccessRequestId");
            AddForeignKey("dbo.DI_M_AttestationReview", "AccessRequestId", "dbo.DI_M_AccessRequest", "AccessRequestId");
        }
    }
}
