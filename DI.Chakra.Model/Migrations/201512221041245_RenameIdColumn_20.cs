namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_20 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DI_M_AttestationReview", name: "ReviewId", newName: "Id");
            RenameColumn(table: "dbo.DI_M_AttestationReview_EndpointSystem", name: "ReviewEndpointId", newName: "Id");           
            
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.DI_M_AttestationReview", name: "Id", newName: "ReviewId");
            RenameColumn(table: "dbo.DI_M_AttestationReview_EndpointSystem", name: "Id", newName: "ReviewEndpointId");
        }
    }
}
