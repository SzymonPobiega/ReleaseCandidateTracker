using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReleaseCandidateTracker.Models;

namespace ReleaseCandidateTracker.Controllers
{
    public class EnvironmentController : BaseController
    {
        public ActionResult Index()
        {
            return View(DocumentSession
               .Query<DeploymentEnvironment>()
               .OrderBy(x => x.Name)
               .ToList());
        }

        public ActionResult Details(string name)
        {
            var environment = CandidateService.FindOneByName(name);
            return View(environment);
        }

        public ActionResult Create()
        {
            return View();
        } 

        [HttpPost]
        public ActionResult Create(DeploymentEnvironment environment)
        {
            DocumentSession.Store(environment);
            return RedirectToAction("Index");
        }        
    }
}
