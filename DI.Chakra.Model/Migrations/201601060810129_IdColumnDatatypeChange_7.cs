namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdColumnDatatypeChange_7 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.DI_M_MailTemplate");
            AlterColumn("dbo.DI_M_MailTemplate", "Id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_MailTemplate", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.DI_M_MailTemplate");
            AlterColumn("dbo.DI_M_MailTemplate", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.DI_M_MailTemplate", "Id");
        }
    }
}
