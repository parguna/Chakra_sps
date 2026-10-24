namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_15 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ADSUserInfoADSGroupInfo", name: "ADSGroupInfo_GroupId", newName: "ADSGroupInfo_Id");
            RenameColumn(table: "dbo.DI_ADS_ComputerInfo", name: "ComputerId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_Configuration", name: "ADSConfigurationId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_ConfigurationHistory", name: "HistoryId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_GroupInfo", name: "GroupId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_GroupInfoHistory", name: "GroupHistoryId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_OUInfo", name: "OuId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_OuInfoHistory", name: "OuHistoryId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_PrinterInfo", name: "PrinterId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_SharedFolderInfo", name: "FolderId", newName: "Id");
            RenameColumn(table: "dbo.DI_ADS_UserInfoHistory", name: "UserHistoryId", newName: "Id");
            RenameColumn(table: "dbo.DI_INH_Configuration", name: "InhouseSystemId", newName: "Id");
            RenameColumn(table: "dbo.DI_Linux_Configuration", name: "LinuxConfigurationId", newName: "Id");
            RenameColumn(table: "dbo.DI_Linux_ResourceInfo", name: "Resource_Id", newName: "Id");
            RenameColumn(table: "dbo.DI_Linux_ResourceInfoHistory", name: "Resource_Id", newName: "Id");
            RenameColumn(table: "dbo.DI_Linux_UserInfoHistory", name: "UserId", newName: "Id");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.ADSUserInfoADSGroupInfo", name: "ADSGroupInfo_Id", newName: "ADSGroupInfo_GroupId");
            RenameColumn(table: "dbo.DI_ADS_ComputerInfo", name: "Id", newName: "ComputerId");
            RenameColumn(table: "dbo.DI_ADS_Configuration", name: "Id", newName: "ADSConfigurationId");
            RenameColumn(table: "dbo.DI_ADS_ConfigurationHistory", name: "Id", newName: "HistoryId");
            RenameColumn(table: "dbo.DI_ADS_GroupInfo", name: "Id", newName: "GroupId");
            RenameColumn(table: "dbo.DI_ADS_GroupInfoHistory", name: "Id", newName: "GroupHistoryId");
            RenameColumn(table: "dbo.DI_ADS_OUInfo", name: "Id", newName: "OuId");
            RenameColumn(table: "dbo.DI_ADS_OuInfoHistory", name: "Id", newName: "OuIOuHistoryId");
            RenameColumn(table: "dbo.DI_ADS_PrinterInfo", name: "Id", newName: "PrinterId");
            RenameColumn(table: "dbo.DI_ADS_SharedFolderInfo", name: "Id", newName: "FolderId");
            RenameColumn(table: "dbo.DI_ADS_UserInfoHistory", name: "Id", newName: "UserHistoryId");
            RenameColumn(table: "dbo.DI_INH_Configuration", name: "Id", newName: "InhouseSystemId");
            RenameColumn(table: "dbo.DI_Linux_Configuration", name: "Id", newName: "LinuxConfigurationId");
            RenameColumn(table: "dbo.DI_Linux_ResourceInfo", name: "Id", newName: "Resource_Id");
            RenameColumn(table: "dbo.DI_Linux_ResourceInfoHistory", name: "Id", newName: "Resource_Id");
            RenameColumn(table: "dbo.DI_Linux_UserInfoHistory", name: "Id", newName: "UserId");
        }
    }
}
