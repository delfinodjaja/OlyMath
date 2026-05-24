using System;
using System.ComponentModel.DataAnnotations; 

namespace OlyMath.Models
{
    public class SystemLog
    {
        [Key] 
        public int LogId { get; set; }
        
        public string Action { get; set; }
        
        public DateTime Timestamp { get; set; }

        public string UserId { get; set; }
        
        public virtual ApplicationUser User { get; set; }

        public SystemLog()
        {
            Action = string.Empty;
            UserId = string.Empty;
            Timestamp = DateTime.UtcNow;
        }
    }
}