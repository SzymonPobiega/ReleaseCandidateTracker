using System;
using System.Web.Mvc;
using ReleaseCandidateTracker.Infrastructure;
using ReleaseCandidateTracker.Models;
using Environment = ReleaseCandidateTracker.Models.Environment;

namespace ReleaseCandidateTracker.Controllers
{
    public class ApiController : BaseController
    {
        [HttpPut]
        public ActionResult AttachScript(string versionNumber)
        {
            ScriptService.AttachScript(versionNumber, Request.InputStream);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateState(string versionNumber, ReleaseCandidateState state)
        {
            CandidateService.UpdateState(versionNumber, state);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult MarkAsDeployed(string versionNumber, Environment environment)
        {
            CandidateService.MarkAsDeployed(versionNumber, environment);
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
            CandidateService.Store(candidate);
            return new EmptyResult();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = new ErrorResult(filterContext.Exception.Message);
            filterContext.ExceptionHandled = true;
        }
    }
}