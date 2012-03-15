using System;

namespace ReleaseCandidateTracker.Services
{
    public class ReleaseCandidateNotFoundException : Exception
    {
        private readonly string requestedVersion;

        public ReleaseCandidateNotFoundException(string requestedVersion)
        {
            this.requestedVersion = requestedVersion;
        }

        public string RequestedVersion
        {
            get { return requestedVersion; }
        }
    }
}