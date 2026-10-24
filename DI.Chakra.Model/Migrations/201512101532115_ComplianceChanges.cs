namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComplianceChanges : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DI_M_SODRiskCode", "RiskLevelId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DI_M_SODRiskCode", "RiskLevelId", c => c.Int(nullable: false));
        }
    }
}
