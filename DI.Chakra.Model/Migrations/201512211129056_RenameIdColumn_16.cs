namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_16 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.DI_Oracle11gUM_Configuration", "ORAUMConfigurationId", "Id");
            RenameColumn("dbo.DI_Oracle11gUM_RoleInfo", "RoleId", "Id");
            RenameColumn("dbo.DI_Oracle11gUM_RoleInfoHistory", "ORAUMRoleInfoHistoryId", "Id");
            RenameColumn("dbo.DI_Oracle11gUM_TableInfo", "TableId", "Id");
            RenameColumn("dbo.DI_Oracle11gUM_UserInfoHistory", "ORAUserInfoHistoryid", "Id");
            RenameColumn("dbo.DI_Oracle11gUM_UserRole", "RoleUserId", "Id");
            RenameColumn("dbo.DI_Oracle11gUM_UserTable", "ORAUserTableId", "Id");
            RenameColumn("dbo.DI_Oracle11gUM_UserTableMap", "UserTableMapId", "Id");

            RenameColumn("dbo.DI_SAP_ActivityInfo", "ActivityId", "Id");
            RenameColumn("dbo.DI_SAP_AuthorizationProfile", "AuthorizationID", "Id");
            RenameColumn("dbo.DI_SAP_AuthObjectMap", "SapObjectId", "Id");
            RenameColumn("dbo.DI_SAP_COMPOSITEPROFILEINFO", "SAP_PROFILEID", "Id");
            RenameColumn("dbo.DI_SAP_COMPOSITEROLEINFO", "CompositeRole_Id", "Id");
            RenameColumn("dbo.DI_SAP_Configuration", "SAPConfigurationID", "Id");
        }

        public override void Down()
        {
            RenameColumn("dbo.DI_Oracle11gUM_Configuration", "Id", "ORAUMConfigurationId");
            RenameColumn("dbo.DI_Oracle11gUM_RoleInfo", "Id", "RoleId");
            RenameColumn("dbo.DI_Oracle11gUM_RoleInfoHistory", "Id", "ORAUMRoleInfoHistoryId");
            RenameColumn("dbo.DI_Oracle11gUM_TableInfo", "Id", "TableId");
            RenameColumn("dbo.DI_Oracle11gUM_UserInfoHistory", "Id", "ORAUserInfoHistoryid");
            RenameColumn("dbo.DI_Oracle11gUM_UserRole", "Id", "RoleUserId");
            RenameColumn("dbo.DI_Oracle11gUM_UserTable", "Id", "ORAUserTableId");
            RenameColumn("dbo.DI_Oracle11gUM_UserTableMap", "Id", "UserTableMapId");

            RenameColumn("dbo.DI_SAP_ActivityInfo", "Id", "ActivityId");
            RenameColumn("dbo.DI_SAP_AuthorizationProfile", "Id", "AuthorizationID");
            RenameColumn("dbo.DI_SAP_AuthObjectMap", "Id", "SapObjectId");
            RenameColumn("dbo.DI_SAP_COMPOSITEPROFILEINFO", "Id", "SAP_PROFILEID");
            RenameColumn("dbo.DI_SAP_COMPOSITEROLEINFO", "Id", "CompositeRole_Id");
            RenameColumn("dbo.DI_SAP_Configuration", "Id", "SAPConfigurationID");
        }
    }
}
