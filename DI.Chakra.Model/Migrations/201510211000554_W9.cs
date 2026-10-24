namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_AccountReconcilation", "FullName", c => c.String());
            AddColumn("dbo.DI_M_AccountReconcilation", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.DI_M_AccountReconcilation", "IsLocked", c => c.Boolean(nullable: false));
            AddColumn("dbo.DI_M_AccountReconcilation", "LockScenario", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_AccountReconcilation", "AccountType", c => c.Byte(nullable: false));
            AddColumn("dbo.DI_M_AccountReconcilation", "MappingType", c => c.Byte(nullable: false));
            DropColumn("dbo.DI_M_AccountReconcilation", "EnteredDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DI_M_AccountReconcilation", "EnteredDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.DI_M_AccountReconcilation", "MappingType");
            DropColumn("dbo.DI_M_AccountReconcilation", "AccountType");
            DropColumn("dbo.DI_M_AccountReconcilation", "LockScenario");
            DropColumn("dbo.DI_M_AccountReconcilation", "IsLocked");
            DropColumn("dbo.DI_M_AccountReconcilation", "CreatedDate");
            DropColumn("dbo.DI_M_AccountReconcilation", "FullName");
        }
    }
}
