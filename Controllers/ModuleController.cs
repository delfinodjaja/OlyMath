using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WAPP_Assignment_Module.Models;

namespace WAPP_Assignment_Module.Controllers
{
    [Authorize(Roles = "Trainer, Admin")]
    public class ModuleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        public ActionResult WhoAmI()
        {
            return Content("IsAuthenticated: " + User.Identity.IsAuthenticated
                + " | Name: " + User.Identity.Name
                + " | IsTrainee: " + User.IsInRole("Trainee")
                + " | IsTrainer: " + User.IsInRole("Trainer"));
        }

        //GET: Module/Catalog - Public page, no login needed
        [AllowAnonymous]
        public ActionResult Catalog()
        {
            var publishedModules = db.Modules
                                     .Where(m => m.Status == "Published")
                                     .OrderByDescending(m => m.CreatedAt)
                                     .ToList();
            return View(publishedModules);
        }

        // GET: Module
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Modules.ToList());
        }

        // GET: Module/Details/5
        public ActionResult Details(int id)
        {
            OlyModule module = db.Modules.Find(id);
            if (module == null) return HttpNotFound();
            return View(module);
        }

        // GET: Module/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Module/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OlyModule module)
        {
            if (ModelState.IsValid)
            {
                module.CreatedAt = DateTime.Now;
                module.UpdatedAt = DateTime.Now;
                module.TrainerId = User.Identity.GetUserId();
                db.Modules.Add(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(module);
        }

        // GET: Module/Edit/5
        public ActionResult Edit(int id)
        {
            OlyModule module = db.Modules.Find(id);
            if (module == null) return HttpNotFound();
            return View(module);
        }

        // POST: Module/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OlyModule module)
        {
            if (ModelState.IsValid)
            {
                // Preserve the original CreatedAt value
                var original = db.Modules.Find(module.ModuleId);
                module.CreatedAt = original.CreatedAt;
                module.UpdatedAt = DateTime.Now;

                db.Entry(original).CurrentValues.SetValues(module);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(module);
        }

        // GET: Module/Delete/5
        public ActionResult Delete(int id)
        {
            OlyModule module = db.Modules.Find(id);
            if (module == null) return HttpNotFound();
            return View(module);
        }

        // POST: Module/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OlyModule module = db.Modules.Find(id);
            db.Modules.Remove(module);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Module/Learn/5 - Trainee view with chapter lock states
        [AllowAnonymous]
        public ActionResult Learn(int id)
        {
            OlyModule module = db.Modules.Find(id);
            if (module == null) return HttpNotFound();

            string traineeId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;

            var chapters = db.Chapters
                             .Where(c => c.ModuleId == id)
                             .OrderBy(c => c.SequenceOrder)
                             .ToList();

            var lockMap = new System.Collections.Generic.Dictionary<int, bool>();
            var completedMap = new System.Collections.Generic.Dictionary<int, bool>();

            foreach (var chapter in chapters)
            {
                // Check if completed
                bool isCompleted = traineeId != null && db.ChapterProgresses
                    .Any(p => p.ChapterId == chapter.ChapterId
                        && p.TraineeId == traineeId
                        && p.IsCompleted);
                completedMap[chapter.ChapterId] = isCompleted;

                // Check if unlocked
                if (chapter.SequenceOrder == 1)
                {
                    lockMap[chapter.ChapterId] = true;
                }
                else
                {
                    var prevChapter = chapters.FirstOrDefault(
                        c => c.SequenceOrder == chapter.SequenceOrder - 1);

                    if (prevChapter == null)
                    {
                        lockMap[chapter.ChapterId] = true;
                    }
                    else if (traineeId == null)
                    {
                        lockMap[chapter.ChapterId] = false;
                    }
                    else
                    {
                        bool prevCompleted = db.ChapterProgresses
                            .Any(p => p.ChapterId == prevChapter.ChapterId
                                && p.TraineeId == traineeId
                                && p.IsCompleted);
                        lockMap[chapter.ChapterId] = prevCompleted;
                    }
                }
            }

            ViewBag.LockMap = lockMap;
            ViewBag.CompletedMap = completedMap;
            ViewBag.ModuleTitle = module.Title;
            return View(chapters);
        }

        // Helper method in ModuleController
        private bool IsChapterUnlocked(int chapterId, string traineeId)
        {
            Chapter chapter = db.Chapters.Find(chapterId);
            if (chapter == null) return false;
            if (chapter.SequenceOrder == 1) return true;

            Chapter prevChapter = db.Chapters
                                    .Where(c => c.ModuleId == chapter.ModuleId
                                        && c.SequenceOrder == chapter.SequenceOrder - 1)
                                    .FirstOrDefault();

            if (prevChapter == null) return true;

            bool prevCompleted = db.ChapterProgresses
                                   .Any(p => p.ChapterId == prevChapter.ChapterId
                                       && p.TraineeId == traineeId
                                       && p.IsCompleted);

            return prevCompleted;
        }
    }
}