-- ============================================================
--  OlyMath - Assessment Module Tables
--  Run this script against your OlyMathDB database
-- ============================================================

USE OlyMathDB;
GO

-- ------------------------------------------------------------
--  1. Assessments
-- ------------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'Assessments'
)
BEGIN
    CREATE TABLE Assessments (
        AssessmentID   INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Title          NVARCHAR(200)  NOT NULL,
        Description    NVARCHAR(1000) NULL,
        PassingScore   INT            NOT NULL DEFAULT 60,   -- percentage
        CreatedBy      NVARCHAR(256)  NOT NULL,              -- username
        CreatedDate    DATETIME       NOT NULL DEFAULT GETDATE(),
        IsActive       BIT            NOT NULL DEFAULT 1
    );
    PRINT 'Assessments table created.';
END
ELSE
BEGIN
    PRINT 'Assessments table already exists.';
END
GO

-- ------------------------------------------------------------
--  2. AssessmentQuestions
-- ------------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'AssessmentQuestions'
)
BEGIN
    CREATE TABLE AssessmentQuestions (
        QuestionID     INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        AssessmentID   INT            NOT NULL,
        QuestionText   NVARCHAR(2000) NOT NULL,
        OptionA        NVARCHAR(500)  NOT NULL,
        OptionB        NVARCHAR(500)  NOT NULL,
        OptionC        NVARCHAR(500)  NOT NULL,
        OptionD        NVARCHAR(500)  NOT NULL,
        CorrectAnswer  CHAR(1)        NOT NULL,   -- 'A','B','C', or 'D'
        DisplayOrder   INT            NOT NULL DEFAULT 0,

        CONSTRAINT FK_Questions_Assessment
            FOREIGN KEY (AssessmentID)
            REFERENCES Assessments(AssessmentID)
            ON DELETE CASCADE
    );
    PRINT 'AssessmentQuestions table created.';
END
ELSE
BEGIN
    PRINT 'AssessmentQuestions table already exists.';
END
GO

-- ------------------------------------------------------------
--  3. AssessmentAttempts
-- ------------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'AssessmentAttempts'
)
BEGIN
    CREATE TABLE AssessmentAttempts (
        AttemptID      INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        AssessmentID   INT            NOT NULL,
        TraineeUsername NVARCHAR(256) NOT NULL,
        Score          INT            NOT NULL DEFAULT 0,    -- percentage
        IsPassed       BIT            NOT NULL DEFAULT 0,
        AttemptDate    DATETIME       NOT NULL DEFAULT GETDATE(),

        CONSTRAINT FK_Attempts_Assessment
            FOREIGN KEY (AssessmentID)
            REFERENCES Assessments(AssessmentID)
    );
    PRINT 'AssessmentAttempts table created.';
END
ELSE
BEGIN
    PRINT 'AssessmentAttempts table already exists.';
END
GO

-- ------------------------------------------------------------
--  4. Certificates
-- ------------------------------------------------------------
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'Certificates'
)
BEGIN
    CREATE TABLE Certificates (
        CertificateID  INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        AttemptID      INT            NOT NULL,
        AssessmentID   INT            NOT NULL,
        TraineeName    NVARCHAR(256)  NOT NULL,
        AssessmentTitle NVARCHAR(200) NOT NULL,
        Score          INT            NOT NULL,
        IssueDate      DATETIME       NOT NULL DEFAULT GETDATE(),

        CONSTRAINT FK_Certificates_Attempt
            FOREIGN KEY (AttemptID)
            REFERENCES AssessmentAttempts(AttemptID),

        CONSTRAINT FK_Certificates_Assessment
            FOREIGN KEY (AssessmentID)
            REFERENCES Assessments(AssessmentID)
    );
    PRINT 'Certificates table created.';
END
ELSE
BEGIN
    PRINT 'Certificates table already exists.';
END
GO

PRINT '=== All assessment tables are ready. ===';
GO
