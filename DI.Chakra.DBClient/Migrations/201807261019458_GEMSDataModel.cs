namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GEMSDataModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_GEMS_RoleInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Name = c.String(),
                        Display_Name = c.String(),
                        Description = c.String(),
                        Extension_ID = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_GEMS_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        USER_NAME = c.String(),
                        DISPLAY_NAME = c.String(),
                        DESCRIPTION = c.String(),
                        EMAIL = c.String(),
                        DATEMODIFIED = c.DateTime(),
                        LASTUSER = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        IsDummyEntry = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_GEMS_RoleMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        BusinessRoleName = c.String(maxLength: 250),
                        TechnicalRole = c.String(maxLength: 250),
                        ADGroups = c.String(),
                        Level1Approver = c.String(maxLength: 250),
                        Level1ApproverType = c.Byte(nullable: false),
                        Level2Approver = c.String(maxLength: 250),
                        Level2ApproverType = c.Byte(nullable: false),
                        Level3Approver = c.String(maxLength: 250),
                        Level3ApproverType = c.Byte(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.GEMSUserInfoGEMSRoleInfo",
                c => new
                    {
                        GEMSUserInfo_Id = c.Long(nullable: false),
                        GEMSRoleInfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.GEMSUserInfo_Id, t.GEMSRoleInfo_Id })
                .ForeignKey("dbo.DI_GEMS_UserInfo", t => t.GEMSUserInfo_Id)
                .ForeignKey("dbo.DI_GEMS_RoleInfo", t => t.GEMSRoleInfo_Id)
                .Index(t => t.GEMSUserInfo_Id)
                .Index(t => t.GEMSRoleInfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_GEMS_RoleMatrix", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.GEMSUserInfoGEMSRoleInfo", "GEMSRoleInfo_Id", "dbo.DI_GEMS_RoleInfo");
            DropForeignKey("dbo.GEMSUserInfoGEMSRoleInfo", "GEMSUserInfo_Id", "dbo.DI_GEMS_UserInfo");
            DropForeignKey("dbo.DI_GEMS_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_GEMS_RoleInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.GEMSUserInfoGEMSRoleInfo", new[] { "GEMSRoleInfo_Id" });
            DropIndex("dbo.GEMSUserInfoGEMSRoleInfo", new[] { "GEMSUserInfo_Id" });
            DropIndex("dbo.DI_GEMS_RoleMatrix", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_GEMS_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_GEMS_RoleInfo", new[] { "EndPointSystemId" });
            DropTable("dbo.GEMSUserInfoGEMSRoleInfo");
            DropTable("dbo.DI_GEMS_RoleMatrix");
            DropTable("dbo.DI_GEMS_UserInfo");
            DropTable("dbo.DI_GEMS_RoleInfo");
        }
    }
}
