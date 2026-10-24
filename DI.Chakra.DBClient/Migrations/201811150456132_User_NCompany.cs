namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_NCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_User", "NCompany", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_User", "NCompany");
        }
    }
}
