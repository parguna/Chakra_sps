namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Q_AccessRights : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_QckLinkAccessRights",
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_QckLinkAccessRights", "RoleId", "dbo.DI_M_Role");
            DropForeignKey("dbo.DI_M_QckLinkAccessRights", "QuickLinkId", "dbo.DI_M_QuickLink");
            DropIndex("dbo.DI_M_QckLinkAccessRights", new[] { "QuickLinkId" });
            DropIndex("dbo.DI_M_QckLinkAccessRights", new[] { "RoleId" });
            DropTable("dbo.DI_M_QckLinkAccessRights");
        }
    }
}
