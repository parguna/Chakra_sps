namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_17 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", name: "SAPSingleRoleInfo_SingleRole_Id", newName: "SAPSingleRoleInfo_Id");
            RenameColumn(table: "dbo.SAPTCodeInfoSAPSingleRoleInfo", name: "SAPTCodeInfo_TCODE_Id", newName: "SAPTCodeInfo_Id");
            RenameColumn(table: "dbo.SAPTCodeInfoSAPSingleRoleInfo", name: "SAPSingleRoleInfo_SingleRole_Id", newName: "SAPSingleRoleInfo_Id");
            RenameColumn(table: "dbo.SAPParameterInfoSAPUserinfo", name: "SAPParameterInfo_SAP_ParameterID", newName: "SAPParameterInfo_Id");
            RenameColumn(table: "dbo.SAPProfileInfoSAPUserinfo", name: "SAPProfileInfo_SAP_PROFILEID", newName: "SAPProfileInfo_Id");
            RenameColumn(table: "dbo.SAPRoleInfoSAPUserinfo", name: "SAPRoleInfo_SAP_ROLEID", newName: "SAPRoleInfo_Id");

            RenameColumn(table: "dbo.DI_SAP_SINGLEROLEINFO", name: "SingleRole_Id", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_TCODEINFO", name: "TCODE_Id", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_PARAMETERINFO", name: "SAP_ParameterID", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_PROFILEINFO", name: "SAP_PROFILEID", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_ROLEINFO", name: "SAP_ROLEID", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_Objects", name: "SapObjectId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_ObjectValues", name: "SapObjectValueId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_PARAMETERHISTORYINFO", name: "SAP_ParameterID", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_Position", name: "PositionId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_ROLETCODEMAPPING", name: "SapRoleTCodeMappingId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_RoleToAuthMap", name: "SAPRoleToAuthMapId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_TCodeObject", name: "TCodeObjectId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_UserHistoryInfo", name: "UserHistoryId", newName: "Id");
            RenameColumn(table: "dbo.DI_SAP_UserProfile", name: "SapUserProfileId", newName: "Id"); 

            
        }
        
        public override void Down()
        {
           
            RenameColumn(table: "dbo.SAPRoleInfoSAPUserinfo", name: "SAPRoleInfo_Id", newName: "SAPRoleInfo_SAP_ROLEID");
            RenameColumn(table: "dbo.SAPProfileInfoSAPUserinfo", name: "SAPProfileInfo_Id", newName: "SAPProfileInfo_SAP_PROFILEID");
            RenameColumn(table: "dbo.SAPParameterInfoSAPUserinfo", name: "SAPParameterInfo_Id", newName: "SAPParameterInfo_SAP_ParameterID");
            RenameColumn(table: "dbo.SAPTCodeInfoSAPSingleRoleInfo", name: "SAPSingleRoleInfo_Id", newName: "SAPSingleRoleInfo_SingleRole_Id");
            RenameColumn(table: "dbo.SAPTCodeInfoSAPSingleRoleInfo", name: "SAPTCodeInfo_Id", newName: "SAPTCodeInfo_TCODE_Id");
            RenameColumn(table: "dbo.SAPSingleRoleInfoSAPCompositeRoleInfo", name: "SAPSingleRoleInfo_Id", newName: "SAPSingleRoleInfo_SingleRole_Id");

            RenameColumn(table: "dbo.DI_SAP_SINGLEROLEINFO", name: "Id", newName: "SingleRole_Id");
            RenameColumn(table: "dbo.DI_SAP_TCODEINFO", name: "Id", newName: "TCODE_Id");
            RenameColumn(table: "dbo.DI_SAP_PARAMETERINFO", name: "Id", newName: "SAP_ParameterID");
            RenameColumn(table: "dbo.DI_SAP_PROFILEINFO", name: "Id", newName: "SAP_PROFILEID");
            RenameColumn(table: "dbo.DI_SAP_ROLEINFO", name: "Id", newName: "SAP_ROLEID");
            RenameColumn(table: "dbo.DI_SAP_Objects", name: "Id", newName: "SapObjectId");
            RenameColumn(table: "dbo.DI_SAP_ObjectValues", name: "Id", newName: "SapObjectValueId");
            RenameColumn(table: "dbo.DI_SAP_PARAMETERHISTORYINFO", name: "Id", newName: "SAP_ParameterID");
            RenameColumn(table: "dbo.DI_SAP_Position", name: "Id", newName: "PositionId");
            RenameColumn(table: "dbo.DI_SAP_ROLETCODEMAPPING", name: "Id", newName: "SapRoleTCodeMappingId");
            RenameColumn(table: "dbo.DI_SAP_RoleToAuthMap", name: "Id", newName: "SAPRoleToAuthMapId");
            RenameColumn(table: "dbo.DI_SAP_TCodeObject", name: "Id", newName: "TCodeObjectId");
            RenameColumn(table: "dbo.DI_SAP_UserHistoryInfo", name: "Id", newName: "UserHistoryId");
            RenameColumn(table: "dbo.DI_SAP_UserProfile", name: "Id", newName: "SapUserProfileId");
        }
    }
}
