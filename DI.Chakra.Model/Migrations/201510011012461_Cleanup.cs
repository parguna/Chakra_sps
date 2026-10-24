namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cleanup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_M_RoleMatrixDetail", "ProvisioningRoleId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_RoleMatrixDetail", "ProvisioningRoleId", c => c.Int());
        }
    }
}
