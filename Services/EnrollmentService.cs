using System;
using System.Collections.Generic;
using System.Linq;
using OlyMath.Models;

namespace OlyMath.Services
{
    public class EnrollmentService
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        // Create: Enroll a user
        public bool EnrollInModule(string userId, int moduleId)
        {
            // Check if already enrolled
            if (_context.Enrollments.Any(e => e.UserId == userId && e.ModuleId == moduleId))
                return false;

            var enrollment = new Enrollment
            {
                UserId = userId,
                ModuleId = moduleId,
                EnrolledAt = DateTime.Now,
                ProgressStatus = "0%" // Default starting progress
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
            return true;
        }

        // Read: Get Trainee's Enrolled Modules
        public List<Enrollment> GetTraineeEnrollments(string userId)
        {
            return _context.Enrollments
                           .Include("Module")
                           .Where(e => e.UserId == userId)
                           .ToList();
        }

        // Delete: Unenroll/Drop
        public void DropModule(string userId, int moduleId)
        {
            var enrollment = _context.Enrollments.FirstOrDefault(e => e.UserId == userId && e.ModuleId == moduleId);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                _context.SaveChanges();
            }
        }
    }
}