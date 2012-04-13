using System;

namespace ReleaseCandidateTracker.Services
{
    public class ReleaseCandidateAlreadyExistsException : ApplicationException
    {
        private readonly string version;

        public ReleaseCandidateAlreadyExistsException(string version)
            : base(string.Format("Release candidate {0} already exists",version))
        {
            this.version = version;
        }

        public string Version
        {
            get { return version; }
        }
    }
}