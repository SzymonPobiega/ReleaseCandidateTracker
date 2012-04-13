using System;
using System.Web.Mvc;
using ReleaseCandidateTracker.Models;
using System.Linq;
using Raven.Client.Linq;

namespace ReleaseCandidateTracker.Controllers
{
    public class ReleaseCandidateApiController : ApiController
    {
        [HttpPut]
        public ActionResult AttachScript(string versionNumber)
        {
            DocumentSession.PutAttachment(versionNumber.MakeDeploymentScriptKey(), Request.InputStream);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateState(string versionNumber, ReleaseCandidateState state)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);
            candidate.UpdateState(state);
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult List()
        {
            var candidates = DocumentSession
                .Query<ReleaseCandidate>()
                .OrderByDescending(x => x.CreationDate)
                .Take(20)
                .ToList()
                .Select(x => string.Format("{0};{1}",x.VersionNumber,x.State));

            return new ContentResult
                       {
                           Content = "Version;State\r\n" + string.Join("\r\n", candidates)
                       };
        }

        [HttpPut]
        public ActionResult GetVersion(string name)
        {
            var environemt = DocumentSession.GetEnvironment(name);
            return new ContentResult
            {
                Content = environemt.CurrentVersion
            };
        }

        [HttpPost]
        public ActionResult MarkAsDeployed(string versionNumber, string environment, bool success)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);
            var env = DocumentSession.GetEnvironment(environment);
            candidate.MarkAsDeployed(success, env);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Create(ReleaseCandidateCreateModel formPost)
        {
            var candidate = new ReleaseCandidate
            {
                CreationDate = DateTime.UtcNow,
                VersionNumber = formPost.VersionNumber,
                ProductName = formPost.ProductName,
                State = formPost.State
            };
            var existing = DocumentSession.Load<ReleaseCandidate>(candidate.VersionNumber.MakeCandidateId());
            if (existing != null)
            {
                throw new HttpException(409, string.Format("Release candidate {0} already exists", candidate.VersionNumber));
            }
            DocumentSession.Store(candidate, candidate.VersionNumber.MakeCandidateId());
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult AddReleaseNotes(string versionNumber, string releaseNotes)
        {
            DocumentSession.PutAttachment(versionNumber.MakeReleaseNotesKey(), Request.InputStream);
            return new EmptyResult();
        }
    }
}