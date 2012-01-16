using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using ReleaseCandidateTracker.Infrastructure;
using ReleaseCandidateTracker.Models;
using Environment = ReleaseCandidateTracker.Models.Environment;

namespace ReleaseCandidateTracker.Controllers
{
    public class ReleaseCandidateController : Controller
    {
        public ActionResult Index(string productName)
        {
            List<ReleaseCandidate> allCandidates;
            using (var session = Database.Instance.OpenSession())
            {
                var query = session
                    .Query<ReleaseCandidate>()
                    .OrderByDescending(x => x.CreationDate);

                var filteredQuery = !string.IsNullOrEmpty(productName)
                                    ? query.Where(x => x.ProductName == productName)
                                    : query;

                allCandidates = filteredQuery.ToList();
            }
            return View(allCandidates);
        }

        public ActionResult GetScript(int id)
        {
            Attachment attachment;
            using (var session = Database.Instance.OpenSession())
            {
                attachment = session.Advanced.DatabaseCommands.GetAttachment(id.ToString());
            }
            if (attachment != null)
            {
                var result = new FileStreamResult(attachment.Data(), "text/plain");
                var version = attachment.Metadata["Version"].ToString();
                var product = attachment.Metadata["Product"].ToString();
                result.FileDownloadName = string.Format("deploy-{0}-{1}.ps1", product, version);
                return result;
            }
            return new HttpNotFoundResult("Deployment script missing.");
        }

        public ActionResult Details(int id)
        {
            ReleaseCandidate candidate;
            using (var session = Database.Instance.OpenSession())
            {
                candidate = session.Load<ReleaseCandidate>(ReleaseCandidate.MakeId(id));
            }
            return View(candidate);
        }

        [HttpPut]
        public ActionResult AttachScript(int id)
        {
            using (var session = Database.Instance.OpenSession())
            {
                var candidate = session.Load<ReleaseCandidate>(ReleaseCandidate.MakeId(id));
                var metadata = new RavenJObject();
                metadata["Version"] = candidate.VersionNumber;
                metadata["Product"] = candidate.ProductName;
                session.Advanced.DatabaseCommands.PutAttachment(id.ToString(), null, Request.InputStream, metadata);
                session.SaveChanges();
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateState(string versionNumber, ReleaseCandidateState state)
        {
            using (var session = Database.Instance.OpenSession())
            {
                var candidate = session.Query<ReleaseCandidate>()
                    .Where(x => x.VersionNumber == versionNumber)
                    .First();
                candidate.UpdateState(state);
                session.SaveChanges();
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult MarkAsDeployed(string versionNumber, Environment environment)
        {
            using (var session = Database.Instance.OpenSession())
            {
                var candidate = session.Query<ReleaseCandidate>()
                    .Where(x => x.VersionNumber == versionNumber)
                    .First();
                candidate.MarkAsDeployed(environment);
                session.SaveChanges();
            }
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
            using (var session = Database.Instance.OpenSession())
            {
                var existing = session.Query<ReleaseCandidate>()
                    .Where(x => x.VersionNumber == candidate.VersionNumber)
                    .Any();
                if (existing)
                {
                    return new HttpNotFoundResult("Release candidate with same version number already exists.");
                }
                session.Store(candidate);
                session.SaveChanges();
            }

            return new CreatedResult(candidate.GetLocalId());
        }

        public ActionResult Edit(int id)
        {
            ReleaseCandidate candidate;
            using (var session = Database.Instance.OpenSession())
            {
                candidate = session.Load<ReleaseCandidate>(ReleaseCandidate.MakeId(id));
            }
            return View(candidate);
        }

        [HttpPost]
        public ActionResult Edit(ReleaseCandidateEditModel formPost)
        {
            ReleaseCandidate candidate;
            using (var session = Database.Instance.OpenSession())
            {
                candidate = session.Load<ReleaseCandidate>(ReleaseCandidate.MakeId(formPost.Id));
                candidate.UpdateState(formPost.State);
                session.SaveChanges();
            }
            return RedirectToAction("Index", new { productName = candidate.ProductName });
        }
    }
}
