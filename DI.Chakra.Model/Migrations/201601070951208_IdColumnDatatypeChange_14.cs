namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_14 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_UserCulturePreference");
            AlterColumn("dbo.DI_M_UserCulturePreference", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_UserCulturePreference", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_UserCulturePreference");
            AlterColumn("dbo.DI_M_UserCulturePreference", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_UserCulturePreference", "Id");
        }
    }
}
