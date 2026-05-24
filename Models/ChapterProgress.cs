using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlyMath.Models
{
    public class ChapterProgress
    {
        [Key]
        public int ProgressId { get; set; }

        public int ChapterId { get; set; }

        public virtual Chapter Chapter { get; set; }

        public string TraineeId { get; set; }

        [ForeignKey("TraineeId")]
        public virtual ApplicationUser Trainee { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CompletedAt { get; set; }
    }
}