using System.Collections.Generic;
using ReleaseCandidateTracker.Models;

namespace ReleaseCandidateTracker.ViewModels
{
    public class ReleaseCandidateListViewModel
    {
        public List<DeploymentEnvironment> Environments { get; set; }
        public List<ReleaseCandidate> Items { get; set; }
        public int Page { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}