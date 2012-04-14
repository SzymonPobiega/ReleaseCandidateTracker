using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Raven.Client.Linq;
using ReleaseCandidateTracker.Models;

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
            return View(new ReleaseCandidateList
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
            return View(candidate);
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
