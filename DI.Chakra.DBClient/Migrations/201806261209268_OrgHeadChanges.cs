namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrgHeadChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DI_M_NOrganisationHead", new[] { "HeadId" });
            AlterColumn("dbo.DI_M_NOrganisationHead", "HeadId", c => c.Long());
            AlterColumn("dbo.DI_M_NOrganisationHead", "CoordinatorId", c => c.Long());
            CreateIndex("dbo.DI_M_NOrganisationHead", "HeadId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DI_M_NOrganisationHead", new[] { "HeadId" });
            AlterColumn("dbo.DI_M_NOrganisationHead", "CoordinatorId", c => c.Long(nullable: false));
            AlterColumn("dbo.DI_M_NOrganisationHead", "HeadId", c => c.Long(nullable: false));
            CreateIndex("dbo.DI_M_NOrganisationHead", "HeadId");
        }
    }
}
