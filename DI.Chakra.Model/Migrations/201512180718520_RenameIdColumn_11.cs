namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_11 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DI_ADS_UserInfo", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_INH_UserInfo", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_Linux_UserInfo", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_LNS_Users", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_UserInfo", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_OEBS_UserInfoHistory", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_Oracle11gUM_UserInfo", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAPEP_UserInfo", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_FIELDVALUEMAPPING", name: "SAPFieldValueMapId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_GROUPHISTORYINFO", name: "SAP_GROUPID", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_GROUPINFO", name: "SAP_GROUPID", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_UserInfo", name: "UserId", newName: "Id");
            RenameColumn(table: "dbo.ADSUserInfoADSGroupInfo", name: "ADSUserInfo_UserId", newName: "ADSUserInfo_Id");
            RenameColumn(table: "dbo.InhouseUserInfoInhousePermission", name: "InhouseUserInfo_UserId", newName: "InhouseUserInfo_Id");
            RenameColumn(table: "dbo.LinuxUserInfoLinuxGroupInfo", name: "LinuxUserInfo_UserId", newName: "LinuxUserInfo_Id");
            RenameColumn(table: "dbo.LotusUserInfoLotusGroupInfo", name: "LotusUserInfo_UserId", newName: "LotusUserInfo_Id");
            RenameColumn(table: "dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", name: "OracleEBSUserInfo_UserId", newName: "OracleEBSUserInfo_Id");
            RenameColumn(table: "dbo.OracleEBSRoleInfoOracleEBSUserInfo", name: "OracleEBSUserInfo_UserId", newName: "OracleEBSUserInfo_Id");
            RenameColumn(table: "dbo.OracleUMUserInfoOracleUMRoleInfo", name: "OracleUMUserInfo_UserId", newName: "OracleUMUserInfo_Id");
            RenameColumn(table: "dbo.OracleUMTableInfoOracleUMUserInfo", name: "OracleUMUserInfo_UserId", newName: "OracleUMUserInfo_Id");
            RenameColumn(table: "dbo.SAPEPUserInfoSAPEPGroupInfo", name: "SAPEPUserInfo_UserId", newName: "SAPEPUserInfo_Id");
            RenameColumn(table: "dbo.SAPEPRoleInfoSAPEPUserInfo", name: "SAPEPUserInfo_UserId", newName: "SAPEPUserInfo_Id");
            RenameColumn(table: "dbo.SAPUserinfoSAPGroupInfo", name: "SAPUserinfo_UserId", newName: "SAPUserinfo_Id");
            RenameColumn(table: "dbo.SAPUserinfoSAPGroupInfo", name: "SAPGroupInfo_SAP_GROUPID", newName: "SAPGroupInfo_Id");
            RenameColumn(table: "dbo.SAPParameterInfoSAPUserinfo", name: "SAPUserinfo_UserId", newName: "SAPUserinfo_Id");
            RenameColumn(table: "dbo.SAPProfileInfoSAPUserinfo", name: "SAPUserinfo_UserId", newName: "SAPUserinfo_Id");
            RenameColumn(table: "dbo.SAPRoleInfoSAPUserinfo", name: "SAPUserinfo_UserId", newName: "SAPUserinfo_Id");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.DI_ADS_UserInfo", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_INH_UserInfo", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_Linux_UserInfo", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_LNS_Users", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_OEBS_UserInfo", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_OEBS_UserInfoHistory", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_Oracle11gUM_UserInfo", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_SAPEP_UserInfo", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.DI_SAP_FIELDVALUEMAPPING", name: "Id", newName: "SAPFieldValueMapId");
            RenameColumn(table: "dbo.DI_SAP_GROUPHISTORYINFO", name: "Id", newName: "SAP_GROUPID");
            RenameColumn(table: "dbo.DI_SAP_GROUPINFO", name: "Id", newName: "SAP_GROUPID");
            RenameColumn(table: "dbo.DI_SAP_UserInfo", name: "Id", newName: "UserId");
            RenameColumn(table: "dbo.SAPRoleInfoSAPUserinfo", name: "SAPUserinfo_Id", newName: "SAPUserinfo_UserId");
            RenameColumn(table: "dbo.SAPProfileInfoSAPUserinfo", name: "SAPUserinfo_Id", newName: "SAPUserinfo_UserId");
            RenameColumn(table: "dbo.SAPParameterInfoSAPUserinfo", name: "SAPUserinfo_Id", newName: "SAPUserinfo_UserId");
            RenameColumn(table: "dbo.SAPUserinfoSAPGroupInfo", name: "SAPGroupInfo_Id", newName: "SAPGroupInfo_SAP_GROUPID");
            RenameColumn(table: "dbo.SAPUserinfoSAPGroupInfo", name: "SAPUserinfo_Id", newName: "SAPUserinfo_UserId");
            RenameColumn(table: "dbo.SAPEPRoleInfoSAPEPUserInfo", name: "SAPEPUserInfo_Id", newName: "SAPEPUserInfo_UserId");
            RenameColumn(table: "dbo.SAPEPUserInfoSAPEPGroupInfo", name: "SAPEPUserInfo_Id", newName: "SAPEPUserInfo_UserId");
            RenameColumn(table: "dbo.OracleUMTableInfoOracleUMUserInfo", name: "OracleUMUserInfo_Id", newName: "OracleUMUserInfo_UserId");
            RenameColumn(table: "dbo.OracleUMUserInfoOracleUMRoleInfo", name: "OracleUMUserInfo_Id", newName: "OracleUMUserInfo_UserId");
            RenameColumn(table: "dbo.OracleEBSRoleInfoOracleEBSUserInfo", name: "OracleEBSUserInfo_Id", newName: "OracleEBSUserInfo_UserId");
            RenameColumn(table: "dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", name: "OracleEBSUserInfo_Id", newName: "OracleEBSUserInfo_UserId");
            RenameColumn(table: "dbo.LotusUserInfoLotusGroupInfo", name: "LotusUserInfo_Id", newName: "LotusUserInfo_UserId");
            RenameColumn(table: "dbo.LinuxUserInfoLinuxGroupInfo", name: "LinuxUserInfo_Id", newName: "LinuxUserInfo_UserId");
            RenameColumn(table: "dbo.InhouseUserInfoInhousePermission", name: "InhouseUserInfo_Id", newName: "InhouseUserInfo_UserId");
            RenameColumn(table: "dbo.ADSUserInfoADSGroupInfo", name: "ADSUserInfo_Id", newName: "ADSUserInfo_UserId");
        }
    }
}
