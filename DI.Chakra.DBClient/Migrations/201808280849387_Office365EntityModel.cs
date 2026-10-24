namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Office365EntityModel : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DI_O365_GroupInfo", newName: "DI_Office365_GroupInfo");
            RenameTable(name: "dbo.DI_O365_UserInfo", newName: "DI_Office365_UserInfo");
            RenameTable(name: "dbo.O365UserInfoO365GroupInfo", newName: "Office365UserInfoOffice365GroupInfo");
            RenameColumn(table: "dbo.Office365UserInfoOffice365GroupInfo", name: "O365UserInfo_Id", newName: "Office365UserInfo_Id");
            RenameColumn(table: "dbo.Office365UserInfoOffice365GroupInfo", name: "O365GroupInfo_Id", newName: "Office365GroupInfo_Id");
            RenameIndex(table: "dbo.Office365UserInfoOffice365GroupInfo", name: "IX_O365UserInfo_Id", newName: "IX_Office365UserInfo_Id");
            RenameIndex(table: "dbo.Office365UserInfoOffice365GroupInfo", name: "IX_O365GroupInfo_Id", newName: "IX_Office365GroupInfo_Id");
          
            CreateIndex("dbo.DI_Office365_GroupInfo", "EndPointSystemId");
            AddForeignKey("dbo.DI_Office365_GroupInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem", "Id");
            DropColumn("dbo.DI_Office365_UserInfo", "Id1");
            DropColumn("dbo.DI_Office365_UserInfo", "MappingType");
            DropColumn("dbo.DI_Office365_UserInfo", "DIUserId");
            DropColumn("dbo.DI_Office365_UserInfo", "IACMLastPasswordChange");
            DropColumn("dbo.DI_Office365_UserInfo", "IfUnmappedMapTo");
            DropColumn("dbo.DI_Office365_UserInfo", "Version");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_Office365_UserInfo", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.DI_Office365_UserInfo", "IfUnmappedMapTo", c => c.String());
            AddColumn("dbo.DI_Office365_UserInfo", "IACMLastPasswordChange", c => c.DateTime());
            AddColumn("dbo.DI_Office365_UserInfo", "DIUserId", c => c.Int());
            AddColumn("dbo.DI_Office365_UserInfo", "MappingType", c => c.String());
            AddColumn("dbo.DI_Office365_UserInfo", "Id1", c => c.String());
            DropForeignKey("dbo.DI_Office365_GroupInfo", "EndPointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_Office365_GroupInfo", new[] { "EndPointSystemId" });
           
            RenameIndex(table: "dbo.Office365UserInfoOffice365GroupInfo", name: "IX_Office365GroupInfo_Id", newName: "IX_O365GroupInfo_Id");
            RenameIndex(table: "dbo.Office365UserInfoOffice365GroupInfo", name: "IX_Office365UserInfo_Id", newName: "IX_O365UserInfo_Id");
            RenameColumn(table: "dbo.Office365UserInfoOffice365GroupInfo", name: "Office365GroupInfo_Id", newName: "O365GroupInfo_Id");
            RenameColumn(table: "dbo.Office365UserInfoOffice365GroupInfo", name: "Office365UserInfo_Id", newName: "O365UserInfo_Id");
            RenameTable(name: "dbo.Office365UserInfoOffice365GroupInfo", newName: "O365UserInfoO365GroupInfo");
            RenameTable(name: "dbo.DI_Office365_UserInfo", newName: "DI_O365_UserInfo");
            RenameTable(name: "dbo.DI_Office365_GroupInfo", newName: "DI_O365_GroupInfo");
        }
    }
}
