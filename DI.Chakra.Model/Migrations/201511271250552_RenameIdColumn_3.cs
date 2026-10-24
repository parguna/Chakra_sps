namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_3 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_Attestation", "AttestationId", "Id");
            RenameColumn("dbo.DI_M_Attestation_Endpoint", "AttestationEndpointId", "Id");
            RenameColumn("dbo.DI_M_AttestationProcess", "AttestationProcessId", "Id");
            RenameColumn("dbo.DI_M_AttestationProcessHistory", "AttestationProcessId", "Id");
            RenameColumn("dbo.DI_M_AttestationReview", "ReviewId", "Id");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem", "ReviewEndpointId", "Id");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystemHistory", "ReviewEndpointHistoryId", "Id");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", "L2DetailId", "Id");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem_L2DetailHistory", "L2DetailHistoryId", "Id");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem_L3Detail", "L3DetailId", "Id");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem_L3DetailHistory", "L3DetailHistoryId", "Id");

            RenameColumn("dbo.DI_M_Group", "GroupId", "Id");
            RenameColumn("dbo.DI_M_GroupHistory", "GroupHistoryId", "Id");

            RenameColumn("dbo.DI_M_ComplianceProcess", "ComplianceProcessId", "Id");
            RenameColumn("dbo.DI_M_ComplianceProcessHistory", "ComplianceProcessHistoryId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_Attestation", "Id", "AttestationId");
            RenameColumn("dbo.DI_M_Attestation_Endpoint", "Id", "AttestationEndpointId");
            RenameColumn("dbo.DI_M_AttestationProcess", "Id", "AttestationProcessId");
            RenameColumn("dbo.DI_M_AttestationProcessHistory", "Id", "AttestationProcessId");
            RenameColumn("dbo.DI_M_AttestationReview", "Id", "ReviewId");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem", "Id", "ReviewEndpointId");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystemHistory", "Id", "ReviewEndpointHistoryId");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem_L2Detail", "Id", "L2DetailId");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem_L2DetailHistory", "Id", "L2DetailHistoryId");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem_L3Detail", "Id", "L3DetailId");
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem_L3DetailHistory", "Id", "L3DetailHistoryId");

            RenameColumn("dbo.DI_M_Group", "Id", "GroupId");
            RenameColumn("dbo.DI_M_GroupHistory", "Id", "GroupHistoryId");

            RenameColumn("dbo.DI_M_ComplianceProcess", "Id", "ComplianceProcessId");
            RenameColumn("dbo.DI_M_ComplianceProcessHistory", "Id", "ComplianceProcessHistoryId");
        }
    }
}
