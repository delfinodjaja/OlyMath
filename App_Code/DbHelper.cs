using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OlyMath
{
    public static class DbHelper
    {
        private static string GetConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["OlyMathDb"];
            string connStr = settings != null ? settings.ConnectionString : null;
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = "Server=(localdb)\\MSSQLLocalDB;Database=OlyMath;Integrated Security=True;";
            }
            return connStr;
        }

        private static string GetMasterConnectionString()
        {
            // Connect to master database to check/create the application database
            string connStr = GetConnectionString();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr);
            builder.InitialCatalog = "master";
            return builder.ConnectionString;
        }

        public static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(GetConnectionString());
            conn.Open();
            return conn;
        }

        public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static void InitializeDatabase()
        {
            try
            {
                // Step 1: Connect to Master and create OlyMath DB if it doesn't exist
                using (SqlConnection masterConn = new SqlConnection(GetMasterConnectionString()))
                {
                    masterConn.Open();
                    string checkDbSql = "SELECT database_id FROM sys.databases WHERE name = 'OlyMath'";
                    using (SqlCommand checkCmd = new SqlCommand(checkDbSql, masterConn))
                    {
                        object dbId = checkCmd.ExecuteScalar();
                        if (dbId == null || dbId == DBNull.Value)
                        {
                            string createDbSql = "CREATE DATABASE OlyMath;";
                            using (SqlCommand createCmd = new SqlCommand(createDbSql, masterConn))
                            {
                                createCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Step 2: Connect to OlyMath and create tables
                using (SqlConnection conn = GetConnection())
                {
                    // Create Tables in sequence
                    string sqlCreateTables = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE Users (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            FullName NVARCHAR(255) NOT NULL,
                            Email NVARCHAR(255) NOT NULL UNIQUE,
                            PasswordHash NVARCHAR(255) NOT NULL,
                            Role NVARCHAR(50) NOT NULL, -- Admin, Trainer, Trainee
                            CityCountry NVARCHAR(255) NULL,
                            CreatedAt DATETIME DEFAULT GETDATE()
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Modules]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE Modules (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Title NVARCHAR(255) NOT NULL,
                            Description NVARCHAR(MAX) NOT NULL,
                            Topic NVARCHAR(100) NOT NULL,
                            EstimatedTime NVARCHAR(50) NOT NULL,
                            CreatedByUserId INT NOT NULL,
                            CreatedAt DATETIME DEFAULT GETDATE(),
                            FOREIGN KEY(CreatedByUserId) REFERENCES Users(Id)
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StudyMaterials]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE StudyMaterials (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            ModuleId INT NOT NULL,
                            Title NVARCHAR(255) NOT NULL,
                            Type NVARCHAR(50) NOT NULL, -- PDF, VID, QZ
                            FileSizeText NVARCHAR(100) NOT NULL,
                            ContentUrl NVARCHAR(MAX) NULL,
                            OrderIndex INT NOT NULL,
                            IsLocked INT DEFAULT 0,
                            FOREIGN KEY(ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Questions]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE Questions (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            ModuleId INT NOT NULL,
                            QuestionText NVARCHAR(MAX) NOT NULL,
                            OptionA NVARCHAR(MAX) NOT NULL,
                            OptionB NVARCHAR(MAX) NOT NULL,
                            OptionC NVARCHAR(MAX) NOT NULL,
                            OptionD NVARCHAR(MAX) NOT NULL,
                            CorrectOption NVARCHAR(10) NOT NULL,
                            FOREIGN KEY(ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProgress]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE UserProgress (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UserId INT NOT NULL,
                            ModuleId INT NOT NULL,
                            ProgressPercentage INT DEFAULT 0,
                            LastAccessed DATETIME DEFAULT GETDATE(),
                            FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                            FOREIGN KEY(ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE,
                            CONSTRAINT UQ_UserProgress UNIQUE(UserId, ModuleId)
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserMaterialsStatus]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE UserMaterialsStatus (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UserId INT NOT NULL,
                            MaterialId INT NOT NULL,
                            IsDone INT DEFAULT 0,
                            FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                            FOREIGN KEY(MaterialId) REFERENCES StudyMaterials(Id) ON DELETE CASCADE,
                            CONSTRAINT UQ_UserMaterialsStatus UNIQUE(UserId, MaterialId)
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserAssessments]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE UserAssessments (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UserId INT NOT NULL,
                            ModuleId INT NOT NULL,
                            Score INT NOT NULL,
                            MaxScore INT NOT NULL,
                            CompletedAt DATETIME DEFAULT GETDATE(),
                            CertificateId NVARCHAR(100) NULL,
                            FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                            FOREIGN KEY(ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Discussions]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE Discussions (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UserId INT NOT NULL,
                            Title NVARCHAR(255) NOT NULL,
                            Content NVARCHAR(MAX) NOT NULL,
                            Topic NVARCHAR(50) NOT NULL,
                            LikesCount INT DEFAULT 0,
                            CreatedAt DATETIME DEFAULT GETDATE(),
                            FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiscussionReplies]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE DiscussionReplies (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            DiscussionId INT NOT NULL,
                            UserId INT NOT NULL,
                            Content NVARCHAR(MAX) NOT NULL,
                            CreatedAt DATETIME DEFAULT GETDATE(),
                            FOREIGN KEY(DiscussionId) REFERENCES Discussions(Id) ON DELETE CASCADE,
                            FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE NO ACTION
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DiscussionLikes]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE DiscussionLikes (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UserId INT NOT NULL,
                            DiscussionId INT NOT NULL,
                            FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE NO ACTION,
                            FOREIGN KEY(DiscussionId) REFERENCES Discussions(Id) ON DELETE CASCADE,
                            CONSTRAINT UQ_DiscussionLikes UNIQUE(UserId, DiscussionId)
                        );
                    END";

                    using (SqlCommand cmd = new SqlCommand(sqlCreateTables, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Seed Default Data
                    SeedDatabase(conn);
                }
            }
            catch (Exception ex)
            {
                // Log exception if loggers are attached, or throw to catch in Global.asax
                throw new Exception("Database initialization failed: " + ex.Message, ex);
            }
        }

        private static void SeedDatabase(SqlConnection conn)
        {
            long userCount = 0;
            string checkSql = "SELECT COUNT(*) FROM Users";
            using (SqlCommand cmd = new SqlCommand(checkSql, conn))
            {
                userCount = Convert.ToInt64(cmd.ExecuteScalar());
            }

            if (userCount > 0) return; // Database already seeded

            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // 1. Seed Users (using IDENTITY_INSERT to preserve pre-linked IDs)
                    string seedUsers = @"
                    SET IDENTITY_INSERT Users ON;
                    INSERT INTO Users (Id, FullName, Email, PasswordHash, Role, CityCountry) VALUES 
                    (1, 'Alexandra M. Reyes', 'trainee@olymath.com', @p1, 'Trainee', 'Manila, PH'),
                    (2, 'Dr. Budi Santoso', 'admin@olymath.com', @p2, 'Admin', 'Jakarta, ID'),
                    (3, 'Dr. Sarah Jenkins', 'trainer@olymath.com', @p3, 'Trainer', 'London, UK'),
                    (4, 'Kenji Aoki', 'kenji@olymath.com', @p4, 'Trainer', 'Tokyo, JP');
                    SET IDENTITY_INSERT Users OFF;";

                    using (SqlCommand cmd = new SqlCommand(seedUsers, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@p1", HashPassword("trainee123"));
                        cmd.Parameters.AddWithValue("@p2", HashPassword("admin123"));
                        cmd.Parameters.AddWithValue("@p3", HashPassword("trainer123"));
                        cmd.Parameters.AddWithValue("@p4", HashPassword("kenji123"));
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Seed Modules
                    string seedModules = @"
                    SET IDENTITY_INSERT Modules ON;
                    INSERT INTO Modules (Id, Title, Description, Topic, EstimatedTime, CreatedByUserId) VALUES
                    (1, 'Modular Arithmetic Fundamentals', 'A deep dive into the theory and application of modular arithmetic, covering residues, congruences, Fermat''s little theorem, and CRT.', 'Number Theory', '4h 20m', 4),
                    (2, 'Counting Techniques and Bijections', 'Advanced counting principles, permutations, combinations, double counting, and bijective mappings for olympiad problems.', 'Combinatorics', '3h 10m', 3),
                    (3, 'Projective Geometry and Transformations', 'Exploring collineations, cross-ratio, harmonic bundles, poles/polars, and inversion in olympiad geometry.', 'Geometry', '5h 45m', 3),
                    (4, 'Polynomials and Root Techniques', 'Roots of unity, symmetric polynomials, Vieta''s formulas, and polynomial divisibility.', 'Algebra', '4h 00m', 4),
                    (5, 'Classical Inequalities and AM-GM', 'Exploring Cauchy-Schwarz, AM-GM, Jensen''s inequality, rearrangement inequality, and their applications.', 'Inequalities', '3h 30m', 3),
                    (6, 'Mathematical Induction Masterclass', 'Strong induction, well-ordering principle, and induction in geometry and game theory.', 'Logic', '2h 15m', 4),
                    (7, 'Introduction to Graph Theory', 'Eulerian paths, Hamiltonian cycles, trees, planar graphs, and coloring theorems.', 'Logic', '3h 50m', 3),
                    (8, 'Euclidean Geometry Basics', 'Similar triangles, cyclic quadrilaterals, power of a point, and standard theorems (Ceva, Menelaus).', 'Geometry', '4h 15m', 3);
                    SET IDENTITY_INSERT Modules OFF;";

                    using (SqlCommand cmd = new SqlCommand(seedModules, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 3. Seed Study Materials
                    string seedMaterials = @"
                    SET IDENTITY_INSERT StudyMaterials ON;
                    INSERT INTO StudyMaterials (Id, ModuleId, Title, Type, FileSizeText, ContentUrl, OrderIndex, IsLocked) VALUES
                    (1, 1, 'Introduction to Congruences', 'PDF', '1.2 MB', 'congruences_intro.pdf', 1, 0),
                    (2, 1, 'Fermat''s Little Theorem Explained', 'VID', '24 min', 'https://youtube.com/watch?v=flt_explained', 2, 0),
                    (3, 1, 'Practice Set — Week 1', 'QZ', '15 questions', 'quiz_w1', 3, 0),
                    (4, 1, 'Chinese Remainder Theorem', 'PDF', '2.0 MB', 'crt_advanced.pdf', 4, 1),
                    
                    (5, 2, 'Combinatorics Basics & Bijections', 'PDF', '1.8 MB', 'combinatorics_basics.pdf', 1, 0),
                    (6, 2, 'Double Counting Proofs', 'VID', '18 min', 'https://youtube.com/watch?v=double_counting', 2, 0),
                    
                    (7, 3, 'Harmonic Bundles and Poles/Polars', 'PDF', '2.5 MB', 'projective_geom.pdf', 1, 0);
                    SET IDENTITY_INSERT StudyMaterials OFF;";

                    using (SqlCommand cmd = new SqlCommand(seedMaterials, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 4. Seed Questions
                    string seedQuestions = @"
                    INSERT INTO Questions (ModuleId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption) VALUES
                    (1, 'What is the value of 2^10 mod 7?', '1', '2', '4', '6', 'C'),
                    (1, 'If a = 5 mod 11, what is the value of a^2 mod 11?', '3', '4', '5', '9', 'A'),
                    (1, 'Find the remainder when 15! is divided by 17 (using Wilson''s Theorem).', '1', '2', '8', '16', 'A'),
                    (1, 'Solve the congruence system: x = 2 mod 3 and x = 3 mod 5. What is the value of x mod 15?', '8', '11', '13', '14', 'A'),
                    (1, 'Which of the following numbers is a primitive root modulo 5?', '1', '2', '3', 'both 2 and 3', 'D');";

                    using (SqlCommand cmd = new SqlCommand(seedQuestions, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 5. Seed Alex's achievements
                    string seedAssessments = @"
                    INSERT INTO UserAssessments (UserId, ModuleId, Score, MaxScore, CompletedAt, CertificateId) VALUES
                    (1, 6, 94, 100, '2025-03-12 14:32:00', 'OM-2025-0312'),
                    (1, 7, 88, 100, '2025-02-02 10:15:00', 'OM-2025-0202'),
                    (1, 8, 76, 100, '2025-01-18 16:40:00', 'OM-2025-0118');";

                    using (SqlCommand cmd = new SqlCommand(seedAssessments, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Enrolled modules
                    string seedProgress = @"
                    INSERT INTO UserProgress (UserId, ModuleId, ProgressPercentage) VALUES
                    (1, 1, 68),
                    (1, 2, 35),
                    (1, 3, 20);";

                    using (SqlCommand cmd = new SqlCommand(seedProgress, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Mark material status
                    string seedMatStatus = @"
                    INSERT INTO UserMaterialsStatus (UserId, MaterialId, IsDone) VALUES
                    (1, 1, 1),
                    (1, 2, 1);";

                    using (SqlCommand cmd = new SqlCommand(seedMatStatus, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 6. Seed Discussions
                    string seedDiscussions = @"
                    SET IDENTITY_INSERT Discussions ON;
                    INSERT INTO Discussions (Id, UserId, Title, Content, Topic, LikesCount, CreatedAt) VALUES
                    (1, 4, 'Proof for Wilson''s Theorem', 'Does anyone have a clean proof for Wilson''s theorem that avoids group theory? I keep going in circles with the direct approach and would love to see a more elementary route.', 'nt', 12, '2026-05-24 20:30:00'),
                    (2, 3, 'Combinatorics bijection questions', 'I am preparing a set of problems using Catalan numbers. What are your favorite representations of Catalan paths (mountain grids, parenthesizations, binary trees)?', 'co', 8, '2026-05-24 18:22:00'),
                    (3, 1, 'Struggling with projective transformations', 'Can anyone explain the physical intuition behind projective collineations? When we map a line to infinity, how does that simplify intersection proofs visually?', 'ge', 4, '2026-05-24 15:45:00');
                    SET IDENTITY_INSERT Discussions OFF;";

                    using (SqlCommand cmd = new SqlCommand(seedDiscussions, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Seed Replies
                    string seedReplies = @"
                    INSERT INTO DiscussionReplies (DiscussionId, UserId, Content, CreatedAt) VALUES
                    (1, 3, 'The standard way is to pair each element a in {2, ..., p-2} with its unique multiplicative inverse. Since the only self-inverses are 1 and p-1, they all pair up to leave 1 * (p-1) = -1 mod p. It is beautifully elementary!', '2026-05-24 20:45:00'),
                    (1, 1, 'Ah! Pairing with multiplicative inverses makes complete sense. Thank you, Sarah! No group theory required.', '2026-05-24 20:55:00');";

                    using (SqlCommand cmd = new SqlCommand(seedReplies, conn, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error during database seed execution: " + ex.Message, ex);
                }
            }
        }
    }
}
