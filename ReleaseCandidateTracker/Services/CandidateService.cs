using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;
using ReleaseCandidateTracker.Models;

namespace ReleaseCandidateTracker.Services
{
    public class CandidateService
    {
        private readonly IDocumentSession documentSession;

        public CandidateService(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public void UpdateState(string versionNumber, ReleaseCandidateState state)
        {
            var candidate = GetCandidate(versionNumber);
            candidate.UpdateState(state);
        }

        public void MarkAsDeployed(string versionNumber, string environmentName, bool success)
        {
            var candidate = GetCandidate(versionNumber);
            var environment = GetEnvironment(environmentName);
            candidate.MarkAsDeployed(success, environment);
        }

        public void Store(ReleaseCandidate candidate)
        {
            var existing = documentSession.Load<ReleaseCandidate>(candidate.VersionNumber.MakeCandidateId());
            if (existing != null)
            {
                throw new ReleaseCandidateAlreadyExistsException(candidate.VersionNumber);
            }
            documentSession.Store(candidate, candidate.VersionNumber.MakeCandidateId());
        }

        public IList<ReleaseCandidate> GetAll(string productName)
        {
            var query = documentSession
               .Query<ReleaseCandidate>()
               .OrderByDescending(x => x.CreationDate);

            var filteredQuery = !string.IsNullOrEmpty(productName)
                                ? query.Where(x => x.ProductName == productName)
                                : query;

            return filteredQuery.ToList();
        }

        public ReleaseCandidate GetCandidate(string versionNumber)
        {
            var result = documentSession.Load<ReleaseCandidate>(versionNumber.MakeCandidateId());
            if(result == null)
            {
                throw new ReleaseCandidateNotFoundException(versionNumber);
            }
            return result;
        }


        public DeploymentEnvironment GetEnvironment(string name)
        {
            var result = documentSession.Load<DeploymentEnvironment>(name.MakeEnvironmentId());
            if (result == null)
            {
                throw new InvalidOperationException(string.Format("Environment {0} not found", name));
            }
            return result;
        }
    }
}