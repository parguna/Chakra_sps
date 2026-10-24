namespace DI.Chakra.DBClient.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_EAMNovaHangingRoleInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DI_M_EAMNovaHangingRoleInfo",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EndPointSystemId = c.Long(nullable: false),
                        AccountName = c.String(maxLength: 200),
                        AccountType = c.Byte(nullable: false),
                        ADGroups = c.String(),
                        FLINSRoles = c.String(),
                        BIRoles = c.String(),
                        Reserved1 = c.String(),
                        Reserved2 = c.String(),
                        Reserved3 = c.String(),
                        Reserved4 = c.String(),
                        Reserved5 = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        LastModifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DI_M_EAMNovaHangingRoleInfo");
        }
    }
}
