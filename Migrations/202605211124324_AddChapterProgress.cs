namespace WAPP_Assignment_Module.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChapterProgress : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChapterProgresses",
                c => new
                    {
                        ProgressId = c.Int(nullable: false, identity: true),
                        ChapterId = c.Int(nullable: false),
                        TraineeId = c.String(maxLength: 128),
                        IsCompleted = c.Boolean(nullable: false),
                        CompletedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProgressId)
                .ForeignKey("dbo.Chapters", t => t.ChapterId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.TraineeId)
                .Index(t => t.ChapterId)
                .Index(t => t.TraineeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChapterProgresses", "TraineeId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChapterProgresses", "ChapterId", "dbo.Chapters");
            DropIndex("dbo.ChapterProgresses", new[] { "TraineeId" });
            DropIndex("dbo.ChapterProgresses", new[] { "ChapterId" });
            DropTable("dbo.ChapterProgresses");
        }
    }
}
