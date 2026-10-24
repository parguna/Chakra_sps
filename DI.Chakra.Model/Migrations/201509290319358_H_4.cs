namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class H_4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_SODRunPolicy", "CreatedBy", c => c.String());
            AddColumn("dbo.DI_M_SODRunPolicy", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.DI_M_SODRunPolicy", "LastModifiedBy", c => c.String());
            AddColumn("dbo.DI_M_SODRunPolicy", "LastModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_SODRunPolicy", "LastModifiedDate");
            DropColumn("dbo.DI_M_SODRunPolicy", "LastModifiedBy");
            DropColumn("dbo.DI_M_SODRunPolicy", "CreatedDate");
            DropColumn("dbo.DI_M_SODRunPolicy", "CreatedBy");
        }
    }
}
