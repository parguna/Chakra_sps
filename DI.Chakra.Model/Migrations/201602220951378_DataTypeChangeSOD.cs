namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataTypeChangeSOD : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_SODPolicyReport", "UserId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_SODPolicyReport", "UserId", c => c.Int(nullable: false));
        }
    }
}
