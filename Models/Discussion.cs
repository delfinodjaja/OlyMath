using System;
using System.Collections.Generic;
using OlyMath.Models;
using Microsoft.AspNet.Identity.EntityFramework;

public class Discussion
{
    public int DiscussionId { get; set; }

    // You must add these three so Entity Framework sees them!
    public string Title { get; set; }
    public string Topic { get; set; }
    public bool IsFlagged { get; set; }

    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public int ModuleId { get; set; }
    public virtual Module Module { get; set; }
}