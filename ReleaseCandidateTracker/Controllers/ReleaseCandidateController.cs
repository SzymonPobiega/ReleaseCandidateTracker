using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Raven.Client.Linq;
using ReleaseCandidateTracker.Models;
using ReleaseCandidateTracker.ViewModels;

namespace ReleaseCandidateTracker.Controllers
{
    public class ReleaseCandidateController : BaseController
    {
        private const int PageSize = 20;

        public ActionResult Index(int? page)
        {
            decimal totalCount = DocumentSession
                .Query<ReleaseCandidate>()
                .Count();

            var query = DocumentSession
                .Query<ReleaseCandidate>()
                .OrderByDescending(x => x.CreationDate);            

            var allCandidates = page.HasValue && page.Value > 0
                                    ? GetPagedResult(page.Value, query)
                                    : query.Take(PageSize).ToList();

            var currentPage = page ?? 1;
            var pageCount = (int)Math.Ceiling(totalCount / (decimal) PageSize);
            return View(new ReleaseCandidateListViewModel
                            {
                                Items = allCandidates,
                                Page = currentPage,
                                First = currentPage == 1,
                                Last = currentPage == pageCount
                            });
        }

        private static List<ReleaseCandidate> GetPagedResult(int page, IRavenQueryable<ReleaseCandidate> query)
        {
            return query
                .Skip((page - 1)*PageSize)                    
                .Take(PageSize)
                .ToList();
        }

        [HttpGet]
        public ActionResult Details(string versionNumber)
        {
            var candidate = DocumentSession.GetCandidate(versionNumber);
            var notesAttachment = DocumentSession.Advanced.DatabaseCommands.GetAttachment(versionNumber.MakeCustomDocumentKey("release-notes.txt"));
            string releaseNotesText;
            using (var stream = notesAttachment.Data())
            {
                releaseNotesText = new StreamReader(stream).ReadToEnd();
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
