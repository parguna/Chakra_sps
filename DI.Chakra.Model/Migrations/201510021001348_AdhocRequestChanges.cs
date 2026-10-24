namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdhocRequestChanges : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DI_M_AdHocAccessRequest", "IsSecondaryAccountRequest");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AdHocAccessRequest", "IsSecondaryAccountRequest", c => c.Boolean(nullable: false));
        }
    }
}
