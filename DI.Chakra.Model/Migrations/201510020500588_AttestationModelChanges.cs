namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttestationModelChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_AttestationReview_EndpointSystem", "AccountType", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_AttestationReview_EndpointSystem", "AccountType", c => c.String());
        }
    }
}
