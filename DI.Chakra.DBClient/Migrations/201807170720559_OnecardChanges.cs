namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OnecardChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_SMS_XLProvMatrix",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PostalCode = c.String(),
                        Location = c.String(),
                        DepartmentCode = c.String(),
                        DepartmentName = c.String(),
                        DivisionCode = c.String(),
                        DivisionName = c.String(),
                        DefaultGroups = c.String(),
                        ValidFrom = c.DateTime(),
                        ValidTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_SMS_XLProvMatrix");
        }
    }
}
