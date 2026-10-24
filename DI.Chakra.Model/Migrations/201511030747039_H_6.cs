namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_EndpointFunctionType",
                c => new
                    {
                        EndpointFunctionTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        EndpointSystemTypeId = c.Int(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.EndpointFunctionTypeId)
                .ForeignKey("dbo.DI_M_EndpointSystemType", t => t.EndpointSystemTypeId)
                .Index(t => t.EndpointSystemTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_EndpointFunctionType", "EndpointSystemTypeId", "dbo.DI_M_EndpointSystemType");
            DropIndex("dbo.DI_M_EndpointFunctionType", new[] { "EndpointSystemTypeId" });
            DropTable("dbo.DI_M_EndpointFunctionType");
        }
    }
}
