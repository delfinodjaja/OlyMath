using System;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OlyMath.Models;

namespace OlyMath
{
    public static class SeedData
    {
        public static void Initialize()
        {
            try
            {
                using (var context = ApplicationDbContext.Create())
                {
                    // Try to obtain managers from OWIN if available (safer during startup)
                    var httpContext = HttpContext.Current;
                    var owinContext = httpContext?.GetOwinContext();

                    RoleManager<IdentityRole> roleManager = null;
                    UserManager<ApplicationUser> userManager = null;

                    try
                    {
                        if (owinContext != null)
                        {
                            // Try to get RoleManager from OWIN first (may not be registered in all startup paths)
                            try { roleManager = owinContext.Get<RoleManager<IdentityRole>>(); } catch { roleManager = null; }

                            // Try to get the application user manager from OWIN
                            try { userManager = owinContext.GetUserManager<ApplicationUserManager>(); } catch { userManager = null; }
                        }
                    }
                    catch
                    {
                        // ignore OWIN access problems — we'll try fallback creation below
                        roleManager = null;
                        userManager = null;
                    }

                    // Fallback: create managers directly from the DB context if OWIN didn't provide them
                    if (roleManager == null)
                    {
                        try { roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)); } catch { roleManager = null; }
                    }

                    if (userManager == null)
                    {
                        try { userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context)); } catch { userManager = null; }
                    }

                    string[] roles = { "Admin", "Trainer", "Trainee" };

                    // Only attempt to create roles if we have a RoleManager ready
                    if (roleManager != null)
                    {
                        foreach (var role in roles)
                        {
                            try
                            {
                                if (!roleManager.RoleExists(role))
                                {
                                    roleManager.Create(new IdentityRole(role));
                                }
                            }
                            catch
                            {
                                // ignore role creation errors — don't crash startup
                            }
                        }
                    }

                    const string adminEmail = "admin@olymath.com";

                    // Only attempt to create the admin user if we have a UserManager
                    if (userManager != null)
                    {
                        try
                        {
                            var existingAdmin = userManager.FindByEmail(adminEmail);

                            if (existingAdmin == null)
                            {
                                var admin = new ApplicationUser
                                {
                                    UserName = adminEmail,
                                    Email = adminEmail,
                                    Name = "OlyMath Admin",
                                        EmailConfirmed = true
                                    };

                                    var result = userManager.Create(admin, "Admin@123");

                                    if (result.Succeeded && roleManager != null)
                                    {
                                        try { userManager.AddToRole(admin.Id, "Admin"); } catch { }
                                    }
                            }
                        }
                        catch
                        {
                            // ignore user creation errors — don't crash startup
                        }
                    }

                    // Ensure essential tables exist. This will not drop or alter existing tables.
                    try
                    {
                        EnsureDatabaseTables(context);
                    }
                    catch
                    {
                        // Table creation is best-effort; swallow exceptions to avoid blocking startup
                    }
                }
            }
            catch (Exception ex)
            {
                // Do not crash the website if seeding fails. Log to trace for diagnostics.
                try { System.Diagnostics.Trace.TraceWarning("SeedData.Initialize failed: " + ex.Message); } catch { }
            }
        }

        private static void EnsureDatabaseTables(ApplicationDbContext context)
        {
            var conn = context.Database.Connection;
            if (conn.State != ConnectionState.Open) conn.Open();

            bool TableExists(string tableName)
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME = @name";
                    var p = cmd.CreateParameter(); p.ParameterName = "@name"; p.Value = tableName; cmd.Parameters.Add(p);
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
                }
                }

            void ExecIgnore(string sql)
            {
                try
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                    // best-effort
                }
            }

            // Assessments table
            if (!TableExists("Assessments"))
            {
                var sql = @"
CREATE TABLE [dbo].[Assessments](
    [AssessmentID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Title] NVARCHAR(255) NULL,
    [Description] NVARCHAR(MAX) NULL,
    [PassingScore] INT NOT NULL DEFAULT 0,
    [CreatedBy] NVARCHAR(256) NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [ModuleId] INT NULL
);";
                ExecIgnore(sql);

                // Add FK to Modules if Modules exists
                if (TableExists("Modules"))
                {
                    ExecIgnore("ALTER TABLE [dbo].[Assessments] ADD CONSTRAINT FK_Assessments_Modules FOREIGN KEY (ModuleId) REFERENCES [dbo].[Modules](ModuleId) ON DELETE NO ACTION");
                }
            }

            // AssessmentQuestions table
            if (!TableExists("AssessmentQuestions"))
            {
                var sql = @"
CREATE TABLE [dbo].[AssessmentQuestions](
    [QuestionID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AssessmentID] INT NOT NULL,
    [QuestionText] NVARCHAR(MAX) NULL,
    [Marks] INT NOT NULL
);";
                ExecIgnore(sql);

                if (TableExists("Assessments"))
                {
                    ExecIgnore("ALTER TABLE [dbo].[AssessmentQuestions] ADD CONSTRAINT FK_AQ_Assessments FOREIGN KEY (AssessmentID) REFERENCES [dbo].[Assessments](AssessmentID) ON DELETE NO ACTION");
                }
            }

            // AssessmentAttempts table
            if (!TableExists("AssessmentAttempts"))
            {
                var sql = @"
CREATE TABLE [dbo].[AssessmentAttempts](
    [AttemptID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AssessmentID] INT NOT NULL,
    [UserID] NVARCHAR(128) NOT NULL,
    [Score] DECIMAL(18,2) NOT NULL,
    [AttemptDate] DATETIME NOT NULL DEFAULT GETDATE()
);";
                ExecIgnore(sql);

                if (TableExists("Assessments"))
                {
                    ExecIgnore("ALTER TABLE [dbo].[AssessmentAttempts] ADD CONSTRAINT FK_AA_Assessments FOREIGN KEY (AssessmentID) REFERENCES [dbo].[Assessments](AssessmentID) ON DELETE NO ACTION");
                }

                if (TableExists("AspNetUsers"))
                {
                    ExecIgnore("ALTER TABLE [dbo].[AssessmentAttempts] ADD CONSTRAINT FK_AA_Users FOREIGN KEY (UserID) REFERENCES [dbo].[AspNetUsers](Id) ON DELETE NO ACTION");
                }
            }

            // Certificates table
            if (!TableExists("Certificates"))
            {
                var sql = @"
CREATE TABLE [dbo].[Certificates](
    [CertificateID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [IssuedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [UserID] NVARCHAR(128) NOT NULL,
    [ModuleID] INT NOT NULL
);";
                ExecIgnore(sql);

                if (TableExists("AspNetUsers"))
                {
                    ExecIgnore("ALTER TABLE [dbo].[Certificates] ADD CONSTRAINT FK_Cert_Users FOREIGN KEY (UserID) REFERENCES [dbo].[AspNetUsers](Id) ON DELETE NO ACTION");
                }

                if (TableExists("Modules"))
                {
                    ExecIgnore("ALTER TABLE [dbo].[Certificates] ADD CONSTRAINT FK_Cert_Modules FOREIGN KEY (ModuleID) REFERENCES [dbo].[Modules](ModuleId) ON DELETE NO ACTION");
                }
            }
        }
    }
}
