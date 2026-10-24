namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_10 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_AccountCategory");
            AlterColumn("dbo.DI_M_AccountCategory", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_AccountCategory", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_AccountCategory");
            AlterColumn("dbo.DI_M_AccountCategory", "Id", c => c.Byte(nullable: false));
            AddPrimaryKey("dbo.DI_M_AccountCategory", "Id");
        }
    }
}
