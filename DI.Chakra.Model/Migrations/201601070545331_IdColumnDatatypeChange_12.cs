namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_12 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_AccessRequestCategory");
            DropPrimaryKey("dbo.DI_M_AccountActionCategory");
            AlterColumn("dbo.DI_M_AccessRequestCategory", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_AccountActionCategory", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_AccessRequestCategory", "Id");
            AddPrimaryKey("dbo.DI_M_AccountActionCategory", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_AccountActionCategory");
            DropPrimaryKey("dbo.DI_M_AccessRequestCategory");
            AlterColumn("dbo.DI_M_AccountActionCategory", "Id", c => c.Byte(nullable: false));
            AlterColumn("dbo.DI_M_AccessRequestCategory", "Id", c => c.Byte(nullable: false));
            AddPrimaryKey("dbo.DI_M_AccountActionCategory", "Id");
            AddPrimaryKey("dbo.DI_M_AccessRequestCategory", "Id");
        }
    }
}
