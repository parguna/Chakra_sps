namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_SODPolicyExceutionDefinition", "RunDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_SODPolicyExceutionDefinition", "RunDate", c => c.DateTime(nullable: false));
        }
    }
}
