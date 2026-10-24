namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_17 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_EmailHistory");
            DropPrimaryKey("dbo.DI_M_Email");
            AlterColumn("dbo.DI_M_EmailHistory", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_Email", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_EmailHistory", "Id");
            AddPrimaryKey("dbo.DI_M_Email", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_Email");
            DropPrimaryKey("dbo.DI_M_EmailHistory");
            AlterColumn("dbo.DI_M_Email", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.DI_M_EmailHistory", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_Email", "Id");
            AddPrimaryKey("dbo.DI_M_EmailHistory", "Id");
        }
    }
}
