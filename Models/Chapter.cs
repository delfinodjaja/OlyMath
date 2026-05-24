using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WAPP_Assignment_Module.Models
{
    public class Chapter
    {
        [Key]
        public int ChapterId { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual OlyModule Module { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Chapter Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Content")]
        public string ContentHtml { get; set; }

        [Display(Name = "Chapter Order")]
        public int SequenceOrder { get; set; }

        [Display(Name = "Has Assessment?")]
        public bool HasAssessment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}