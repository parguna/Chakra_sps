namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InHousePermission : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InhouseUserInfoInhousePermission", "InhouseUserInfo_Id", "dbo.DI_INH_UserInfo");
            DropForeignKey("dbo.InhouseUserInfoInhousePermission", "InhousePermission_Id", "dbo.DI_INH_Permission");
            DropForeignKey("dbo.DI_INH_Permission", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_INH_Permission", new[] { "EndPointSystemId" });
            DropIndex("dbo.InhouseUserInfoInhousePermission", new[] { "InhouseUserInfo_Id" });
            DropIndex("dbo.InhouseUserInfoInhousePermission", new[] { "InhousePermission_Id" });
            AddColumn("dbo.DI_INH_Permission", "Level1ApproverType", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_INH_Permission", "Level2ApproverType", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_INH_Permission", "Level3Approver", c => c.String(maxLength: 250));
            AddColumn("dbo.DI_INH_Permission", "Level3ApproverType", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_INH_Permission", "EndPoint_Id", c => c.Long());
            AddColumn("dbo.DI_INH_Permission", "InhouseUserInfo_Id", c => c.Long());
            AlterColumn("dbo.DI_INH_Permission", "Level1Approver", c => c.String(maxLength: 250));
            AlterColumn("dbo.DI_INH_Permission", "Level2Approver", c => c.String(maxLength: 250));
            CreateIndex("dbo.DI_INH_Permission", "EndPoint_Id");
            CreateIndex("dbo.DI_INH_Permission", "InhouseUserInfo_Id");
            AddForeignKey("dbo.DI_INH_Permission", "InhouseUserInfo_Id", "dbo.DI_INH_UserInfo", "Id");
            AddForeignKey("dbo.DI_INH_Permission", "EndPoint_Id", "dbo.DI_M_EndpointSystem", "Id");
            DropColumn("dbo.DI_INH_Permission", "CanRequest");
            DropColumn("dbo.DI_INH_Permission", "UserDefinedDisplayName");
            DropColumn("dbo.DI_INH_Permission", "IsLevel1Group");
            DropColumn("dbo.DI_INH_Permission", "IsLevel2Group");
            DropTable("dbo.InhouseUserInfoInhousePermission");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.InhouseUserInfoInhousePermission",
                c => new
                    {
                        InhouseUserInfo_Id = c.Long(nullable: false),
                        InhousePermission_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.InhouseUserInfo_Id, t.InhousePermission_Id });
            
            AddColumn("dbo.DI_INH_Permission", "IsLevel2Group", c => c.Boolean());
            AddColumn("dbo.DI_INH_Permission", "IsLevel1Group", c => c.Boolean());
            AddColumn("dbo.DI_INH_Permission", "UserDefinedDisplayName", c => c.String());
            AddColumn("dbo.DI_INH_Permission", "CanRequest", c => c.Boolean());
            DropForeignKey("dbo.DI_INH_Permission", "EndPoint_Id", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_INH_Permission", "InhouseUserInfo_Id", "dbo.DI_INH_UserInfo");
            DropIndex("dbo.DI_INH_Permission", new[] { "InhouseUserInfo_Id" });
            DropIndex("dbo.DI_INH_Permission", new[] { "EndPoint_Id" });
            AlterColumn("dbo.DI_INH_Permission", "Level2Approver", c => c.Long());
            AlterColumn("dbo.DI_INH_Permission", "Level1Approver", c => c.Long());
            DropColumn("dbo.DI_INH_Permission", "InhouseUserInfo_Id");
            DropColumn("dbo.DI_INH_Permission", "EndPoint_Id");
            DropColumn("dbo.DI_INH_Permission", "Level3ApproverType");
            DropColumn("dbo.DI_INH_Permission", "Level3Approver");
            DropColumn("dbo.DI_INH_Permission", "Level2ApproverType");
            DropColumn("dbo.DI_INH_Permission", "Level1ApproverType");
            CreateIndex("dbo.InhouseUserInfoInhousePermission", "InhousePermission_Id");
            CreateIndex("dbo.InhouseUserInfoInhousePermission", "InhouseUserInfo_Id");
            CreateIndex("dbo.DI_INH_Permission", "EndPointSystemId");
            AddForeignKey("dbo.DI_INH_Permission", "EndPointSystemId", "dbo.DI_M_EndpointSystem", "Id");
            AddForeignKey("dbo.InhouseUserInfoInhousePermission", "InhousePermission_Id", "dbo.DI_INH_Permission", "Id");
            AddForeignKey("dbo.InhouseUserInfoInhousePermission", "InhouseUserInfo_Id", "dbo.DI_INH_UserInfo", "Id");
        }
    }
}
