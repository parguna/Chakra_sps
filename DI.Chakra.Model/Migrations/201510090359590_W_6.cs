namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class W_6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_UserCulturePreference",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CultureName = c.String(),
                        DateTimeFormat = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_UserCulturePreference");
        }
    }
}
