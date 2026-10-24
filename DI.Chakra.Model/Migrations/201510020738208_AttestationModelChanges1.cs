namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttestationModelChanges1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", "AccessType", c => c.Int(nullable: false));
            DropColumn("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", "BusinessOwner");
            DropColumn("dbo.DI_M_AttestationReview_EndpointSystem_L3Detail", "BusinessOwner");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AttestationReview_EndpointSystem_L3Detail", "BusinessOwner", c => c.String());
            AddColumn("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", "BusinessOwner", c => c.String());
            AlterColumn("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", "AccessType", c => c.String());
        }
    }
}
