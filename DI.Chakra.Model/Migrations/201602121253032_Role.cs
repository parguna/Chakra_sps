namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Role : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_M_Role", "RoleId", "Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.DI_M_Role", "Id", "RoleId");
        }
    }
}
