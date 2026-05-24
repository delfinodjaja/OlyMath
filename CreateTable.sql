-- =============================================
-- OlyMath Database - Full Schema
-- Run this in SSMS against your OlyMathDB
-- Safe to re-run: all tables use IF NOT EXISTS
-- =============================================

USE OlyMathDB;
GO

-- =============================================
-- 1. Modules
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Modules')
BEGIN
    CREATE TABLE Modules (
        ModuleId    INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Title       NVARCHAR(200)  NOT NULL,
        Description NVARCHAR(MAX)  NULL,
        Category    NVARCHAR(100)  NULL,
        CreatedAt   DATETIME       NOT NULL DEFAULT GETDATE(),
        TrainerId   NVARCHAR(450)  NULL
    );
    PRINT 'Modules table created.';
END ELSE PRINT 'Modules table already exists.';
GO

-- =============================================
-- 2. Enrollments
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Enrollments')
BEGIN
    CREATE TABLE Enrollments (
        EnrollmentId    INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        EnrolledAt      DATETIME       NOT NULL DEFAULT GETDATE(),
        ProgressStatus  NVARCHAR(50)   NOT NULL DEFAULT 'Not Started',
        UserId          NVARCHAR(450)  NOT NULL,
        ModuleId        INT            NULL,
        CONSTRAINT FK_Enrollments_Module FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId)
    );
    PRINT 'Enrollments table created.';
END ELSE PRINT 'Enrollments table already exists.';
GO

-- =============================================
-- 3. Materials
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Materials')
BEGIN
    CREATE TABLE Materials (
        MaterialId  INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Title       NVARCHAR(200)  NOT NULL,
        Type        NVARCHAR(50)   NULL,
        ContentUrl  NVARCHAR(500)  NULL,
        ModuleId    INT            NULL,
        CONSTRAINT FK_Materials_Module FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId)
    );
    PRINT 'Materials table created.';
END ELSE PRINT 'Materials table already exists.';
GO

-- =============================================
-- 4. Assessments
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Assessments')
BEGIN
    CREATE TABLE Assessments (
        AssessmentID   INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Title          NVARCHAR(200)  NOT NULL,
        Description    NVARCHAR(1000) NULL,
        PassingScore   INT            NOT NULL DEFAULT 60,
        CreatedBy      NVARCHAR(256)  NOT NULL,
        CreatedDate    DATETIME       NOT NULL DEFAULT GETDATE(),
        IsActive       BIT            NOT NULL DEFAULT 1
    );
    PRINT 'Assessments table created.';
END ELSE PRINT 'Assessments table already exists.';
GO

-- =============================================
-- 5. AssessmentQuestions
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AssessmentQuestions')
BEGIN
    CREATE TABLE AssessmentQuestions (
        QuestionID     INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        AssessmentID   INT            NOT NULL,
        QuestionText   NVARCHAR(2000) NOT NULL,
        OptionA        NVARCHAR(500)  NOT NULL,
        OptionB        NVARCHAR(500)  NOT NULL,
        OptionC        NVARCHAR(500)  NOT NULL,
        OptionD        NVARCHAR(500)  NOT NULL,
        CorrectAnswer  CHAR(1)        NOT NULL,
        DisplayOrder   INT            NOT NULL DEFAULT 0,
        CONSTRAINT FK_Questions_Assessment
            FOREIGN KEY (AssessmentID) REFERENCES Assessments(AssessmentID) ON DELETE CASCADE
    );
    PRINT 'AssessmentQuestions table created.';
END ELSE PRINT 'AssessmentQuestions table already exists.';
GO

-- =============================================
-- 6. AssessmentAttempts
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AssessmentAttempts')
BEGIN
    CREATE TABLE AssessmentAttempts (
        AttemptID       INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        AssessmentID    INT            NOT NULL,
        TraineeUsername NVARCHAR(256)  NOT NULL,
        Score           INT            NOT NULL DEFAULT 0,
        IsPassed        BIT            NOT NULL DEFAULT 0,
        AttemptDate     DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Attempts_Assessment
            FOREIGN KEY (AssessmentID) REFERENCES Assessments(AssessmentID)
    );
    PRINT 'AssessmentAttempts table created.';
END ELSE PRINT 'AssessmentAttempts table already exists.';
GO

