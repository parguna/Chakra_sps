namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIdColumn_21 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_OEBS_UserInfoHistory");
            AlterColumn("dbo.DI_OEBS_UserInfoHistory", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_OEBS_UserInfoHistory", "Id");
            DropColumn("dbo.DI_OEBS_UserInfoHistory", "OEBSUserInfoHistoryid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_OEBS_UserInfoHistory", "OEBSUserInfoHistoryid", c => c.Long(nullable: false, identity: true));
            DropPrimaryKey("dbo.DI_OEBS_UserInfoHistory");
            AlterColumn("dbo.DI_OEBS_UserInfoHistory", "Id", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.DI_OEBS_UserInfoHistory", "OEBSUserInfoHistoryid");
        }
    }
}
