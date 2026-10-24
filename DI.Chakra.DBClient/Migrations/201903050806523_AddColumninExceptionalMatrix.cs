namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumninExceptionalMatrix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_NOVA_ExceptionalRoleMatrix", "IsExclusiveForUserAccount", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_NOVA_ExceptionalRoleMatrix", "IsExclusiveForUserAccount");
        }
    }
}
