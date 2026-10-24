namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserBuisinessRoleMapping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_UserBusinessRoleMapping",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Long(nullable: false),
                        EndpointUserId = c.Long(nullable: false),
                        EndpointSystemId = c.Long(nullable: false),
                        BusinessRole = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_UserBusinessRoleMapping");
        }
    }
}
