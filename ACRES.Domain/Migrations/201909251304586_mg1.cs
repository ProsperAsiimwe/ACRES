namespace ACRES.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mg1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activities", "DownloadId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Activities", "DownloadId");
        }
    }
}
