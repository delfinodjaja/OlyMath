namespace OlyMath.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssessmentResults",
                c => new
                    {
                        ResultId = c.Int(nullable: false, identity: true),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AttemptDate = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        AssessmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResultId)
                .ForeignKey("dbo.Assessments", t => t.AssessmentId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AssessmentId);
            
            CreateTable(
                "dbo.Assessments",
                c => new
                    {
                        AssessmentId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        TotalMarks = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssessmentId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Category = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        TrainerId = c.Int(nullable: false),
                        Trainer_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.AspNetUsers", t => t.Trainer_Id)
                .Index(t => t.Trainer_Id);
            
            CreateTable(
                "dbo.Certificates",
                c => new
                    {
                        CertificateId = c.Int(nullable: false, identity: true),
                        IssuedDate = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CertificateId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Discussions",
                c => new
                    {
                        DiscussionId = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiscussionId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Enrollments",
                c => new
                    {
                        EnrollmentId = c.Int(nullable: false, identity: true),
                        EnrolledAt = c.DateTime(nullable: false),
                        ProgressStatus = c.String(),
                        UserId = c.String(maxLength: 128),
                        ModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EnrollmentId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        MaterialId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Type = c.String(),
                        ContentUrl = c.String(),
                        ModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaterialId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssessmentResults", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AssessmentResults", "AssessmentId", "dbo.Assessments");
            DropForeignKey("dbo.Modules", "Trainer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Materials", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Enrollments", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Enrollments", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Discussions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Discussions", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Certificates", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Certificates", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Assessments", "ModuleId", "dbo.Modules");
            DropIndex("dbo.Materials", new[] { "ModuleId" });
            DropIndex("dbo.Enrollments", new[] { "ModuleId" });
            DropIndex("dbo.Enrollments", new[] { "UserId" });
            DropIndex("dbo.Discussions", new[] { "ModuleId" });
            DropIndex("dbo.Discussions", new[] { "UserId" });
            DropIndex("dbo.Certificates", new[] { "ModuleId" });
            DropIndex("dbo.Certificates", new[] { "UserId" });
            DropIndex("dbo.Modules", new[] { "Trainer_Id" });
            DropIndex("dbo.Assessments", new[] { "ModuleId" });
            DropIndex("dbo.AssessmentResults", new[] { "AssessmentId" });
            DropIndex("dbo.AssessmentResults", new[] { "UserId" });
            DropTable("dbo.Materials");
            DropTable("dbo.Enrollments");
            DropTable("dbo.Discussions");
            DropTable("dbo.Certificates");
            DropTable("dbo.Modules");
            DropTable("dbo.Assessments");
            DropTable("dbo.AssessmentResults");
        }
    }
}
