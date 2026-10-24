namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class R_QuickLink : DbMigration
    {
        public override void Up()
        {
            CreateTable(
               "dbo.DI_M_DefaultRoleQuickLink",
               c => new
               {
                   Id = c.Int(nullable: false, identity: true),
                   RoleId = c.Int(nullable: false),
                   QuickLinkId = c.Int(nullable: false),
                   CreatedBy = c.String(),
                   CreatedDate = c.DateTime(),
                   LastModifiedBy = c.String(),
                   LastModifiedDate = c.DateTime(),
               })
               .PrimaryKey(t => t.Id)
               .ForeignKey("dbo.DI_M_QuickLink", t => t.QuickLinkId, cascadeDelete: true)
               .ForeignKey("dbo.DI_M_Role", t => t.RoleId, cascadeDelete: true)
               .Index(t => t.RoleId)
               .Index(t => t.QuickLinkId);

            DropForeignKey("dbo.DI_M_DefaultRoleMenu", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_DefaultRoleMenu", "TabMenuId", "dbo.DI_M_TabMenu");
            DropForeignKey("dbo.DI_M_QuickLink", "TabMenuId", "dbo.DI_M_TabMenu");
            DropForeignKey("dbo.DI_M_UserQuickLink", "TabMenuId", "dbo.DI_M_TabMenu");
            DropIndex("dbo.DI_M_DefaultRoleMenu", new[] { "RoleId" });
            DropIndex("dbo.DI_M_DefaultRoleMenu", new[] { "TabMenuId" });
            DropIndex("dbo.DI_M_QuickLink", new[] { "TabMenuId" });
            DropIndex("dbo.DI_M_UserQuickLink", new[] { "TabMenuId" });
            AddColumn("dbo.DI_M_DefaultRoleQuickLink", "Position", c => c.Int(nullable: false));
            AddColumn("dbo.DI_M_QuickLink", "QuickLinkId", c => c.Int(nullable: false));
            AddColumn("dbo.DI_M_UserQuickLink", "QuickLinkId", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_M_DefaultRoleQuickLink", "CreatedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.DI_M_QuickLink", "QuickLinkId");
            CreateIndex("dbo.DI_M_UserQuickLink", "QuickLinkId");
            AddForeignKey("dbo.DI_M_QuickLink", "QuickLinkId", "dbo.DI_M_QuickLink", "Id");
            AddForeignKey("dbo.DI_M_UserQuickLink", "QuickLinkId", "dbo.DI_M_QuickLink", "Id", cascadeDelete: true);
            DropColumn("dbo.DI_M_QuickLink", "TabMenuId");
            DropColumn("dbo.DI_M_UserQuickLink", "TabMenuId");
            DropTable("dbo.DI_M_DefaultRoleMenu");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DI_M_DefaultRoleMenu",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        TabMenuId = c.Int(nullable: false),
                        Position = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DI_M_UserQuickLink", "TabMenuId", c => c.Int(nullable: false));
            AddColumn("dbo.DI_M_QuickLink", "TabMenuId", c => c.Int(nullable: false));
            DropForeignKey("dbo.DI_M_UserQuickLink", "QuickLinkId", "dbo.DI_M_QuickLink");
            DropForeignKey("dbo.DI_M_QuickLink", "QuickLinkId", "dbo.DI_M_QuickLink");
            DropIndex("dbo.DI_M_UserQuickLink", new[] { "QuickLinkId" });
            DropIndex("dbo.DI_M_QuickLink", new[] { "QuickLinkId" });
            AlterColumn("dbo.DI_M_DefaultRoleQuickLink", "CreatedDate", c => c.DateTime());
            DropColumn("dbo.DI_M_UserQuickLink", "QuickLinkId");
            DropColumn("dbo.DI_M_QuickLink", "QuickLinkId");
            DropColumn("dbo.DI_M_DefaultRoleQuickLink", "Position");
            CreateIndex("dbo.DI_M_UserQuickLink", "TabMenuId");
            CreateIndex("dbo.DI_M_QuickLink", "TabMenuId");
            CreateIndex("dbo.DI_M_DefaultRoleMenu", "TabMenuId");
            CreateIndex("dbo.DI_M_DefaultRoleMenu", "RoleId");
            AddForeignKey("dbo.DI_M_UserQuickLink", "TabMenuId", "dbo.DI_M_TabMenu", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_QuickLink", "TabMenuId", "dbo.DI_M_TabMenu", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_DefaultRoleMenu", "TabMenuId", "dbo.DI_M_TabMenu", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DI_M_DefaultRoleMenu", "RoleId", "dbo.DI_M_Role", "RoleId", cascadeDelete: true);
            DropTable("dbo.DI_M_DefaultRoleQuickLink");
        }
    }
}