-- =============================================
-- 7. Certificates
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Certificates')
BEGIN
    CREATE TABLE Certificates (
        CertificateID   INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        AttemptID       INT            NOT NULL,
        AssessmentID    INT            NOT NULL,
        TraineeName     NVARCHAR(256)  NOT NULL,
        AssessmentTitle NVARCHAR(200)  NOT NULL,
        Score           INT            NOT NULL,
        IssueDate       DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Certificates_Attempt
            FOREIGN KEY (AttemptID) REFERENCES AssessmentAttempts(AttemptID),
        CONSTRAINT FK_Certificates_Assessment
            FOREIGN KEY (AssessmentID) REFERENCES Assessments(AssessmentID)
    );
    PRINT 'Certificates table created.';
END ELSE PRINT 'Certificates table already exists.';
GO

-- =============================================
-- 8. Discussions
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Discussions')
BEGIN
    CREATE TABLE Discussions (
        DiscussionId  INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Title         NVARCHAR(300)  NOT NULL,
        Content       NVARCHAR(MAX)  NOT NULL,
        CreatedAt     DATETIME       NOT NULL DEFAULT GETDATE(),
        UserId        NVARCHAR(450)  NOT NULL,
        [Like]        INT            NOT NULL DEFAULT 0,
        IsFlagged     BIT            NOT NULL DEFAULT 0,
        ModuleId      INT            NULL,
        CONSTRAINT FK_Discussions_Module
            FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId)
    );
    PRINT 'Discussions table created.';
END ELSE PRINT 'Discussions table already exists.';
GO

-- =============================================
-- 9. DiscussionPosts
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DiscussionPosts')
BEGIN
    CREATE TABLE DiscussionPosts (
        PostId        INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        DiscussionId  INT            NOT NULL,
        Content       NVARCHAR(MAX)  NOT NULL,
        CreatedAt     DATETIME       NOT NULL DEFAULT GETDATE(),
        UserId        NVARCHAR(450)  NOT NULL,
        CONSTRAINT FK_Posts_Discussion
            FOREIGN KEY (DiscussionId) REFERENCES Discussions(DiscussionId) ON DELETE CASCADE
    );
    PRINT 'DiscussionPosts table created.';
END ELSE PRINT 'DiscussionPosts table already exists.';
GO

-- =============================================
-- 10. SystemLogs
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SystemLogs')
BEGIN
    CREATE TABLE SystemLogs (
        Id          INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Action      NVARCHAR(255)  NOT NULL,
        Timestamp   DATETIME       NOT NULL DEFAULT GETDATE(),
        UserId      NVARCHAR(450)  NULL
    );
    PRINT 'SystemLogs table created.';
END ELSE PRINT 'SystemLogs table already exists.';
GO

-- =============================================
-- 11. AssessmentResults (EF model)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AssessmentResults')
BEGIN
    CREATE TABLE AssessmentResults (
        ResultId        INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Score           DECIMAL(5,2)   NOT NULL,
        AttemptDate     DATETIME       NOT NULL DEFAULT GETDATE(),
        UserId          NVARCHAR(450)  NOT NULL,
        AssessmentId    INT            NOT NULL,
        CONSTRAINT FK_AssessmentResults_Assessment
            FOREIGN KEY (AssessmentId) REFERENCES Assessments(AssessmentID)
    );
    PRINT 'AssessmentResults table created.';
END ELSE PRINT 'AssessmentResults table already exists.';
GO

-- =============================================
-- 12. Chapters
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Chapters')
BEGIN
    CREATE TABLE Chapters (
        ChapterId       INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Title           NVARCHAR(200)  NOT NULL,
        Description     NVARCHAR(MAX)  NULL,
        DisplayOrder    INT            NOT NULL DEFAULT 0,
        HasAssessment   BIT            NOT NULL DEFAULT 0,
        ModuleId        INT            NOT NULL,
        CONSTRAINT FK_Chapters_Module FOREIGN KEY (ModuleId) REFERENCES Modules(ModuleId)
    );
    PRINT 'Chapters table created.';
END ELSE PRINT 'Chapters table already exists.';
GO

-- =============================================
-- 13. ChapterProgress
-- =============================================
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ChapterProgresses')
BEGIN
    CREATE TABLE ChapterProgresses (
        Id              INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        UserId          NVARCHAR(450)  NOT NULL,
        ChapterId       INT            NOT NULL,
        IsCompleted     BIT            NOT NULL DEFAULT 0,
        CompletedAt     DATETIME       NULL,
        CONSTRAINT FK_ChapterProgress_Chapter FOREIGN KEY (ChapterId) REFERENCES Chapters(ChapterId)
    );
    PRINT 'ChapterProgresses table created.';
END ELSE PRINT 'ChapterProgresses table already exists.';
GO

PRINT '=== OlyMathDB full schema is ready. ===';
GO