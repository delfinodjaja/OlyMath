using System;
using System.Linq;
using OlyMath.Models;

namespace OlyMath.Services
{
    public class ProgressTrackingService
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        // Updates the ProgressStatus string in the Enrollment table
        public void UpdateProgress(string userId, int moduleId, string newStatus)
        {
            var enrollment = _context.Enrollments.FirstOrDefault(e => e.UserId == userId && e.ModuleId == moduleId);
            if (enrollment != null)
            {
                enrollment.ProgressStatus = newStatus;
                _context.SaveChanges();
            }
        }

        // Enforces the "Chapter 2 needs Chapter 1 Assessment completed" rule
        public bool CanAccessChapter(string userId, int moduleId, int targetChapterOrder)
        {
            // If it's the first chapter, grant access
            if (targetChapterOrder == 1) return true;

            // Find the previous chapter's assessment
            // Assuming Developer 2/4 ordered Assessments by Chapter/Sequence
            int previousChapterOrder = targetChapterOrder - 1;

            var prevAssessment = _context.Assessments
                .Where(a => a.ModuleId == moduleId && a.Title.Contains("Chapter " + previousChapterOrder))
                .FirstOrDefault();

            // If the previous chapter DOES NOT have an assessment, let them proceed
            if (prevAssessment == null) return true;

            // If it DOES have an assessment, check if Developer 4 recorded a passing result
            bool hasPassedPrevious = _context.AssessmentResults
                .Any(r => r.AssessmentId == prevAssessment.AssessmentId
                       && r.UserId == userId
                       && r.Score >= 50); // Assuming 50 is passing

            return hasPassedPrevious;
        }
    }
}