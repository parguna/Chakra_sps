namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class w13 : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.DI_M_AccessRequest", "IsRequestedToRemove");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.DI_M_AccessRequest", "IsRequestedToRemove", c => c.Boolean());
        }
    }
}
