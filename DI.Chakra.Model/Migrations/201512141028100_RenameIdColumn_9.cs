namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_9 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_AttestationReview", "Id", "ReviewId");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_AttestationReview", "ReviewId", "Id");
        }
    }
}
