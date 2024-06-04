namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        BirthYear = c.Int(nullable: false),
                        DeathYear = c.Int(nullable: false),
                        ArtMovement = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Galleries",
                c => new
                    {
                        PIB = c.String(nullable: false, maxLength: 128),
                        Address = c.String(),
                        MBR = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        IsInEditingMode = c.Boolean(nullable: false),
                        GalleryIsEdditedBy = c.String(),
                    })
                .PrimaryKey(t => t.PIB);
            
            CreateTable(
                "dbo.WorkOfArts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArtName = c.String(),
                        ArtMovement = c.Int(nullable: false),
                        Style = c.Int(nullable: false),
                        AuthorID = c.Int(nullable: false),
                        AuthorName = c.String(),
                        GalleryPIB = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Galleries", t => t.GalleryPIB)
                .Index(t => t.GalleryPIB);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Username = c.String(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        UserType = c.Int(nullable: false),
                        PasswordHash = c.String(nullable: false),
                        IsLoggedIn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkOfArts", "GalleryPIB", "dbo.Galleries");
            DropIndex("dbo.WorkOfArts", new[] { "GalleryPIB" });
            DropTable("dbo.Users");
            DropTable("dbo.WorkOfArts");
            DropTable("dbo.Galleries");
            DropTable("dbo.Authors");
        }
    }
}
