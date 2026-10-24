namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserCulPrefence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_UserCulturePreference", "DateFormat", c => c.String());
            AddColumn("dbo.DI_M_UserCulturePreference", "TimeFormat", c => c.String());
            DropColumn("dbo.DI_M_UserCulturePreference", "DateTimeFormat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_UserCulturePreference", "DateTimeFormat", c => c.String());
            DropColumn("dbo.DI_M_UserCulturePreference", "TimeFormat");
            DropColumn("dbo.DI_M_UserCulturePreference", "DateFormat");
        }
    }
}
