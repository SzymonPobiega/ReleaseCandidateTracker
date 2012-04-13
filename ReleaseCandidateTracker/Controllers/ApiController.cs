using System.Web.Mvc;
using ReleaseCandidateTracker.Infrastructure;

namespace ReleaseCandidateTracker.Controllers
{
    public abstract class ApiController : BaseController
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            var httpException = filterContext.Exception as HttpException;
            filterContext.Result = httpException != null
                                       ? new ErrorResult(httpException.Message, httpException.ReponseCode)
                                       : new ErrorResult(filterContext.Exception.ToString());
            filterContext.ExceptionHandled = true;
        }
    }
}