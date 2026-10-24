namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttestationUserComparision : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_AttestationSnapshot", "Name", c => c.String(maxLength: 1024));
            AlterColumn("dbo.DI_M_AttestationSnapshot", "AccountName", c => c.String(maxLength: 1024));
            AlterColumn("dbo.DI_M_AttestationSnapshot", "DIUserName", c => c.String(maxLength: 1024));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_AttestationSnapshot", "DIUserName", c => c.String());
            AlterColumn("dbo.DI_M_AttestationSnapshot", "AccountName", c => c.String());
            AlterColumn("dbo.DI_M_AttestationSnapshot", "Name", c => c.String());
        }
    }
}
