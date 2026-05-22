using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WAPP_Assignment_Module.Models;

namespace WAPP_Assignment_Module.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();

            if (User.IsInRole("Trainer"))
            {
                // Trainer dashboard data
                var myModules = db.Modules
                                  .Where(m => m.TrainerId == userId)
                                  .OrderByDescending(m => m.CreatedAt)
                                  .Take(5)
                                  .ToList();
                ViewBag.MyModules = myModules;
                ViewBag.TotalModules = db.Modules
                                         .Count(m => m.TrainerId == userId);
                return View("TrainerDashboard");
            }
            else if (User.IsInRole("Trainee"))
            {
                // Trainee dashboard data
                var latestModules = db.Modules
                                      .Where(m => m.Status == "Published")
                                      .OrderByDescending(m => m.CreatedAt)
                                      .Take(3)
                                      .ToList();
                ViewBag.LatestModules = latestModules;

                // Get in-progress chapters
                var completedChapterIds = db.ChapterProgresses
                                            .Where(p => p.TraineeId == userId
                                                && p.IsCompleted)
                                            .Select(p => p.ChapterId)
                                            .ToList();
                ViewBag.CompletedCount = completedChapterIds.Count;
                return View("TraineeDashboard");
            }
            else if (User.IsInRole("Admin"))
            {
                ViewBag.TotalModules = db.Modules.Count();
                ViewBag.PublishedModules = db.Modules
                                             .Count(m => m.Status == "Published");
                ViewBag.DraftModules = db.Modules
                                         .Count(m => m.Status == "Draft");
                var allModules = db.Modules
                                   .OrderByDescending(m => m.CreatedAt)
                                   .Take(10)
                                   .ToList();
                ViewBag.AllModules = allModules;
                return View("AdminDashboard");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}