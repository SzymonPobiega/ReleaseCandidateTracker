using System;
using System.Web.Mvc;
using ReleaseCandidateTracker.Infrastructure;
using ReleaseCandidateTracker.Models;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;

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
            var environemt = CandidateService.GetEnvironment(name);
            return new ContentResult
            {
                Content = environemt.CurrentVersion
            };
        }

        [HttpPost]
        public ActionResult MarkAsDeployed(string versionNumber, string environment, bool success)
        {
            CandidateService.MarkAsDeployed(versionNumber, environment, success);
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
            if (filterContext.Exception is ApplicationException)
            {
                filterContext.Result = new ErrorResult(filterContext.Exception.Message);
            }
            else
            {
                filterContext.Result = new ErrorResult(filterContext.Exception.ToString());
            }
            filterContext.ExceptionHandled = true;
        }
    }
}