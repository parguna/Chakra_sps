namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InhouseFunctions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_INH_Functions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        FunctionName = c.String(),
                        PermissionName = c.String(),
                        InhouseSystemCode = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                        IsValid = c.Boolean(nullable: false),
                        ValidationMessage = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_INH_Functions");
        }
    }
}
