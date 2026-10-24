namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W14 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DI_INH_Permission", new[] { "EndPointSystemID" });
            CreateTable(
                "dbo.InhouseUserInfoInhousePermission",
                c => new
                    {
                        InhouseUserInfo_UserId = c.Int(nullable: false),
                        InhousePermission_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.InhouseUserInfo_UserId, t.InhousePermission_Id })
                .ForeignKey("dbo.DI_INH_UserInfo", t => t.InhouseUserInfo_UserId, cascadeDelete: false)
                .ForeignKey("dbo.DI_INH_Permission", t => t.InhousePermission_Id, cascadeDelete: false)
                .Index(t => t.InhouseUserInfo_UserId)
                .Index(t => t.InhousePermission_Id);
            
            CreateIndex("dbo.DI_INH_Permission", "EndPointSystemId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InhouseUserInfoInhousePermission", "InhousePermission_Id", "dbo.DI_INH_Permission");
            DropForeignKey("dbo.InhouseUserInfoInhousePermission", "InhouseUserInfo_UserId", "dbo.DI_INH_UserInfo");
            DropIndex("dbo.InhouseUserInfoInhousePermission", new[] { "InhousePermission_Id" });
            DropIndex("dbo.InhouseUserInfoInhousePermission", new[] { "InhouseUserInfo_UserId" });
            DropIndex("dbo.DI_INH_Permission", new[] { "EndPointSystemId" });
            DropTable("dbo.InhouseUserInfoInhousePermission");
            CreateIndex("dbo.DI_INH_Permission", "EndPointSystemID");
        }
    }
}
