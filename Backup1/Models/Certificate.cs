using System;
using System.Collections.Generic;
using OlyMath.Models;
using Microsoft.AspNet.Identity.EntityFramework;

public class Certificate
{
    public int CertificateId { get; set; }
    public DateTime IssuedDate { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public int ModuleId { get; set; }
    public virtual Module Module { get; set; }
}