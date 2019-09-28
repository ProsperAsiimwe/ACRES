namespace ACRES.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        ActivityId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ArticleId = c.Int(),
                        NewsletterId = c.Int(),
                        MembershipId = c.Int(),
                        PublicationId = c.Int(),
                        Title = c.String(maxLength: 128),
                        Description = c.String(),
                        Recorded = c.DateTime(nullable: false),
                        RecordedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("dbo.AspNetUsers", t => t.RecordedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RecordedById);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DisplayId = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 4),
                        FirstName = c.String(maxLength: 60),
                        LastName = c.String(maxLength: 50),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.References",
                c => new
                    {
                        ReferenceId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Type = c.Byte(nullable: false),
                        Capacity = c.String(maxLength: 80),
                        DateFrom = c.DateTime(),
                        DateTo = c.DateTime(),
                        HonestIntegrity = c.Byte(nullable: false),
                        Performance_Cooperative = c.Byte(nullable: false),
                        Performance_UnderPressure = c.Byte(nullable: false),
                        Performance_Flexible = c.Byte(nullable: false),
                        Performance_Punctual = c.Byte(nullable: false),
                        Performance_Competent = c.Byte(nullable: false),
                        Performance_WorkSatisfactory = c.Byte(nullable: false),
                        Employment_Terminated = c.Byte(nullable: false),
                        Employment_WouldRemploy = c.Boolean(),
                        Education_CompleteCourse = c.Boolean(),
                        Comments = c.String(maxLength: 1024),
                        Sections = c.String(maxLength: 60),
                        Notified = c.DateTime(),
                        RemindDate = c.DateTime(),
                        RemindCount = c.Int(nullable: false),
                        Updated = c.DateTime(nullable: false),
                        Started = c.DateTime(),
                        Submitted = c.DateTime(),
                        Doc = c.String(maxLength: 128),
                        AuthCode = c.String(maxLength: 12),
                        OptedOutDate = c.DateTime(),
                        MarketingDate = c.DateTime(),
                        Title = c.String(maxLength: 4),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 60),
                        Email = c.String(maxLength: 80),
                        PhoneNumber = c.String(maxLength: 20),
                        Mobile = c.String(maxLength: 20),
                        Organisation = c.String(maxLength: 80),
                        JobTitle = c.String(maxLength: 80),
                        Line1 = c.String(maxLength: 50),
                        Line2 = c.String(maxLength: 50),
                        Line3 = c.String(maxLength: 50),
                        Line4 = c.String(maxLength: 50),
                        Line5 = c.String(maxLength: 50),
                        PostCode = c.String(maxLength: 12),
                        CountryId = c.Int(),
                    })
                .PrimaryKey(t => t.ReferenceId)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        CountryName = c.String(nullable: false, maxLength: 80),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        ArticleId = c.Int(nullable: false, identity: true),
                        Author = c.String(maxLength: 1000),
                        Published = c.DateTime(nullable: false),
                        Title = c.String(maxLength: 100),
                        Description = c.String(),
                        UrlToImage = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ArticleId);
            
            CreateTable(
                "dbo.BlogPosts",
                c => new
                    {
                        BlogPostId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        Author = c.String(nullable: false, maxLength: 50),
                        Content = c.String(nullable: false),
                        FileName = c.String(maxLength: 1000),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BlogPostId);
            
            CreateTable(
                "dbo.ContactMessages",
                c => new
                    {
                        ContactMessageId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50),
                        Subject = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 50),
                        Message = c.String(maxLength: 2000),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ContactMessageId);
            
            CreateTable(
                "dbo.GalleryUploads",
                c => new
                    {
                        GalleryUploadId = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 50),
                        FileName = c.String(maxLength: 1000),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GalleryUploadId);
            
            CreateTable(
                "dbo.Inquiries",
                c => new
                    {
                        InquiryId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Name = c.String(maxLength: 100),
                        Email = c.String(maxLength: 100),
                        Company = c.String(maxLength: 100),
                        Subject = c.String(maxLength: 100),
                        Message = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.InquiryId);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userName = c.String(),
                        usageKind = c.String(),
                        usageDetails = c.String(),
                        userAddress = c.String(),
                        userHost = c.String(),
                        aluTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Newsletters",
                c => new
                    {
                        NewsletterId = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        News = c.String(),
                        Added = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NewsletterId);
            
            CreateTable(
                "dbo.DownloadItems",
                c => new
                    {
                        PublicationId = c.Int(nullable: false),
                        DownloadId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PublicationId, t.DownloadId })
                .ForeignKey("dbo.Downloads", t => t.DownloadId, cascadeDelete: true)
                .ForeignKey("dbo.Publications", t => t.PublicationId, cascadeDelete: true)
                .Index(t => t.PublicationId)
                .Index(t => t.DownloadId);
            
            CreateTable(
                "dbo.Downloads",
                c => new
                    {
                        DownloadId = c.Int(nullable: false, identity: true),
                        ClientId = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DownloadId)
                .ForeignKey("dbo.AspNetUsers", t => t.ClientId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Publications",
                c => new
                    {
                        PublicationId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Title = c.String(maxLength: 100),
                        Description = c.String(maxLength: 1000),
                        SubjectMatter = c.String(maxLength: 100),
                        Doc = c.String(maxLength: 100),
                        SplitPage = c.Int(nullable: false),
                        Submitted = c.DateTime(nullable: false),
                        Deleted = c.DateTime(),
                    })
                .PrimaryKey(t => t.PublicationId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Subscribers",
                c => new
                    {
                        SubscriberId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        Email = c.String(maxLength: 200),
                        Added = c.DateTime(nullable: false),
                        Cancelled = c.DateTime(),
                    })
                .PrimaryKey(t => t.SubscriberId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DownloadItems", "PublicationId", "dbo.Publications");
            DropForeignKey("dbo.DownloadItems", "DownloadId", "dbo.Downloads");
            DropForeignKey("dbo.Downloads", "ClientId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Activities", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Activities", "RecordedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.References", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.References", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Downloads", new[] { "ClientId" });
            DropIndex("dbo.DownloadItems", new[] { "DownloadId" });
            DropIndex("dbo.DownloadItems", new[] { "PublicationId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.References", new[] { "CountryId" });
            DropIndex("dbo.References", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Activities", new[] { "RecordedById" });
            DropIndex("dbo.Activities", new[] { "UserId" });
            DropTable("dbo.Subscribers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Publications");
            DropTable("dbo.Downloads");
            DropTable("dbo.DownloadItems");
            DropTable("dbo.Newsletters");
            DropTable("dbo.Logs");
            DropTable("dbo.Inquiries");
            DropTable("dbo.GalleryUploads");
            DropTable("dbo.ContactMessages");
            DropTable("dbo.BlogPosts");
            DropTable("dbo.Articles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Countries");
            DropTable("dbo.References");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Activities");
        }
    }
}
