namespace DI.Chakra.DBClient.Migrations
{
    using Infrastructure.Enums;
    using System;
    using System.Data.Entity.Migrations;

    public partial class UndoEventChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_OUChangeSnapshot", "AccountType", c => c.Byte(nullable: false, defaultValue: (byte)AccountType.User));
            AddColumn("dbo.DI_M_OUChangeSnapshot", "AdditionalInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_OUChangeSnapshot", "AdditionalInfo");
            DropColumn("dbo.DI_M_OUChangeSnapshot", "AccountType");
        }
    }
}
