using Raven.Client;
using ReleaseCandidateTracker.Models;

namespace ReleaseCandidateTracker
{
    public static class DocumentSessionExtensions
    {
        public static ReleaseCandidate GetCandidate(this IDocumentSession documentSession, string versionNumber)
        {
            var result = documentSession.Load<ReleaseCandidate>(versionNumber.MakeCandidateId());
            if(result == null)
            {
                throw new HttpException(404, string.Format("Release candidate {0} does not exist", versionNumber));
            }
            return result;
        }

        public static DeploymentEnvironment GetEnvironment(this IDocumentSession documentSession, string name)
        {
            var result = documentSession.Load<DeploymentEnvironment>(name.MakeEnvironmentId());
            if (result == null)
            {
                throw new HttpException(404, string.Format("Environment {0} does not exist", name));
            }
            return result;
        }
    }
}