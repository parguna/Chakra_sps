namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W_5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_SqlConfiguration",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ServerName = c.String(),
                        Database = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        SqlAuthentication = c.Boolean(nullable: false),
                        EndpointSystem_EndpointSystemId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DI_M_EndpointSystem", t => t.EndpointSystem_EndpointSystemId)
                .Index(t => t.EndpointSystem_EndpointSystemId);
            
            AddColumn("dbo.DI_M_EndpointSystem", "UserObjectName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DI_M_SqlConfiguration", "EndpointSystem_EndpointSystemId", "dbo.DI_M_EndpointSystem");
            DropIndex("dbo.DI_M_SqlConfiguration", new[] { "EndpointSystem_EndpointSystemId" });
            DropColumn("dbo.DI_M_EndpointSystem", "UserObjectName");
            DropTable("dbo.DI_M_SqlConfiguration");
        }
    }
}
