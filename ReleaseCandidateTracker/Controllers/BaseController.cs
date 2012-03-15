using System.Web.Mvc;
using Raven.Client;
using ReleaseCandidateTracker.Infrastructure;
using ReleaseCandidateTracker.Services;

namespace ReleaseCandidateTracker.Controllers
{
    public class BaseController : Controller
    {
        public IDocumentSession DocumentSession { get; private set; }
        public CandidateService CandidateService { get; private set; }
        public ScriptService ScriptService { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }
            DocumentSession = Database.Instance.OpenSession();
            CandidateService = new CandidateService(DocumentSession);
            ScriptService = new ScriptService(DocumentSession);
            base.OnActionExecuting(filterContext);
        }
        
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }
            if(DocumentSession != null)
            {
                if (filterContext.Exception == null)
                {
                    DocumentSession.SaveChanges();
                }
                DocumentSession.Dispose();
            }
            base.OnActionExecuted(filterContext);
        }
    }
}