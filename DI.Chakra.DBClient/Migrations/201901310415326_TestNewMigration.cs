namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestNewMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_NOVA_ExceptionalRoleMatrix", "JobDescription", c => c.String());
            AddColumn("dbo.DI_NOVA_ManagementRoleMatrix", "JobDescription", c => c.String());
            AddColumn("dbo.DI_NOVA_UserRoleMatrix", "JobDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_NOVA_UserRoleMatrix", "JobDescription");
            DropColumn("dbo.DI_NOVA_ManagementRoleMatrix", "JobDescription");
            DropColumn("dbo.DI_NOVA_ExceptionalRoleMatrix", "JobDescription");
        }
    }
}
