namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NOVACompositeRoleMatrix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_NOVA_CompositeRoleMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        Job = c.String(),
                        CompositeRoleName = c.String(),
                        ModuleWiseRoles = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_NOVA_CompositeRoleMatrix");
        }
    }
}
