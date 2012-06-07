using System.Web.Mvc;

namespace ReleaseCandidateTracker.Controllers
{
    public class SettingsController : BaseController
    {
        public ActionResult Edit()
        {
            var settings = DocumentSession.GetSettings();
            return View(settings);
        }

        [HttpPost]
        public ActionResult Edit(int pageSize, string deploymentWorkingDirectory)
        {
            var settings = DocumentSession.GetSettings();

            settings.PageSize = pageSize;
            settings.DeploymentWorkingDirectory = deploymentWorkingDirectory;

            return RedirectToAction("Index", "ReleaseCandidate");
        }
    }
}