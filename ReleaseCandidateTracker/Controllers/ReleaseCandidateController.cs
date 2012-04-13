using System.Linq;
using System.Web.Mvc;
using ReleaseCandidateTracker.Models;

namespace ReleaseCandidateTracker.Controllers
{
    public class ReleaseCandidateController : BaseController
    {
        public ActionResult Index(string productName)
        {
            var allCandidates = CandidateService.GetAll(productName);
            return View(allCandidates);
        }

        [HttpGet]
        public ActionResult GetScript(string versionNumber)
        {
            var candidate = CandidateService.GetCandidate(versionNumber);
            var attachment = ScriptService.GetScript(versionNumber);
            if (attachment != null)
            {
                var result = new FileStreamResult(attachment, "text/plain");
                var version = candidate.VersionNumber;
                var product = candidate.ProductName;
                result.FileDownloadName = string.Format("deploy-{0}-{1}.ps1", product, version);
                return result;
            }
            return new HttpNotFoundResult("Deployment script missing.");
        }

        [HttpGet]
        public ActionResult Details(string versionNumber)
        {
            var candidate = CandidateService.GetCandidate(versionNumber);
            return View(candidate);
        }

        [HttpGet]
        public ActionResult Edit(string versionNumber)
        {
            var candidate = CandidateService.GetCandidate(versionNumber);
            return View(candidate);
        }

        [HttpPost]
        public ActionResult Edit(string versionNumber, ReleaseCandidateState state)
        {
            CandidateService.UpdateState(versionNumber, state);
            return RedirectToAction("Index");
        }
    }
}
