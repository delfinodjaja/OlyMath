-- =============================================
-- OlyMath Database Schema
-- Run this in SSMS against your OlyMathDB
-- =============================================

USE OlyMathDB;
GO

-- =============================================
-- 1. USER
-- =============================================
CREATE TABLE [User] (
    user_id     INT IDENTITY(1,1) PRIMARY KEY,
    name        NVARCHAR(100)   NOT NULL,
    email       NVARCHAR(255)   NOT NULL UNIQUE,
    password    NVARCHAR(255)   NOT NULL,
    role        NVARCHAR(50)    NOT NULL,   -- 'Admin', 'Trainer', 'Trainee'
    created_at  DATETIME        NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================
-- 2. MODULE
-- =============================================
CREATE TABLE Module (
    module_id   INT IDENTITY(1,1) PRIMARY KEY,
    title       NVARCHAR(200)   NOT NULL,
    description NVARCHAR(MAX),
    category    NVARCHAR(100),
    created_at  DATETIME        NOT NULL DEFAULT GETDATE(),
    trainer_id  INT             NOT NULL,
    CONSTRAINT FK_Module_Trainer FOREIGN KEY (trainer_id) REFERENCES [User](user_id)
);
GO

-- =============================================
-- 3. ENROLLMENT
-- =============================================
CREATE TABLE Enrollment (
    enrollment_id       INT IDENTITY(1,1) PRIMARY KEY,
    enrolled_at         DATETIME        NOT NULL DEFAULT GETDATE(),
    progress_status     NVARCHAR(50)    NOT NULL DEFAULT 'Not Started',  -- 'Not Started', 'In Progress', 'Completed'
    user_id             INT             NOT NULL,
    module_id           INT             NOT NULL,
    CONSTRAINT FK_Enrollment_User   FOREIGN KEY (user_id)   REFERENCES [User](user_id),
    CONSTRAINT FK_Enrollment_Module FOREIGN KEY (module_id) REFERENCES Module(module_id)
);
GO

-- =============================================
-- 4. MATERIAL
-- =============================================
CREATE TABLE Material (
    material_id INT IDENTITY(1,1) PRIMARY KEY,
    title       NVARCHAR(200)   NOT NULL,
    type        NVARCHAR(50),               -- 'Video', 'PDF', 'Article', etc.
    content_url NVARCHAR(500),
    module_id   INT             NOT NULL,
    CONSTRAINT FK_Material_Module FOREIGN KEY (module_id) REFERENCES Module(module_id)
);
GO

-- =============================================
-- 5. ASSESSMENT
-- =============================================
CREATE TABLE Assessment (
    assessment_id   INT IDENTITY(1,1) PRIMARY KEY,
    title           NVARCHAR(200)   NOT NULL,
    total_marks     INT             NOT NULL DEFAULT 100,
    module_id       INT             NOT NULL,
    CONSTRAINT FK_Assessment_Module FOREIGN KEY (module_id) REFERENCES Module(module_id)
);
GO

-- =============================================
-- 6. ASSESSMENT_RESULT
-- =============================================
CREATE TABLE Assessment_Result (
    result_id       INT IDENTITY(1,1) PRIMARY KEY,
    score           DECIMAL(5,2)    NOT NULL,
    attempt_date    DATETIME        NOT NULL DEFAULT GETDATE(),
    user_id         INT             NOT NULL,
    assessment_id   INT             NOT NULL,
    CONSTRAINT FK_Result_User       FOREIGN KEY (user_id)       REFERENCES [User](user_id),
    CONSTRAINT FK_Result_Assessment FOREIGN KEY (assessment_id) REFERENCES Assessment(assessment_id)
);
GO

-- =============================================
-- 7. CERTIFICATE
-- =============================================
CREATE TABLE Certificate (
    certificate_id  INT IDENTITY(1,1) PRIMARY KEY,
    issued_date     DATETIME        NOT NULL DEFAULT GETDATE(),
    user_id         INT             NOT NULL,
    module_id       INT             NOT NULL,
    CONSTRAINT FK_Certificate_User   FOREIGN KEY (user_id)   REFERENCES [User](user_id),
    CONSTRAINT FK_Certificate_Module FOREIGN KEY (module_id) REFERENCES Module(module_id)
);
GO

-- =============================================
-- 8. DISCUSSION
-- =============================================
CREATE TABLE Discussion (
    discussion_id   INT IDENTITY(1,1) PRIMARY KEY,
    content         NVARCHAR(MAX)   NOT NULL,
    created_at      DATETIME        NOT NULL DEFAULT GETDATE(),
    user_id         INT             NOT NULL,
    module_id       INT             NOT NULL,
    CONSTRAINT FK_Discussion_User   FOREIGN KEY (user_id)   REFERENCES [User](user_id),
    CONSTRAINT FK_Discussion_Module FOREIGN KEY (module_id) REFERENCES Module(module_id)
);
GO

-- =============================================
-- 9. SYSTEM_LOG
-- =============================================
CREATE TABLE System_Log (
    log_id      INT IDENTITY(1,1) PRIMARY KEY,
    action      NVARCHAR(255)   NOT NULL,
    timestamp   DATETIME        NOT NULL DEFAULT GETDATE(),
    user_id     INT             NOT NULL,
    CONSTRAINT FK_SystemLog_User FOREIGN KEY (user_id) REFERENCES [User](user_id)
);
GO

-- =============================================
-- DONE
-- =============================================
PRINT 'OlyMathDB schema created successfully.';
GO