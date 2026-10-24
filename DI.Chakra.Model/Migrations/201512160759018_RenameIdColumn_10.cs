namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_10 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem", "Id", "ReviewEndpointId");
        }

        public override void Down()
        {
            RenameColumn("dbo.DI_M_AttestationReview_EndpointSystem", "ReviewEndpointId", "Id");
        }
    }
}
