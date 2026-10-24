namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_18 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", name: "OracleEBSResponsibilitiesInfo_ResponsibilityId", newName: "OracleEBSResponsibilitiesInfo_Id");
            RenameColumn(table: "dbo.OracleEBSRoleInfoOracleEBSUserInfo", name: "OracleEBSRoleInfo_RoleId", newName: "OracleEBSRoleInfo_Id");
            RenameColumn(table: "dbo.DI_LNS_Configuration", name: "LNSConfigurationId", newName: "Id");
            RenameColumn(table: "dbo.DI_LNS_Certifiers", name: "CertifierId", newName: "Id");
            RenameColumn(table: "dbo.DI_LNS_UsersHistory", name: "UserHistoryId", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_Configuration", name: "OEBSConfigurationId", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_HRMSUserInfo", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_HRMSUserInfoHistory", name: "OEBSHRMSUserInfoHistoryid", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_ResponsibilitiesInfo", name: "ResponsibilityId", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_ResponsibilitiesInfoHistory", name: "ResponsibilityHistoryId", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_RoleInfo", name: "RoleId", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_RoleInfoHistory", name: "RoleHistoryId", newName: "Id");
        }
        
        public override void Down()
        {     
            RenameColumn(table: "dbo.OracleEBSRoleInfoOracleEBSUserInfo", name: "OracleEBSRoleInfo_Id", newName: "OracleEBSRoleInfo_RoleId");
            RenameColumn(table: "dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", name: "OracleEBSResponsibilitiesInfo_Id", newName: "OracleEBSResponsibilitiesInfo_ResponsibilityId");
            RenameColumn(table: "dbo.DI_LNS_Configuration", name: "Id", newName: "LNSConfigurationId");
            RenameColumn(table: "dbo.DI_LNS_Certifiers", name: "Id", newName: "CertifierId");
            RenameColumn(table: "dbo.DI_LNS_UsersHistory", name: "Id", newName: "UserHistoryId");
            RenameColumn(table: "dbo.DI_OEBS_Configuration", name: "Id", newName: "OEBSConfigurationId");
            RenameColumn(table: "dbo.DI_OEBS_HRMSUserInfo", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_OEBS_HRMSUserInfoHistory", name: "Id", newName: "OEBSHRMSUserInfoHistoryid");
            RenameColumn(table: "dbo.DI_OEBS_ResponsibilitiesInfo", name: "Id", newName: "ResponsibilityId");
            RenameColumn(table: "dbo.DI_OEBS_ResponsibilitiesInfoHistory", name: "Id", newName: "ResponsibilityHistoryId");
            RenameColumn(table: "dbo.DI_OEBS_RoleInfo", name: "Id", newName: "RoleId");
            RenameColumn(table: "dbo.DI_OEBS_RoleInfoHistory", name: "Id", newName: "RoleHistoryId");
        }
    }
}
