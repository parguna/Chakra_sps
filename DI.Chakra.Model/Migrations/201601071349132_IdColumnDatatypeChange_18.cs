namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_18 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_PasswordSyncConfigurationDetail");
            AlterColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_PasswordSyncConfigurationDetail", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_PasswordSyncConfigurationDetail");
            AlterColumn("dbo.DI_M_PasswordSyncConfigurationDetail", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_PasswordSyncConfigurationDetail", "Id");
        }
    }
}
