using System;
using System.Web.Mvc;
using ReleaseCandidateTracker.Models;
using System.Linq;
using Raven.Client.Linq;

namespace ReleaseCandidateTracker.Controllers
{
    public class ReleaseCandidateApiController : ApiController
    {        
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
                .Select(x => string.Format("{0};{1};{2};{3}",x.ProductName, x.VersionNumber,x.State,x.CreationDate.ToString("s")));

            return new ContentResult
                       {
                           Content = "ProductName;Version;State;CreationDate\r\n" + string.Join("\r\n", candidates)
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
            var candidateId = candidate.FullVersion.MakeCandidateId();
            var existing = DocumentSession.Load<ReleaseCandidate>(candidateId);
            if (existing != null)
            {
                throw new HttpException(409, string.Format("Release candidate {0} already exists", candidate.VersionNumber));
            }
            DocumentSession.Store(candidate, candidateId);
            return new EmptyResult();
        }

        
        [HttpPut]
        public ActionResult AttachDocument(string versionNumber, string documentName, string contentType)
        {
            DocumentSession.PutAttachment(versionNumber.MakeCustomDocumentKey(documentName), contentType, Request.InputStream);
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult GetDocument(string versionNumber, string documentName)
        {
            var doc = DocumentSession.GetAttachmentResult(versionNumber.MakeCustomDocumentKey(documentName));
            return doc ?? new HttpNotFoundResult(string.Format("No {0} associated with release candidate {1}", documentName, versionNumber));
        }

        [HttpPut]
        public ActionResult AttachScript(string versionNumber)
        {
            return AttachDocument(versionNumber, "deploy.ps1", "text/plain");
        }

        [HttpGet]
        public ActionResult GetScript(string versionNumber)
        {
            return GetDocument(versionNumber, "deploy.ps1");
        }


        [HttpPut]
        public ActionResult AttachReleaseNotes(string versionNumber)
        {
            return AttachDocument(versionNumber, "release-notes.txt", "text/plain");
        }

        [HttpGet]
        public ActionResult GetReleaseNotes(string versionNumber)
        {
            return GetDocument(versionNumber, "release-notes.txt");
        }
    }
}