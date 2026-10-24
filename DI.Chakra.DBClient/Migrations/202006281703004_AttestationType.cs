namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AttestationType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_Attestation", "AttestationType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_Attestation", "AttestationType");
        }
    }
}
