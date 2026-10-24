namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_25 : DbMigration
    {
        public override void Up()
        {
            Sql("DROP INDEX [dbo].[OracleEBSUserInfoOracleEBSResponsibilitiesInfo].[IX_OracleEBSResponsibilitiesInfo_ResponsibilityId]");
            //DropForeignKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_Id", "dbo.DI_OEBS_ResponsibilitiesInfo");
            Sql("ALTER TABLE [dbo].[OracleEBSUserInfoOracleEBSResponsibilitiesInfo] DROP CONSTRAINT [FK_dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo_dbo.DI_OEBS_ResponsibilitiesInfo_OracleEBSResponsibilitiesInfo_Responsibil]");
            DropIndex("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", new[] { "OracleEBSResponsibilitiesInfo_Id" });
            DropPrimaryKey("dbo.DI_OEBS_Configuration");
            DropPrimaryKey("dbo.DI_OEBS_HRMSUserInfo");
            DropPrimaryKey("dbo.DI_OEBS_HRMSUserInfoHistory");
            DropPrimaryKey("dbo.DI_OEBS_MenusInfo");
            DropPrimaryKey("dbo.DI_OEBS_ResponsibilitiesInfo");
            DropPrimaryKey("dbo.DI_OEBS_ResponsibilitiesInfoHistory");
            DropPrimaryKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo");
            AlterColumn("dbo.DI_OEBS_Configuration", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_HRMSUserInfo", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_HRMSUserInfoHistory", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_MenusInfo", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_ResponsibilitiesInfo", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_ResponsibilitiesInfoHistory", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_Id", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.DI_OEBS_Configuration", "Id");
            AddPrimaryKey("dbo.DI_OEBS_HRMSUserInfo", "Id");
            AddPrimaryKey("dbo.DI_OEBS_HRMSUserInfoHistory", "Id");
            AddPrimaryKey("dbo.DI_OEBS_MenusInfo", "Id");
            AddPrimaryKey("dbo.DI_OEBS_ResponsibilitiesInfo", "Id");
            AddPrimaryKey("dbo.DI_OEBS_ResponsibilitiesInfoHistory", "Id");
            AddPrimaryKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", new[] { "OracleEBSUserInfo_Id", "OracleEBSResponsibilitiesInfo_Id" });
            CreateIndex("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_Id");
            AddForeignKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_Id", "dbo.DI_OEBS_ResponsibilitiesInfo", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_Id", "dbo.DI_OEBS_ResponsibilitiesInfo");
            DropIndex("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", new[] { "OracleEBSResponsibilitiesInfo_Id" });
            DropPrimaryKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo");
            DropPrimaryKey("dbo.DI_OEBS_ResponsibilitiesInfoHistory");
            DropPrimaryKey("dbo.DI_OEBS_ResponsibilitiesInfo");
            DropPrimaryKey("dbo.DI_OEBS_MenusInfo");
            DropPrimaryKey("dbo.DI_OEBS_HRMSUserInfoHistory");
            DropPrimaryKey("dbo.DI_OEBS_HRMSUserInfo");
            DropPrimaryKey("dbo.DI_OEBS_Configuration");
            AlterColumn("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.DI_OEBS_ResponsibilitiesInfoHistory", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_ResponsibilitiesInfo", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_MenusInfo", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_HRMSUserInfoHistory", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_HRMSUserInfo", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_OEBS_Configuration", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", new[] { "OracleEBSUserInfo_Id", "OracleEBSResponsibilitiesInfo_Id" });
            AddPrimaryKey("dbo.DI_OEBS_ResponsibilitiesInfoHistory", "Id");
            AddPrimaryKey("dbo.DI_OEBS_ResponsibilitiesInfo", "Id");
            AddPrimaryKey("dbo.DI_OEBS_MenusInfo", "Id");
            AddPrimaryKey("dbo.DI_OEBS_HRMSUserInfoHistory", "Id");
            AddPrimaryKey("dbo.DI_OEBS_HRMSUserInfo", "Id");
            AddPrimaryKey("dbo.DI_OEBS_Configuration", "Id");
            CreateIndex("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_Id");
            AddForeignKey("dbo.OracleEBSUserInfoOracleEBSResponsibilitiesInfo", "OracleEBSResponsibilitiesInfo_Id", "dbo.DI_OEBS_ResponsibilitiesInfo", "Id", cascadeDelete: true);
        }
    }
}
