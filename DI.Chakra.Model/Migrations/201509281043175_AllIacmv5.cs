namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllIacmv5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_Oracle11gUM_Configuration",
                c => new
                    {
                        ORAUMConfigurationId = c.Int(nullable: false, identity: true),
                        EndpointSystemId = c.Int(nullable: false),
                        HostName = c.String(),
                        IPAddress = c.String(),
                        DomainName = c.String(),
                        ServerName = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        InstanceName = c.String(),
                        PortNo = c.Int(nullable: false),
                        ImportUsers = c.Boolean(nullable: false),
                        ImportRoles = c.Boolean(nullable: false),
                        ImportTables = c.Boolean(nullable: false),
                        MapAll = c.Boolean(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ORAUMConfigurationId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId, cascadeDelete: true)
                .Index(t => t.EndpointSystemId);
            
            CreateTable(
                "dbo.DI_Oracle11gUM_RoleInfo",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(),
                        RoleName = c.String(),
                        Description = c.String(),
                        PasswordRequired = c.String(),
                        AuthenticationType = c.String(),
                        Version = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.DI_Oracle11gUM_UserInfo",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        UserName = c.String(),
                        OraUserId = c.Int(nullable: false),
                        AccountStatus = c.String(),
                        CreatedAt = c.DateTime(),
                        LockAt = c.DateTime(),
                        ExpiryAt = c.DateTime(),
                        DefaultTableSpace = c.String(),
                        Profile = c.String(),
                        InitialRSRCConsumerGroup = c.String(),
                        AuthenticationType = c.String(),
                        DIUserId = c.Int(),
                        SpecialAccount = c.Boolean(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Version = c.Int(nullable: false),
                        AccountName = c.String(),
                        IsLocked = c.Boolean(nullable: false),
                        LockScenario = c.Byte(nullable: false),
                        IsUserAccount = c.Boolean(nullable: false),
                        AccountPassword = c.String(),
                        AccountType = c.Byte(nullable: false),
                        MappingType = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndPointSystemId, cascadeDelete: true)
                .Index(t => t.EndPointSystemId);
            
            CreateTable(
                "dbo.DI_Oracle11gUM_TableInfo",
                c => new
                    {
                        TableId = c.Int(nullable: false, identity: true),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        Owner = c.String(),
                        TableName = c.String(),
                        TableSpaceName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.TableId);
            
            CreateTable(
                "dbo.DI_Oracle11gUM_UserRole",
                c => new
                    {
                        RoleUserId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        DateModified = c.DateTime(),
                        LastUser = c.String(),
                        EndpointTypeSystemId = c.Int(nullable: false),
                        Version = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.RoleUserId)
                .ForeignKey("dbo.DI_Oracle11gUM_UserInfo", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.DI_Oracle11gUM_RoleInfoHistory",
                c => new
                    {
                        ORAUMRoleInfoHistoryId = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        RoleName = c.String(),
                        PasswordRequired = c.String(),
                        AuthenticationType = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ORAUMRoleInfoHistoryId);
            
            CreateTable(
                "dbo.DI_Oracle11gUM_UserInfoHistory",
                c => new
                    {
                        ORAUserInfoHistoryid = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        UserName = c.String(),
                        OraUserId = c.Int(nullable: false),
                        AccountStatus = c.String(),
                        CreatedAt = c.DateTime(),
                        LockAt = c.DateTime(),
                        ExpiryAt = c.DateTime(),
                        DefaultTableSpace = c.String(),
                        Profile = c.String(),
                        InitialRSRCConsumerGroup = c.String(),
                        AuthenticationType = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ORAUserInfoHistoryid);
            
            CreateTable(
                "dbo.DI_Oracle11gUM_UserTable",
                c => new
                    {
                        ORAUserTableId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TableId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ORAUserTableId);
            
            CreateTable(
                "dbo.DI_Oracle11gUM_UserTableMap",
                c => new
                    {
                        UserTableMapId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TenantId = c.String(),
                        EndPointSystemId = c.Int(nullable: false),
                        UserName = c.String(),
                        TableName = c.String(),
                        TableSpaceName = c.String(),
                        ClusterName = c.String(),
                        IOTName = c.String(),
                        Status = c.String(),
                        LastAnalyzed = c.DateTime(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserTableMapId);
            
            CreateTable(
                "dbo.OracleUMUserInfoOracleUMRoleInfo",
                c => new
                    {
                        OracleUMUserInfo_UserId = c.Int(nullable: false),
                        OracleUMRoleInfo_RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OracleUMUserInfo_UserId, t.OracleUMRoleInfo_RoleId })
                .ForeignKey("dbo.DI_Oracle11gUM_UserInfo", t => t.OracleUMUserInfo_UserId, cascadeDelete: true)
                .ForeignKey("dbo.DI_Oracle11gUM_RoleInfo", t => t.OracleUMRoleInfo_RoleId, cascadeDelete: true)
                .Index(t => t.OracleUMUserInfo_UserId)
                .Index(t => t.OracleUMRoleInfo_RoleId);
            
            CreateTable(
                "dbo.OracleUMTableInfoOracleUMUserInfo",
                c => new
                    {
                        OracleUMTableInfo_TableId = c.Int(nullable: false),
                        OracleUMUserInfo_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OracleUMTableInfo_TableId, t.OracleUMUserInfo_UserId })
                .ForeignKey("dbo.DI_Oracle11gUM_TableInfo", t => t.OracleUMTableInfo_TableId, cascadeDelete: true)
                .ForeignKey("dbo.DI_Oracle11gUM_UserInfo", t => t.OracleUMUserInfo_UserId, cascadeDelete: true)
                .Index(t => t.OracleUMTableInfo_TableId)
                .Index(t => t.OracleUMUserInfo_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_Oracle11gUM_UserRole", "UserId", "dbo.DI_Oracle11gUM_UserInfo");
            DropForeignKey("dbo.OracleUMTableInfoOracleUMUserInfo", "OracleUMUserInfo_UserId", "dbo.DI_Oracle11gUM_UserInfo");
            DropForeignKey("dbo.OracleUMTableInfoOracleUMUserInfo", "OracleUMTableInfo_TableId", "dbo.DI_Oracle11gUM_TableInfo");
            DropForeignKey("dbo.OracleUMUserInfoOracleUMRoleInfo", "OracleUMRoleInfo_RoleId", "dbo.DI_Oracle11gUM_RoleInfo");
            DropForeignKey("dbo.OracleUMUserInfoOracleUMRoleInfo", "OracleUMUserInfo_UserId", "dbo.DI_Oracle11gUM_UserInfo");
            DropForeignKey("dbo.DI_Oracle11gUM_UserInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropForeignKey("dbo.DI_Oracle11gUM_Configuration", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.OracleUMTableInfoOracleUMUserInfo", new[] { "OracleUMUserInfo_UserId" });
            DropIndex("dbo.OracleUMTableInfoOracleUMUserInfo", new[] { "OracleUMTableInfo_TableId" });
            DropIndex("dbo.OracleUMUserInfoOracleUMRoleInfo", new[] { "OracleUMRoleInfo_RoleId" });
            DropIndex("dbo.OracleUMUserInfoOracleUMRoleInfo", new[] { "OracleUMUserInfo_UserId" });
            DropIndex("dbo.DI_Oracle11gUM_UserRole", new[] { "UserId" });
            DropIndex("dbo.DI_Oracle11gUM_UserInfo", new[] { "EndPointSystemId" });
            DropIndex("dbo.DI_Oracle11gUM_Configuration", new[] { "EndpointSystemId" });
            DropTable("dbo.OracleUMTableInfoOracleUMUserInfo");
            DropTable("dbo.OracleUMUserInfoOracleUMRoleInfo");
            DropTable("dbo.DI_Oracle11gUM_UserTableMap");
            DropTable("dbo.DI_Oracle11gUM_UserTable");
            DropTable("dbo.DI_Oracle11gUM_UserInfoHistory");
            DropTable("dbo.DI_Oracle11gUM_RoleInfoHistory");
            DropTable("dbo.DI_Oracle11gUM_UserRole");
            DropTable("dbo.DI_Oracle11gUM_TableInfo");
            DropTable("dbo.DI_Oracle11gUM_UserInfo");
            DropTable("dbo.DI_Oracle11gUM_RoleInfo");
            DropTable("dbo.DI_Oracle11gUM_Configuration");
        }
    }
}
