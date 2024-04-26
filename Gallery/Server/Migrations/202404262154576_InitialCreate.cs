namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
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
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Galleries",
                c => new
                    {
                        PIB = c.String(nullable: false, maxLength: 128),
                        Address = c.String(),
                        MBR = c.String(),
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
                        Gallery_PIB = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Galleries", t => t.Gallery_PIB)
                .Index(t => t.Gallery_PIB);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Username = c.String(nullable: false),
                        UserType = c.Int(nullable: false),
                        PasswordHash = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkOfArts", "Gallery_PIB", "dbo.Galleries");
            DropIndex("dbo.WorkOfArts", new[] { "Gallery_PIB" });
            DropTable("dbo.Users");
            DropTable("dbo.WorkOfArts");
            DropTable("dbo.Galleries");
            DropTable("dbo.Authors");
        }
    }
}
