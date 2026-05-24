using System;
namespace OlyMath.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string ProgressStatus { get; set; }
        public string UserId { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
