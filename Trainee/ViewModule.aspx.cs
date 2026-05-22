using System;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using OlyMath.Models;
using OlyMath.Services;

namespace OlyMath.Trainee
{
    public partial class ViewModule : System.Web.UI.Page
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        private ProgressTrackingService _progressService = new ProgressTrackingService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int moduleId))
                {
                    LoadModuleDetails(moduleId);
                }
                else
                {
                    Response.Redirect("~/Default.aspx");
                }
            }
        }

        private void LoadModuleDetails(int moduleId)
        {
            var module = _context.Modules.FirstOrDefault(m => m.ModuleId == moduleId);
            if (module != null)
            {
                litModuleTitle.Text = module.Title;
                litModuleDesc.Text = module.Description;

                var materials = _context.Materials.Where(m => m.ModuleId == moduleId).ToList();
                if (materials.Count > 0)
                {
                    rptMaterials.DataSource = materials;
                    rptMaterials.DataBind();
                }
                else
                {
                    lblNoMaterials.Visible = true;
                }

                var assessments = _context.Assessments.Where(a => a.ModuleId == moduleId).ToList();
                rptAssessments.DataSource = assessments;
                rptAssessments.DataBind();
            }
        }

        protected void rptMaterials_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var material = (Material)e.Item.DataItem;
                HyperLink hlView = (HyperLink)e.Item.FindControl("hlViewMaterial");
                Label lblLocked = (Label)e.Item.FindControl("lblLocked");

                string userId = User.Identity.GetUserId();
                int chapterNum = ExtractChapterNumber(material.Title);

                if (_progressService.CanAccessChapter(userId, material.ModuleId, chapterNum))
                {
                    hlView.NavigateUrl = material.ContentUrl;
                    hlView.Visible = true;
                    lblLocked.Visible = false;
                }
                else
                {
                    hlView.Visible = false;
                    lblLocked.Visible = true;
                }
            }
        }

        protected void rptAssessments_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var assessment = (Assessment)e.Item.DataItem;
                HyperLink hlTake = (HyperLink)e.Item.FindControl("hlTakeAssessment");
                Label lblLocked = (Label)e.Item.FindControl("lblLockedAssessment");

                string userId = User.Identity.GetUserId();
                int chapterNum = ExtractChapterNumber(assessment.Title);

                if (_progressService.CanAccessChapter(userId, assessment.ModuleId, chapterNum))
                {
                    hlTake.NavigateUrl = $"~/Trainee/TakeAssessment.aspx?id={assessment.AssessmentId}";
                    hlTake.Visible = true;
                    lblLocked.Visible = false;
                }
                else
                {
                    hlTake.Visible = false;
                    lblLocked.Visible = true;
                }
            }
        }


        private int ExtractChapterNumber(string title)
        {
            if (string.IsNullOrEmpty(title)) return 1;

            string[] words = title.Split(' ', ':');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Equals("Chapter", StringComparison.OrdinalIgnoreCase) && i + 1 < words.Length)
                {
                    if (int.TryParse(words[i + 1], out int num))
                    {
                        return num;
                    }
                }
            }
            return 1; 
        }
    }
}