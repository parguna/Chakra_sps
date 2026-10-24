namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InhouseUserPermissionFunction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_INH_UserPermissionFunction",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        InhouseSystemCode = c.String(),
                        EmployeeId = c.String(),
                        NRIC = c.String(),
                        UserName = c.String(),
                        Permission = c.String(),
                        Functions = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_INH_UserPermissionFunction");
        }
    }
}
