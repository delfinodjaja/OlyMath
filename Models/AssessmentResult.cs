using System;
using System.ComponentModel.DataAnnotations;

namespace OlyMath.Models
{
    public class AssessmentResult
    {
        [Key]
        public int ResultId { get; set; }
        public decimal Score { get; set; }
        public DateTime AttemptDate { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int AssessmentId { get; set; }
        public virtual Assessment Assessment { get; set; }
    }
}