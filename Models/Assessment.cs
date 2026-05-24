using System.Collections.Generic;
namespace OlyMath.Models
{
    public class Assessment
    {
        public int AssessmentId { get; set; }
        public string Title { get; set; }
        public int TotalMarks { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
        public virtual ICollection<AssessmentResult> Results { get; set; }
    }
}
