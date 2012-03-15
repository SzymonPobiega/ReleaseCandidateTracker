using System.Web.Mvc;
using ReleaseCandidateTracker.Infrastructure;
using ReleaseCandidateTracker.Models;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;


namespace ReleaseCandidateTracker.Controllers
{
    public class EnvironmentApiController : BaseController
    {
        [HttpGet]
        public ActionResult List()
        {
            var environments = DocumentSession
                .Query<DeploymentEnvironment>()
                .OrderBy(x => x.Name)
                .ToList()
                .Select(x => string.Format("{0};{1}", x.Name, x.CurrentVersion));

            return new ContentResult
            {
                Content = "Name;Version\r\n" + string.Join("\r\n", environments)
            };
        }
        
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.Result = new ErrorResult(filterContext.Exception.Message);
            filterContext.ExceptionHandled = true;
        }
    }
}