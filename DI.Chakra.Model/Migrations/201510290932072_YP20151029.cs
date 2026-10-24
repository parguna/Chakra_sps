namespace DI.Chakra.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YP20151029 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DI_M_AttestationProcess", "ProcessName", c => c.String());
            AddColumn("dbo.DI_M_AttestationProcess", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DI_M_AttestationProcess", "Description");
            DropColumn("dbo.DI_M_AttestationProcess", "ProcessName");
        }
    }
}
