using System;
namespace OlyMath.Models
{
    public class Discussion
    {
        public int DiscussionId { get; set; }
        public string Title { get; set; }
        public bool IsFlagged { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
