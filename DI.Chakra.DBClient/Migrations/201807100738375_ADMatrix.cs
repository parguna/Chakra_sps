namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ADMatrix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_ADS_XLProvMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CompanyCode = c.String(),
                        DepartmentCode = c.String(),
                        DivisionCode = c.String(),
                        OrganisationId = c.String(),
                        OrganisationName = c.String(),
                        CostCenterCode = c.String(),
                        CostDistributionCode = c.String(),
                        OU = c.String(),
                        DefaultGroups = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_ADS_XLProvMatrix");
        }
    }
}
