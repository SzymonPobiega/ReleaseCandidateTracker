using System.Web.Mvc;

namespace ReleaseCandidateTracker.Infrastructure
{
    public class ErrorResult : ActionResult
    {
        private readonly string message;

        public ErrorResult(string message)
        {
            this.message = message;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write(message);
            context.HttpContext.Response.StatusCode = 500;
        }
    }
}