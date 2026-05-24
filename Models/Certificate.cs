using System;
using System.ComponentModel.DataAnnotations;
namespace OlyMath.Models
{
    public class UserCertificate
    {
        [Key]
        public int CertificateId { get; set; }
        public DateTime IssuedDate { get; set; }
        public string UserId { get; set; }
        public int ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
