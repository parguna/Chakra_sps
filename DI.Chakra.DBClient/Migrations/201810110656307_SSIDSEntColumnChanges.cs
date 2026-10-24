namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SSIDSEntColumnChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SSIDS_RoleInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        RoleName = c.String(),
                        Description = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_SSIDS_UserInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        User_Id = c.String(),
                        UserName = c.String(),
                        Section = c.String(),
                        Designation = c.String(),
                        Company = c.String(),
                        Branch = c.String(),
                        Division = c.String(),
                        OfficeTelePhone = c.String(),
                        HandPhone = c.String(),
                        Fax = c.String(),
                        ImageLink = c.String(),
                        IsActive = c.Boolean(),
                        LastLogon = c.DateTime(),
                        UpdatedDate = c.DateTime(),
                        Encrypt_Pwd = c.String(),
                        ValidDate = c.DateTime(),
                        InValidDate = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.SSIDSUserInfoSSIDSRolelnfo",
                c => new
                    {
                        SSIDSUserInfo_Id = c.Long(nullable: false),
                        SSIDSRolelnfo_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SSIDSUserInfo_Id, t.SSIDSRolelnfo_Id })
                .ForeignKey("dbo.DI_SSIDS_UserInfo", t => t.SSIDSUserInfo_Id)
                .ForeignKey("dbo.DI_SSIDS_RoleInfo", t => t.SSIDSRolelnfo_Id)
                .Index(t => t.SSIDSUserInfo_Id)
                .Index(t => t.SSIDSRolelnfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SSIDSUserInfoSSIDSRolelnfo", "SSIDSRolelnfo_Id", "dbo.DI_SSIDS_RoleInfo");
            DropForeignKey("dbo.SSIDSUserInfoSSIDSRolelnfo", "SSIDSUserInfo_Id", "dbo.DI_SSIDS_UserInfo");
            DropForeignKey("dbo.DI_SSIDS_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_SSIDS_RoleInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.SSIDSUserInfoSSIDSRolelnfo", new[] { "SSIDSRolelnfo_Id" });
            DropIndex("dbo.SSIDSUserInfoSSIDSRolelnfo", new[] { "SSIDSUserInfo_Id" });
            DropIndex("dbo.DI_SSIDS_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_SSIDS_RoleInfo", new[] { "EndPointSystemId" });
            DropTable("dbo.SSIDSUserInfoSSIDSRolelnfo");
            DropTable("dbo.DI_SSIDS_UserInfo");
            DropTable("dbo.DI_SSIDS_RoleInfo");
        }
    }
}
