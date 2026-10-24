namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SAPSRMEnablerRoleMatrix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SAP_SRMEnablerRoleMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SRMEnablerRule = c.String(),
                        POrg = c.String(),
                        PGrp = c.String(),
                        SRMTechRole = c.String(),
                        BIPOrgRole = c.String(),
                        BIPGrpRole = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SAP_SRMEnablerRoleMatrix");
        }
    }
}
