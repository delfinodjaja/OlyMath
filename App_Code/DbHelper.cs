using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OlyMath
{
    public static class DbHelper
    {
        private static string GetConnectionString()
        {
            // Fallback connection string if not defined in Web.config
            string connStr = ConfigurationManager.ConnectionStrings["OlyMathDb"]?.ConnectionString;
            if (string.IsNullOrEmpty(connStr))
            {
                string appData = HttpContext.Current?.Server.MapPath("~/App_Data") ?? AppDomain.CurrentDomain.BaseDirectory;
                connStr = $"Data Source={Path.Combine(appData, "OlyMath.db")};Version=3;";
            }
            return connStr;
        }

        public static SQLiteConnection GetConnection()
        {
            SQLiteConnection conn = new SQLiteConnection(GetConnectionString());
            conn.Open();
            return conn;
        }

        public static DataTable ExecuteQuery(string sql, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string sql, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string sql, params SQLiteParameter[] parameters)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
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
            // SQLite creates the file automatically on connection open if it doesn't exist.
            using (SQLiteConnection conn = GetConnection())
            {
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    // Create Tables
                    string sqlCreate = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Email TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        Role TEXT NOT NULL, -- Admin, Trainer, Trainee
                        CityCountry TEXT NULL, -- e.g. Tokyo, JP (for discussion avatars)
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    );

                    CREATE TABLE IF NOT EXISTS Modules (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT NOT NULL,
                        Topic TEXT NOT NULL, -- Number Theory, Combinatorics, Geometry, Algebra, Inequalities, Logic
                        EstimatedTime TEXT NOT NULL, -- e.g. 4h 20m
                        CreatedByUserId INTEGER NOT NULL,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY(CreatedByUserId) REFERENCES Users(Id)
                    );

                    CREATE TABLE IF NOT EXISTS StudyMaterials (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ModuleId INTEGER NOT NULL,
                        Title TEXT NOT NULL,
                        Type TEXT NOT NULL, -- PDF, VID, QZ
                        FileSizeText TEXT NOT NULL, -- e.g. 1.2 MB or 24 min
                        ContentUrl TEXT NULL,
                        OrderIndex INTEGER NOT NULL,
                        IsLocked INTEGER DEFAULT 0, -- 0 = Unlocked, 1 = Locked
                        FOREIGN KEY(ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS Questions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ModuleId INTEGER NOT NULL,
                        QuestionText TEXT NOT NULL,
                        OptionA TEXT NOT NULL,
                        OptionB TEXT NOT NULL,
                        OptionC TEXT NOT NULL,
                        OptionD TEXT NOT NULL,
                        CorrectOption TEXT NOT NULL, -- A, B, C, D
                        FOREIGN KEY(ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS UserProgress (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        ModuleId INTEGER NOT NULL,
                        ProgressPercentage INTEGER DEFAULT 0,
                        LastAccessed DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                        FOREIGN KEY(ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE,
                        UNIQUE(UserId, ModuleId)
                    );

                    CREATE TABLE IF NOT EXISTS UserMaterialsStatus (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        MaterialId INTEGER NOT NULL,
                        IsDone INTEGER DEFAULT 0, -- 0 = No, 1 = Yes
                        FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                        FOREIGN KEY(MaterialId) REFERENCES StudyMaterials(Id) ON DELETE CASCADE,
                        UNIQUE(UserId, MaterialId)
                    );

                    CREATE TABLE IF NOT EXISTS UserAssessments (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        ModuleId INTEGER NOT NULL,
                        Score INTEGER NOT NULL,
                        MaxScore INTEGER NOT NULL,
                        CompletedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        CertificateId TEXT NULL, -- Unique string if passed (OM-2025-xxxx)
                        FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                        FOREIGN KEY(ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS Discussions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        Title TEXT NOT NULL,
                        Content TEXT NOT NULL,
                        Topic TEXT NOT NULL, -- nt, co, ge, al, iq (Number Theory, Combinatorics, etc.)
                        LikesCount INTEGER DEFAULT 0,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS DiscussionReplies (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        DiscussionId INTEGER NOT NULL,
                        UserId INTEGER NOT NULL,
                        Content TEXT NOT NULL,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY(DiscussionId) REFERENCES Discussions(Id) ON DELETE CASCADE,
                        FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE
                    );

                    CREATE TABLE IF NOT EXISTS DiscussionLikes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        DiscussionId INTEGER NOT NULL,
                        FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                        FOREIGN KEY(DiscussionId) REFERENCES Discussions(Id) ON DELETE CASCADE,
                        UNIQUE(UserId, DiscussionId)
                    );";

                    using (SQLiteCommand cmd = new SQLiteCommand(sqlCreate, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

                // Seed Default Data
                SeedDatabase(conn);
            }
        }

        private static void SeedDatabase(SQLiteConnection conn)
        {
            // Check if database is empty by counting users
            long userCount = 0;
            string checkSql = "SELECT COUNT(*) FROM Users";
            using (SQLiteCommand cmd = new SQLiteCommand(checkSql, conn))
            {
                userCount = (long)cmd.ExecuteScalar();
            }

            if (userCount > 0) return; // DB already has data

            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                // 1. Seed Users
                string seedUsers = @"
                INSERT INTO Users (FullName, Email, PasswordHash, Role, CityCountry) VALUES 
                ('Alexandra M. Reyes', 'trainee@olymath.com', @p1, 'Trainee', 'Manila, PH'),
                ('Dr. Budi Santoso', 'admin@olymath.com', @p2, 'Admin', 'Jakarta, ID'),
                ('Dr. Sarah Jenkins', 'trainer@olymath.com', @p3, 'Trainer', 'London, UK'),
                ('Kenji Aoki', 'kenji@olymath.com', @p4, 'Trainer', 'Tokyo, JP');";

                using (SQLiteCommand cmd = new SQLiteCommand(seedUsers, conn))
                {
                    cmd.Parameters.AddWithValue("@p1", HashPassword("trainee123"));
                    cmd.Parameters.AddWithValue("@p2", HashPassword("admin123"));
                    cmd.Parameters.AddWithValue("@p3", HashPassword("trainer123"));
                    cmd.Parameters.AddWithValue("@p4", HashPassword("kenji123"));
                    cmd.ExecuteNonQuery();
                }

                // 2. Seed Modules
                string seedModules = @"
                INSERT INTO Modules (Id, Title, Description, Topic, EstimatedTime, CreatedByUserId) VALUES
                (1, 'Modular Arithmetic Fundamentals', 'A deep dive into the theory and application of modular arithmetic, covering residues, congruences, Fermat''s little theorem, and CRT.', 'Number Theory', '4h 20m', 4),
                (2, 'Counting Techniques and Bijections', 'Advanced counting principles, permutations, combinations, double counting, and bijective mappings for olympiad problems.', 'Combinatorics', '3h 10m', 3),
                (3, 'Projective Geometry and Transformations', 'Exploring collineations, cross-ratio, harmonic bundles, poles/polars, and inversion in olympiad geometry.', 'Geometry', '5h 45m', 3),
                (4, 'Polynomials and Root Techniques', 'Roots of unity, symmetric polynomials, Vieta''s formulas, and polynomial divisibility.', 'Algebra', '4h 00m', 4),
                (5, 'Classical Inequalities and AM-GM', 'Exploring Cauchy-Schwarz, AM-GM, Jensen''s inequality, rearrangement inequality, and their applications.', 'Inequalities', '3h 30m', 3),
                (6, 'Mathematical Induction Masterclass', 'Strong induction, well-ordering principle, and induction in geometry and game theory.', 'Logic', '2h 15m', 4),
                (7, 'Introduction to Graph Theory', 'Eulerian paths, Hamiltonian cycles, trees, planar graphs, and coloring theorems.', 'Logic', '3h 50m', 3),
                (8, 'Euclidean Geometry Basics', 'Similar triangles, cyclic quadrilaterals, power of a point, and standard theorems (Ceva, Menelaus).', 'Geometry', '4h 15m', 3);";

                using (SQLiteCommand cmd = new SQLiteCommand(seedModules, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // 3. Seed Study Materials
                string seedMaterials = @"
                INSERT INTO StudyMaterials (Id, ModuleId, Title, Type, FileSizeText, ContentUrl, OrderIndex, IsLocked) VALUES
                (1, 1, 'Introduction to Congruences', 'PDF', '1.2 MB', 'congruences_intro.pdf', 1, 0),
                (2, 1, 'Fermat''s Little Theorem Explained', 'VID', '24 min', 'https://youtube.com/watch?v=flt_explained', 2, 0),
                (3, 1, 'Practice Set — Week 1', 'QZ', '15 questions', 'quiz_w1', 3, 0),
                (4, 1, 'Chinese Remainder Theorem', 'PDF', '2.0 MB', 'crt_advanced.pdf', 4, 1),
                
                (5, 2, 'Combinatorics Basics & Bijections', 'PDF', '1.8 MB', 'combinatorics_basics.pdf', 1, 0),
                (6, 2, 'Double Counting Proofs', 'VID', '18 min', 'https://youtube.com/watch?v=double_counting', 2, 0),
                
                (7, 3, 'Harmonic Bundles and Poles/Polars', 'PDF', '2.5 MB', 'projective_geom.pdf', 1, 0);";

                using (SQLiteCommand cmd = new SQLiteCommand(seedMaterials, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // 4. Seed Questions (Modular Arithmetic Quiz)
                string seedQuestions = @"
                INSERT INTO Questions (ModuleId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption) VALUES
                (1, 'What is the value of 2^10 mod 7?', '1', '2', '4', '6', 'C'),
                (1, 'If a = 5 mod 11, what is the value of a^2 mod 11?', '3', '4', '5', '9', 'A'),
                (1, 'Find the remainder when 15! is divided by 17 (using Wilson''s Theorem).', '1', '2', '8', '16', 'A'),
                (1, 'Solve the congruence system: x = 2 mod 3 and x = 3 mod 5. What is the value of x mod 15?', '8', '11', '13', '14', 'A'),
                (1, 'Which of the following numbers is a primitive root modulo 5?', '1', '2', '3', 'both 2 and 3', 'D');";

                using (SQLiteCommand cmd = new SQLiteCommand(seedQuestions, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // 5. Seed Alex's achievements (Alexandra M. Reyes has UserId 1)
                // She completed:
                // - Mathematical Induction Masterclass (Module 6): score 94/100, Certificate: OM-2025-0312
                // - Introduction to Graph Theory (Module 7): score 88/100, Certificate: OM-2025-0202
                // - Euclidean Geometry Basics (Module 8): score 76/100, Certificate: OM-2025-0118
                string seedAssessments = @"
                INSERT INTO UserAssessments (UserId, ModuleId, Score, MaxScore, CompletedAt, CertificateId) VALUES
                (1, 6, 94, 100, '2025-03-12 14:32:00', 'OM-2025-0312'),
                (1, 7, 88, 100, '2025-02-02 10:15:00', 'OM-2025-0202'),
                (1, 8, 76, 100, '2025-01-18 16:40:00', 'OM-2025-0118');";

                using (SQLiteCommand cmd = new SQLiteCommand(seedAssessments, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create initial progress for Alex:
                // Module 1 progress: 68% (has completed materials 1 & 2)
                // Module 2 progress: 35%
                // Module 3 progress: 20%
                string seedProgress = @"
                INSERT INTO UserProgress (UserId, ModuleId, ProgressPercentage) VALUES
                (1, 1, 68),
                (1, 2, 35),
                (1, 3, 20);";

                using (SQLiteCommand cmd = new SQLiteCommand(seedProgress, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Mark material status for Alex
                string seedMatStatus = @"
                INSERT INTO UserMaterialsStatus (UserId, MaterialId, IsDone) VALUES
                (1, 1, 1),
                (1, 2, 1);";

                using (SQLiteCommand cmd = new SQLiteCommand(seedMatStatus, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // 6. Seed Discussions
                string seedDiscussions = @"
                INSERT INTO Discussions (Id, UserId, Title, Content, Topic, LikesCount, CreatedAt) VALUES
                (1, 4, 'Proof for Wilson''s Theorem', 'Does anyone have a clean proof for Wilson''s theorem that avoids group theory? I keep going in circles with the direct approach and would love to see a more elementary route.', 'nt', 12, '2026-05-24 20:30:00'),
                (2, 3, 'Combinatorics bijection questions', 'I am preparing a set of problems using Catalan numbers. What are your favorite representations of Catalan paths (mountain grids, parenthesizations, binary trees)?', 'co', 8, '2026-05-24 18:22:00'),
                (3, 1, 'Struggling with projective transformations', 'Can anyone explain the physical intuition behind projective collineations? When we map a line to infinity, how does that simplify intersection proofs visually?', 'ge', 4, '2026-05-24 15:45:00');";

                using (SQLiteCommand cmd = new SQLiteCommand(seedDiscussions, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Seed Discussion Replies
                string seedReplies = @"
                INSERT INTO DiscussionReplies (DiscussionId, UserId, Content, CreatedAt) VALUES
                (1, 3, 'The standard way is to pair each element a in {2, ..., p-2} with its unique multiplicative inverse. Since the only self-inverses are 1 and p-1, they all pair up to leave 1 * (p-1) = -1 mod p. It is beautifully elementary!', '2026-05-24 20:45:00'),
                (1, 1, 'Ah! Pairing with multiplicative inverses makes complete sense. Thank you, Sarah! No group theory required.', '2026-05-24 20:55:00');";

                using (SQLiteCommand cmd = new SQLiteCommand(seedReplies, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }
    }
}
