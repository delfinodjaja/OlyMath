using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace OlyMath.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Module>           Modules           { get; set; }
        public DbSet<Enrollment>       Enrollments       { get; set; }
        public DbSet<Material>         Materials         { get; set; }
        public DbSet<Assessment>       Assessments       { get; set; }
        public DbSet<AssessmentResult> AssessmentResults { get; set; }
        public DbSet<UserCertificate>      Certificates { get; set; }
        public DbSet<Discussion>       Discussions       { get; set; }
        public DbSet<SystemLog>        SystemLogs        { get; set; }
    }
}

