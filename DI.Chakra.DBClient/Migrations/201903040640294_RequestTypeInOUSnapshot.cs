namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequestTypeInOUSnapshot : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_OUChangeSnapshot", "RequestId", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_OUChangeSnapshot", "RequestId");
        }
    }
}
