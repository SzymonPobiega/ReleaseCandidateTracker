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
            var candidate = FindOneByVersionNumber(versionNumber);
            candidate.UpdateState(state);
        }

        public void MarkAsDeployed(string versionNumber, string environmentName, bool success)
        {
            var candidate = FindOneByVersionNumber(versionNumber);
            var environment = FindOneByName(environmentName);
            candidate.MarkAsDeployed(success, environment);
        }

        public void Store(ReleaseCandidate candidate)
        {
            var existing = documentSession.Query<ReleaseCandidate>()
                .Where(x => x.VersionNumber == candidate.VersionNumber)
                .Any();
            if (existing)
            {
                throw new ReleaseCandidateAlreadyExistsException(candidate.VersionNumber);
            }
            documentSession.Store(candidate);
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

        public ReleaseCandidate FindOneByVersionNumber(string versionNumber)
        {
            var result = documentSession.Query<ReleaseCandidate>()
                .Where(x => x.VersionNumber == versionNumber)
                .FirstOrDefault();
            if(result == null)
            {
                throw new ReleaseCandidateNotFoundException(versionNumber);
            }
            return result;
        }


        public DeploymentEnvironment FindOneByName(string name)
        {
            var result = documentSession.Query<DeploymentEnvironment>()
                .Where(x => x.Name == name)
                .FirstOrDefault();
            if (result == null)
            {
                throw new InvalidOperationException(string.Format("Environment {0} not found", name));
            }
            return result;
        }
    }
}