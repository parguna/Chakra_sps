namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LINUXXLMatrix_Migrations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_LINUX_XLProvMatrix",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyCode = c.String(),
                        DepartmentCode = c.String(),
                        DivisionCode = c.String(),
                        OrganisationId = c.String(),
                        OrganisationName = c.String(),
                        CostCenterCode = c.String(),
                        CostDistributionCode = c.String(),
                        Groups = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_LINUX_XLProvMatrix");
        }
    }
}
