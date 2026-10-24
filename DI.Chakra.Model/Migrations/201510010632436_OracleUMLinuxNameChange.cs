namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OracleUMLinuxNameChange : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DI_Unix_GroupInfoHistory", newName: "DI_Linux_GroupInfoHistory");
            RenameTable(name: "dbo.DI_Unix_UserInfo", newName: "DI_Linux_UserInfo");
            RenameTable(name: "dbo.DI_Unix_ResourceInfoHistory", newName: "DI_Linux_ResourceInfoHistory");
            RenameTable(name: "dbo.DI_Unix_UserInfoHistory", newName: "DI_Linux_UserInfoHistory");
            DropForeignKey("dbo.DI_M_QuickLink", "QuickLinkId", "dbo.DI_M_QuickLink");
            DropForeignKey("dbo.DI_Unix_Configuration", "EndpointSystem_EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.UnixUserInfoUnixGroupInfo", "UnixUserInfo_UserId", "dbo.DI_Unix_UserInfo");
            DropForeignKey("dbo.UnixUserInfoUnixGroupInfo", "UnixGroupInfo_Id", "dbo.DI_Unix_GroupInfo");
            DropIndex("dbo.DI_M_QuickLink", new[] { "QuickLinkId" });
            DropIndex("dbo.DI_Unix_Configuration", new[] { "EndpointSystem_EndpointSystemId" });
            DropIndex("dbo.UnixUserInfoUnixGroupInfo", new[] { "UnixUserInfo_UserId" });
            DropIndex("dbo.UnixUserInfoUnixGroupInfo", new[] { "UnixGroupInfo_Id" });
            CreateTable(
                "dbo.DI_Linux_Configuration",
                c => new
                    {
                        LinuxConfigurationId = c.Int(nullable: false, identity: true),
                        IPAddress = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndpointSystem_EndpointSystemId = c.Int(),
                    })
                .PrimaryKey(t => t.LinuxConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystem_EndpointSystemId)
                .Index(t => t.EndpointSystem_EndpointSystemId);
            
            CreateTable(
                "dbo.DI_Linux_GroupInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        GroupNumber = c.String(),
                        GroupName = c.String(),
                        GroupPassword = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Linux_ResourceInfo",
                c => new
                    {
                        Resource_Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Resource_Name = c.String(),
                        Resource_Type = c.String(),
                        Resource_User = c.String(),
                        Resource_Group = c.String(),
                        Resource_Parent = c.String(),
                        Resource_Level = c.Int(nullable: false),
                        Resouce_AccessRights = c.String(),
                        Resouce_Size = c.String(),
                        Resouce_LastModified = c.String(),
                        MountDrive = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Resource_Id);
            
            CreateTable(
                "dbo.LinuxUserInfoLinuxGroupInfo",
                c => new
                    {
                        LinuxUserInfo_UserId = c.Int(nullable: false),
                        LinuxGroupInfo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LinuxUserInfo_UserId, t.LinuxGroupInfo_Id })
                .ForeignKey("dbo.DI_Linux_UserInfo", t => t.LinuxUserInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.DI_Linux_GroupInfo", t => t.LinuxGroupInfo_Id, cascadeDelete: true)
                .Index(t => t.LinuxUserInfo_UserId)
                .Index(t => t.LinuxGroupInfo_Id);
            
            AddColumn("dbo.DI_Linux_UserInfo", "LinuxUserId", c => c.String());
            AddColumn("dbo.DI_Linux_UserInfoHistory", "LinuxUserId", c => c.String());
            DropColumn("dbo.DI_Linux_UserInfo", "UnixUserId");
            DropColumn("dbo.DI_Linux_UserInfoHistory", "UnixUserId");
            DropTable("dbo.DI_Unix_Configuration");
            DropTable("dbo.DI_Unix_GroupInfo");
            DropTable("dbo.DI_Unix_ResourceInfo");
            DropTable("dbo.UnixUserInfoUnixGroupInfo");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UnixUserInfoUnixGroupInfo",
                c => new
                    {
                        UnixUserInfo_UserId = c.Int(nullable: false),
                        UnixGroupInfo_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UnixUserInfo_UserId, t.UnixGroupInfo_Id });
            
            CreateTable(
                "dbo.DI_Unix_ResourceInfo",
                c => new
                    {
                        Resource_Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Resource_Name = c.String(),
                        Resource_Type = c.String(),
                        Resource_User = c.String(),
                        Resource_Group = c.String(),
                        Resource_Parent = c.String(),
                        Resource_Level = c.Int(nullable: false),
                        Resouce_AccessRights = c.String(),
                        Resouce_Size = c.String(),
                        Resouce_LastModified = c.String(),
                        MountDrive = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Resource_Id);
            
            CreateTable(
                "dbo.DI_Unix_GroupInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        GroupNumber = c.String(),
                        GroupName = c.String(),
                        GroupPassword = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(nullable: false),
                        Version = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DI_Unix_Configuration",
                c => new
                    {
                        UnixConfigurationId = c.Int(nullable: false, identity: true),
                        IPAddress = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        EndpointSystem_EndpointSystemId = c.Int(),
                    })
                .PrimaryKey(t => t.UnixConfigurationId);
            
            AddColumn("dbo.DI_Linux_UserInfoHistory", "UnixUserId", c => c.String());
            AddColumn("dbo.DI_Linux_UserInfo", "UnixUserId", c => c.String());
            DropForeignKey("dbo.LinuxUserInfoLinuxGroupInfo", "LinuxGroupInfo_Id", "dbo.DI_Linux_GroupInfo");
            DropForeignKey("dbo.LinuxUserInfoLinuxGroupInfo", "LinuxUserInfo_UserId", "dbo.DI_Linux_UserInfo");
            DropForeignKey("dbo.DI_Linux_Configuration", "EndpointSystem_EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.LinuxUserInfoLinuxGroupInfo", new[] { "LinuxGroupInfo_Id" });
            DropIndex("dbo.LinuxUserInfoLinuxGroupInfo", new[] { "LinuxUserInfo_UserId" });
            DropIndex("dbo.DI_Linux_Configuration", new[] { "EndpointSystem_EndpointSystemId" });
            DropColumn("dbo.DI_Linux_UserInfoHistory", "LinuxUserId");
            DropColumn("dbo.DI_Linux_UserInfo", "LinuxUserId");
            DropTable("dbo.LinuxUserInfoLinuxGroupInfo");
            DropTable("dbo.DI_Linux_ResourceInfo");
            DropTable("dbo.DI_Linux_GroupInfo");
            DropTable("dbo.DI_Linux_Configuration");
            CreateIndex("dbo.UnixUserInfoUnixGroupInfo", "UnixGroupInfo_Id");
            CreateIndex("dbo.UnixUserInfoUnixGroupInfo", "UnixUserInfo_UserId");
            CreateIndex("dbo.DI_Unix_Configuration", "EndpointSystem_EndpointSystemId");
            CreateIndex("dbo.DI_M_QuickLink", "QuickLinkId");
            AddForeignKey("dbo.UnixUserInfoUnixGroupInfo", "UnixGroupInfo_Id", "dbo.DI_Unix_GroupInfo", "Id", cascadeDelete: true);
            AddForeignKey("dbo.UnixUserInfoUnixGroupInfo", "UnixUserInfo_UserId", "dbo.DI_Unix_UserInfo", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.DI_Unix_Configuration", "EndpointSystem_EndpointSystemId", "dbo.DI_M_EndpointSystem", "EndpointSystemId");
            AddForeignKey("dbo.DI_M_QuickLink", "QuickLinkId", "dbo.DI_M_QuickLink", "Id");
            RenameTable(name: "dbo.DI_Linux_UserInfoHistory", newName: "DI_Unix_UserInfoHistory");
            RenameTable(name: "dbo.DI_Linux_ResourceInfoHistory", newName: "DI_Unix_ResourceInfoHistory");
            RenameTable(name: "dbo.DI_Linux_UserInfo", newName: "DI_Unix_UserInfo");
            RenameTable(name: "dbo.DI_Linux_GroupInfoHistory", newName: "DI_Unix_GroupInfoHistory");
        }
    }
}
