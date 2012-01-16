using System.Web.Mvc;

namespace ReleaseCandidateTracker.Infrastructure
{
    public class CreatedResult : ActionResult
    {
        private readonly int id;

        public CreatedResult(int id)
        {
            this.id = id;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write(id);
            context.HttpContext.Response.StatusCode = 201;
        }
    }
}