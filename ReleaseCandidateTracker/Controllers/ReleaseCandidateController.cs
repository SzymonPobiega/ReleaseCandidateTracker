using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Raven.Client.Linq;
using ReleaseCandidateTracker.Models;
using ReleaseCandidateTracker.ViewModels;

namespace ReleaseCandidateTracker.Controllers
{
    public class ReleaseCandidateController : BaseController
    {
        public ActionResult Index(int? page)
        {
            var settings = DocumentSession.GetSettings();

            decimal totalCount = DocumentSession
                .Query<ReleaseCandidate>()
                .Count();

            var query = DocumentSession
                .Query<ReleaseCandidate>()
                .OrderByDescending(x => x.CreationDate);

            var environments = DocumentSession
                .Query<DeploymentEnvironment>()
                .ToList();

            var allCandidates = page.HasValue && page.Value > 0
                                    ? GetPagedResult(page.Value, settings.PageSize, query)
                                    : query.Take(settings.PageSize).ToList();

            var currentPage = page ?? 1;
            var pageCount = (int)Math.Ceiling(totalCount / (decimal) settings.PageSize);
            return View(new ReleaseCandidateListViewModel
                            {
                                Items = allCandidates,
                                Page = currentPage,
                                Environments = environments,
                                First = currentPage == 1,
                                Last = currentPage == pageCount
                            });
        }

        private static List<ReleaseCandidate> GetPagedResult(int page, int pageSize, IRavenQueryable<ReleaseCandidate> query)
        {
            return query
                .Skip((page - 1)*pageSize)                    
                .Take(pageSize)
                .ToList();
        }

        [HttpGet]
        public ActionResult Deploy(string versionNumber)
        {
            var settings = DocumentSession.GetSettings();
            if (string.IsNullOrEmpty(settings.DeploymentWorkingDirectory))
            {
                TempData["Message"] = "Please specify deployment working directory";
                return RedirectToAction("Edit", "Settings");
            }

            var deployScriptKey = versionNumber.MakeCustomDocumentKey("deploy.ps1");
            var deployScript = DocumentSession.Advanced.DatabaseCommands.GetAttachment(deployScriptKey);

            using (var fileStream = System.IO.File.Create(Path.Combine(settings.DeploymentWorkingDirectory, deployScriptKey)))
            {
                deployScript.Data().CopyTo(fileStream);
            }

            var uniqueId = Guid.NewGuid().ToString();
            var listenUrl = string.Format("http://{0}:12345/{1}", Environment.MachineName, uniqueId);
            var startInfo = new ProcessStartInfo(Path.Combine(settings.DeploymentWorkingDirectory, "PowerShellHtmlConsole.exe"),
                string.Format(@"--listen={0} --script=.\{1}", listenUrl,deployScriptKey));
            startInfo.WorkingDirectory = settings.DeploymentWorkingDirectory;
            Process.Start(startInfo);
            Thread.Sleep(5000);
            return Redirect(listenUrl + "/console.htm");
        }

        [HttpGet]
        public ActionResult Details(string versionNumber)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);            
            string releaseNotesText = "";
            var notesAttachment = DocumentSession.Advanced.DatabaseCommands.GetAttachment(versionNumber.MakeCustomDocumentKey("release-notes.txt"));
            if (notesAttachment != null)
            {
                using (var stream = notesAttachment.Data())
                {
                    releaseNotesText = new StreamReader(stream).ReadToEnd();
                }
            }
            return View(new ReleaseCandidateViewModel
                            {
                                Candidate = candidate,                                
                                ReleaseNotes = FormatReleaseNotesForHtml(releaseNotesText)
                            });
        }

        private static string FormatReleaseNotesForHtml(string releaseNotesText)
        {
            var items = releaseNotesText.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            return "<ul>" + string.Join(Environment.NewLine, items.Select(x => "<li>" + x.TrimStart(' ','*') + "</li>")) + "</ul>";
        }

        [HttpGet]
        public ActionResult Edit(string versionNumber)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);
            return View(candidate);
        }

        [HttpPost]
        public ActionResult Edit(string versionNumber, ReleaseCandidateState state)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);
            candidate.UpdateState(state);
            return RedirectToAction("Index");
        }
    }
}
