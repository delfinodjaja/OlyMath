using System;
using System.Linq;
using System.Web.Mvc;
using WAPP_Assignment_Module.Models;
using Microsoft.AspNet.Identity;

namespace WAPP_Assignment_Module.Controllers
{
    [Authorize(Roles = "Trainer,Admin")]
    public class ChapterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Chapter/Index/5 (5 = moduleId)
        public ActionResult Index(int moduleId)
        {
            var chapters = db.Chapters
                             .Where(c => c.ModuleId == moduleId)
                             .OrderBy(c => c.SequenceOrder)
                             .ToList();
            ViewBag.ModuleId = moduleId;
            ViewBag.ModuleTitle = db.Modules.Find(moduleId).Title;
            return View(chapters);
        }

        // GET: Chapter/Create?moduleId=5
        public ActionResult Create(int moduleId)
        {
            ViewBag.ModuleId = moduleId;
            ViewBag.ModuleTitle = db.Modules.Find(moduleId).Title;
            return View();
        }

        // POST: Chapter/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                // Auto assign sequence order
                var lastOrder = db.Chapters
                                  .Where(c => c.ModuleId == chapter.ModuleId)
                                  .Select(c => c.SequenceOrder)
                                  .DefaultIfEmpty(0)
                                  .Max();
                chapter.SequenceOrder = lastOrder + 1;
                chapter.CreatedAt = DateTime.Now;
                db.Chapters.Add(chapter);
                db.SaveChanges();
                return RedirectToAction("Index", new { moduleId = chapter.ModuleId });
            }
            ViewBag.ModuleId = chapter.ModuleId;
            return View(chapter);
        }

        // GET: Chapter/Edit/5
        public ActionResult Edit(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null) return HttpNotFound();
            ViewBag.ModuleTitle = db.Modules.Find(chapter.ModuleId).Title;
            return View(chapter);
        }

        // POST: Chapter/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chapter).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { moduleId = chapter.ModuleId });
            }
            return View(chapter);
        }

        // GET: Chapter/Delete/5
        public ActionResult Delete(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null) return HttpNotFound();
            return View(chapter);
        }

        // POST: Chapter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            int moduleId = chapter.ModuleId;
            db.Chapters.Remove(chapter);
            db.SaveChanges();
            return RedirectToAction("Index", new { moduleId = moduleId });
        }

        // GET: Chapter/ViewChapter/5 (trainee reads chapter)
        [AllowAnonymous]
        public ActionResult ViewChapter(int id)
        {
            Chapter chapter = db.Chapters.Find(id);
            if (chapter == null) return HttpNotFound();

            string traineeId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;

            // Check if chapter is unlocked
            if (!IsChapterUnlocked(id, traineeId))
            {
                ViewBag.Message = "You need to complete the previous chapter first.";
                ViewBag.ModuleId = chapter.ModuleId;
                return View("Locked");
            }

            return View(chapter);
        }

        // POST: Chapter/MarkComplete/5
        [HttpPost]
        [AllowAnonymous]
        public ActionResult MarkComplete(int chapterId)
        {
            string traineeId = User.Identity.GetUserId();

            // Check if progress already exists
            var existing = db.ChapterProgresses
                             .FirstOrDefault(p => p.ChapterId == chapterId
                                 && p.TraineeId == traineeId);

            if (existing == null)
            {
                db.ChapterProgresses.Add(new ChapterProgress
                {
                    ChapterId = chapterId,
                    TraineeId = traineeId,
                    IsCompleted = true,
                    CompletedAt = DateTime.Now
                });
            }
            else
            {
                existing.IsCompleted = true;
                existing.CompletedAt = DateTime.Now;
            }

            db.SaveChanges();

            // Redirect back to the module page
            Chapter chapter = db.Chapters.Find(chapterId);
            return RedirectToAction("Learn", "Module", new { id = chapter.ModuleId });
        }

        // Helper method to check if a chapter is unlocked for a trainee
        private bool IsChapterUnlocked(int chapterId, string traineeId)
        {
            Chapter chapter = db.Chapters.Find(chapterId);
            if (chapter == null) return false;

            // First chapter is always unlocked
            if (chapter.SequenceOrder == 1) return true;

            // Find the previous chapter
            Chapter prevChapter = db.Chapters
                                    .Where(c => c.ModuleId == chapter.ModuleId
                                        && c.SequenceOrder == chapter.SequenceOrder - 1)
                                    .FirstOrDefault();

            if (prevChapter == null) return true;

            // Check if previous chapter is completed
            bool prevCompleted = db.ChapterProgresses
                                   .Any(p => p.ChapterId == prevChapter.ChapterId
                                       && p.TraineeId == traineeId
                                       && p.IsCompleted);

            return prevCompleted;
        }
    }
}