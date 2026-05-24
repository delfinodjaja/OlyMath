using System;
using System.Collections.Generic;
using OlyMath.Models;
using Microsoft.AspNet.Identity.EntityFramework;


public class Module
{
    public int ModuleId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }

    public string TrainerId { get; set; }
    public virtual ApplicationUser Trainer { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; }
    public virtual ICollection<Material> Materials { get; set; }
    public virtual ICollection<Assessment> Assessments { get; set; }
    public virtual ICollection<Discussion> Discussions { get; set; }
    public virtual ICollection<Certificate> Certificates { get; set; }
}