namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBulkRequestIdInAccessRequest : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_AccessRequest", "BulkRequestId", c => c.Long());
            AddColumn("dbo.DI_M_AccessRequest", "EventUndoRequestId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_AccessRequest", "EventUndoRequestId");
            DropColumn("dbo.DI_M_AccessRequest", "BulkRequestId");
        }
    }
}
