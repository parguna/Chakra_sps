namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_13 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DI_M_AccessRights", "PagePermissionId", "dbo.DI_M_PagePermission");
            DropForeignKey("dbo.DI_M_PagePermission", "PermissionId", "dbo.DI_M_Permission");
            DropIndex("dbo.DI_M_AccessRights", new[] { "PagePermissionId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "PermissionId" });
            DropPrimaryKey("dbo.DI_M_PagePermission");
            DropPrimaryKey("dbo.DI_M_Permission");
            AlterColumn("dbo.DI_M_AccessRights", "PagePermissionId", c => c.Long());
            AlterColumn("dbo.DI_M_PagePermission", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_PagePermission", "PermissionId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_Permission", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_PagePermission", "Id");
            AddPrimaryKey("dbo.DI_M_Permission", "Id");
            CreateIndex("dbo.DI_M_AccessRights", "PagePermissionId");
            CreateIndex("dbo.DI_M_PagePermission", "PermissionId");
            AddForeignKey("dbo.DI_M_AccessRights", "PagePermissionId", "dbo.DI_M_PagePermission", "Id");
            AddForeignKey("dbo.DI_M_PagePermission", "PermissionId", "dbo.DI_M_Permission", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_PagePermission", "PermissionId", "dbo.DI_M_Permission");
            DropForeignKey("dbo.DI_M_AccessRights", "PagePermissionId", "dbo.DI_M_PagePermission");
            DropIndex("dbo.DI_M_PagePermission", new[] { "PermissionId" });
            DropIndex("dbo.DI_M_AccessRights", new[] { "PagePermissionId" });
            DropPrimaryKey("dbo.DI_M_Permission");
            DropPrimaryKey("dbo.DI_M_PagePermission");
            AlterColumn("dbo.DI_M_Permission", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_PagePermission", "PermissionId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_M_PagePermission", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_AccessRights", "PagePermissionId", c => c.Int());
            AddPrimaryKey("dbo.DI_M_Permission", "Id");
            AddPrimaryKey("dbo.DI_M_PagePermission", "Id");
            CreateIndex("dbo.DI_M_PagePermission", "PermissionId");
            CreateIndex("dbo.DI_M_AccessRights", "PagePermissionId");
            AddForeignKey("dbo.DI_M_PagePermission", "PermissionId", "dbo.DI_M_Permission", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_AccessRights", "PagePermissionId", "dbo.DI_M_PagePermission", "Id");
        }
    }
}
