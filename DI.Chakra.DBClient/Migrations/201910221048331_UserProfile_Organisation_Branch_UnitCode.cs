namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile_Organisation_Branch_UnitCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_User", "BranchCode", c => c.String());
            AddColumn("dbo.DI_M_User", "UnitCode", c => c.String());
            AddColumn("dbo.DI_M_User", "OrganisationCode", c => c.String());
             
        }
        
        public override void Down()
        { 
            DropColumn("dbo.DI_M_User", "OrganisationCode");
            DropColumn("dbo.DI_M_User", "UnitCode");
            DropColumn("dbo.DI_M_User", "BranchCode");
        }
    }
}
