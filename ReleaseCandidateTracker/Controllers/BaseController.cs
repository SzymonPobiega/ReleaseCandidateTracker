using System.Web.Mvc;
using Raven.Client;
using ReleaseCandidateTracker.Infrastructure;

namespace ReleaseCandidateTracker.Controllers
{
    public abstract class BaseController : Controller
    {
        public IDocumentSession DocumentSession { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }
            DocumentSession = Database.Instance.OpenSession();
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