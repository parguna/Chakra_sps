namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserModelChanges : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE DI_M_User SET IsActive = 1 WHERE IsActive is null");
            AlterColumn("dbo.DI_M_User", "IsActive", c => c.Boolean(nullable: false, defaultValue:true));
            DropColumn("dbo.DI_M_User", "UnitCode");
            DropColumn("dbo.DI_M_User", "UnitName");

            
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_User", "UnitName", c => c.String());
            AddColumn("dbo.DI_M_User", "UnitCode", c => c.String());
            AlterColumn("dbo.DI_M_User", "IsActive", c => c.Boolean());
        }
    }
}
