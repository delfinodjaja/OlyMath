using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WAPP_Assignment_Module.Models
{
    public class OlyModule
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Module Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Difficulty Level")]
        public string Difficulty { get; set; }

        [Required]
        [Display(Name = "Topic Tag")]
        public string TopicTag { get; set; }

        [Display(Name = "Thumbnail Path")]
        public string ThumbnailPath { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public virtual ApplicationUser Trainer { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; }
        }
    }