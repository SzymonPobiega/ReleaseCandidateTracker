using System.Web.Mvc;

namespace ReleaseCandidateTracker.Infrastructure
{
    public class ErrorResult : ActionResult
    {
        private readonly string message;
        private readonly int statusCode;

        public ErrorResult(string message)
            : this(message, 500)
        {
        }

        public ErrorResult(string message, int statusCode)
        {
            this.message = message;
            this.statusCode = statusCode;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write(message);
            context.HttpContext.Response.StatusCode = statusCode;
        }
    }
}