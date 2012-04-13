using System;

namespace ReleaseCandidateTracker.Services
{
    public class ReleaseCandidateNotFoundException : ApplicationException
    {
        private readonly string requestedVersion;

        public ReleaseCandidateNotFoundException(string requestedVersion)
            : base(string.Format("Release candidate {0} does not exist", requestedVersion)
        {
            this.requestedVersion = requestedVersion;
        }

        public string RequestedVersion
        {
            get { return requestedVersion; }
        }
    }
}