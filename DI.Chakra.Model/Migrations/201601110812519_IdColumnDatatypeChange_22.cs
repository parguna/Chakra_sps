namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_22 : DbMigration
    {
        public override void Up()
        {
            Sql("DROP INDEX [dbo].[DI_M_PasswordSyncConfigurationDetail].[IX_ConfigId]");
            Sql("ALTER TABLE [dbo].[DI_M_PasswordSyncConfigurationDetail] DROP CONSTRAINT                               [FK_dbo.DI_M_PasswordSyncConfigurationDetail_dbo.DI_M_PasswordSyncConfiguration_ConfigId]");
            DropIndex("dbo.DI_M_PasswordSyncConfigurationDetail", new[] { "PasswordSyncConfiguration_Id" });
            DropColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId");
            AddColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", c => c.Long(nullable: false));
            DropPrimaryKey("dbo.DI_M_PasswordSyncConfiguration");
            AlterColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_PasswordSyncConfiguration", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_PasswordSyncConfiguration", "Id");
            CreateIndex("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId");
            AddForeignKey("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", "dbo.DI_M_PasswordSyncConfiguration", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", "dbo.DI_M_PasswordSyncConfiguration");
            DropIndex("dbo.DI_M_PasswordSyncConfigurationDetail", new[] { "ConfigId" });
            DropPrimaryKey("dbo.DI_M_PasswordSyncConfiguration");
            AlterColumn("dbo.DI_M_PasswordSyncConfiguration", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", c => c.Int());
            AlterColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.DI_M_PasswordSyncConfiguration", "Id");
            RenameColumn(table: "dbo.DI_M_PasswordSyncConfigurationDetail", name: "ConfigId", newName: "PasswordSyncConfiguration_Id");
            AddColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "ConfigId", c => c.Int(nullable: false));
            CreateIndex("dbo.DI_M_PasswordSyncConfigurationDetail", "PasswordSyncConfiguration_Id");
            AddForeignKey("dbo.DI_M_PasswordSyncConfigurationDetail", "PasswordSyncConfiguration_Id", "dbo.DI_M_PasswordSyncConfiguration", "Id");
        }
    }
}
