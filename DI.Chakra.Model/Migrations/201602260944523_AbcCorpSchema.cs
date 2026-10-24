namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AbcCorpSchema : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.DI_M_SODException", "SODRunPolicy_Id", "dbo.DI_M_SODRunPolicy");
            //DropIndex("dbo.DI_M_SODException", new[] { "SODRunPolicy_Id" });
            CreateTable(
                "dbo.DI_ABC_Configuration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndpointSystemId = c.Long(),
                        WebAPI = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystemId)
                .Index(t => t.EndpointSystemId);
            
            //DropColumn("dbo.DI_M_SODException", "SODRunPolicy_Id");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.DI_M_SODException", "SODRunPolicy_Id", c => c.Long());
            DropForeignKey("dbo.DI_ABC_Configuration", "EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_ABC_Configuration", new[] { "EndpointSystemId" });
            DropTable("dbo.DI_ABC_Configuration");
            //CreateIndex("dbo.DI_M_SODException", "SODRunPolicy_Id");
            //AddForeignKey("dbo.DI_M_SODException", "SODRunPolicy_Id", "dbo.DI_M_SODRunPolicy", "Id");
        }
    }
}
