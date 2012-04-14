using System.Linq;
using System.Web.Mvc;
using Raven.Client.Linq;
using ReleaseCandidateTracker.Models;

namespace ReleaseCandidateTracker.Controllers
{
    public class ReleaseCandidateController : BaseController
    {
        public ActionResult Index(string productName)
        {
            var allCandidates = DocumentSession
                .Query<ReleaseCandidate>()
                .OrderByDescending(x => x.CreationDate)
                .ToList();
            return View(allCandidates);
        }        

        [HttpGet]
        public ActionResult Details(string versionNumber)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);
            return View(candidate);
        }

        [HttpGet]
        public ActionResult Edit(string versionNumber)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);
            return View(candidate);
        }

        [HttpPost]
        public ActionResult Edit(string versionNumber, ReleaseCandidateState state)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);
            candidate.UpdateState(state);
            return RedirectToAction("Index");
        }
    }
}
