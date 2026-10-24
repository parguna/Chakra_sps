namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADSConfigHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_ADS_ConfigurationHistory",
                c => new
                    {
                        HistoryId = c.Long(nullable: false, identity: true),
                        ADSConfigurationId = c.Int(nullable: false),
                        IPAddress = c.String(),
                        DomainName = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        Ou = c.String(),
                        MapAll = c.Boolean(),
                        AuditAction = c.String(),
                        AuditUser = c.String(),
                        AuditDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.HistoryId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_ADS_ConfigurationHistory");
        }
    }
}
