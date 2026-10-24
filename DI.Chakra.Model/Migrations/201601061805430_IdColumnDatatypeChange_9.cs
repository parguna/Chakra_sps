namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_9 : DbMigration
    {
        public override void Up()
        {
            //Sql("ALTER TABLE DI_M_AdminModeItem DROP CONSTRAINT DF__DI_M_Admi__TabMe__2C1E8537");

            Sql(@"
                DECLARE @DI_M_AdminModeItem_TabMenuId_Default_Contraint VarChar(200);

                SELECT @DI_M_AdminModeItem_TabMenuId_Default_Contraint = D.name
                FROM sys.default_constraints AS D
                INNER JOIN sys.columns AS C
                ON D.parent_column_id = C.column_id
                WHERE D.parent_object_id = OBJECT_ID(N'dbo.DI_M_AdminModeItem')
                AND C.name = 'TabMenuId';

                IF(@DI_M_AdminModeItem_TabMenuId_Default_Contraint IS NOT NULL)
	                EXEC (N'ALTER TABLE dbo.DI_M_AdminModeItem DROP CONSTRAINT ' + @DI_M_AdminModeItem_TabMenuId_Default_Contraint)
             ");

            DropForeignKey("dbo.DI_M_PagePermission", "PageId", "dbo.DI_M_Page");
            DropForeignKey("dbo.DI_M_TabMenu", "PageId", "dbo.DI_M_Page");
            DropForeignKey("dbo.DI_M_Page", "PageUrlId", "dbo.DI_M_PageUrl");
            DropForeignKey("dbo.DI_M_PagePermission", "UrlId", "dbo.DI_M_PageUrl");
            DropForeignKey("dbo.DI_M_AdminModeItem", "TabMenuId", "dbo.DI_M_TabMenu");
            DropIndex("dbo.DI_M_PagePermission", new[] { "PageId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "UrlId" });
            DropIndex("dbo.DI_M_Page", new[] { "PageUrlId" });
            DropIndex("dbo.DI_M_AdminModeItem", new[] { "TabMenuId" });
            DropIndex("dbo.DI_M_TabMenu", new[] { "PageId" });
            DropPrimaryKey("dbo.DI_M_Page");
            DropPrimaryKey("dbo.DI_M_PageUrl");
            DropPrimaryKey("dbo.DI_M_TabMenu");
            AlterColumn("dbo.DI_M_PagePermission", "PageId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_PagePermission", "UrlId", c => c.Long());
            AlterColumn("dbo.DI_M_Page", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_Page", "PageUrlId", c => c.Long());
            AlterColumn("dbo.DI_M_PageUrl", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_AdminModeItem", "TabMenuId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_TabMenu", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_TabMenu", "PageId", c => c.Long());
            AddPrimaryKey("dbo.DI_M_Page", "Id");
            AddPrimaryKey("dbo.DI_M_PageUrl", "Id");
            AddPrimaryKey("dbo.DI_M_TabMenu", "Id");
            CreateIndex("dbo.DI_M_PagePermission", "PageId");
            CreateIndex("dbo.DI_M_PagePermission", "UrlId");
            CreateIndex("dbo.DI_M_Page", "PageUrlId");
            CreateIndex("dbo.DI_M_AdminModeItem", "TabMenuId");
            CreateIndex("dbo.DI_M_TabMenu", "PageId");
            AddForeignKey("dbo.DI_M_PagePermission", "PageId", "dbo.DI_M_Page", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_TabMenu", "PageId", "dbo.DI_M_Page", "Id");
            AddForeignKey("dbo.DI_M_Page", "PageUrlId", "dbo.DI_M_PageUrl", "Id");
            AddForeignKey("dbo.DI_M_PagePermission", "UrlId", "dbo.DI_M_PageUrl", "Id");
            AddForeignKey("dbo.DI_M_AdminModeItem", "TabMenuId", "dbo.DI_M_TabMenu", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_AdminModeItem", "TabMenuId", "dbo.DI_M_TabMenu");
            DropForeignKey("dbo.DI_M_PagePermission", "UrlId", "dbo.DI_M_PageUrl");
            DropForeignKey("dbo.DI_M_Page", "PageUrlId", "dbo.DI_M_PageUrl");
            DropForeignKey("dbo.DI_M_TabMenu", "PageId", "dbo.DI_M_Page");
            DropForeignKey("dbo.DI_M_PagePermission", "PageId", "dbo.DI_M_Page");
            DropIndex("dbo.DI_M_TabMenu", new[] { "PageId" });
            DropIndex("dbo.DI_M_AdminModeItem", new[] { "TabMenuId" });
            DropIndex("dbo.DI_M_Page", new[] { "PageUrlId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "UrlId" });
            DropIndex("dbo.DI_M_PagePermission", new[] { "PageId" });
            DropPrimaryKey("dbo.DI_M_TabMenu");
            DropPrimaryKey("dbo.DI_M_PageUrl");
            DropPrimaryKey("dbo.DI_M_Page");
            AlterColumn("dbo.DI_M_TabMenu", "PageId", c => c.Int());
            AlterColumn("dbo.DI_M_TabMenu", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_AdminModeItem", "TabMenuId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_M_PageUrl", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_Page", "PageUrlId", c => c.Int());
            AlterColumn("dbo.DI_M_Page", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_PagePermission", "UrlId", c => c.Int());
            AlterColumn("dbo.DI_M_PagePermission", "PageId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.DI_M_TabMenu", "Id");
            AddPrimaryKey("dbo.DI_M_PageUrl", "Id");
            AddPrimaryKey("dbo.DI_M_Page", "Id");
            CreateIndex("dbo.DI_M_TabMenu", "PageId");
            CreateIndex("dbo.DI_M_AdminModeItem", "TabMenuId");
            CreateIndex("dbo.DI_M_Page", "PageUrlId");
            CreateIndex("dbo.DI_M_PagePermission", "UrlId");
            CreateIndex("dbo.DI_M_PagePermission", "PageId");
            AddForeignKey("dbo.DI_M_AdminModeItem", "TabMenuId", "dbo.DI_M_TabMenu", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_PagePermission", "UrlId", "dbo.DI_M_PageUrl", "Id");
            AddForeignKey("dbo.DI_M_Page", "PageUrlId", "dbo.DI_M_PageUrl", "Id");
            AddForeignKey("dbo.DI_M_TabMenu", "PageId", "dbo.DI_M_Page", "Id");
            AddForeignKey("dbo.DI_M_PagePermission", "PageId", "dbo.DI_M_Page", "Id", cascadeDelete: true);
        }
    }
}
