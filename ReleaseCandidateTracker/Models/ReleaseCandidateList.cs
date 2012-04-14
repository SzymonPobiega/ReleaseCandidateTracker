using System.Collections.Generic;

namespace ReleaseCandidateTracker.Models
{
    public class ReleaseCandidateList
    {
        public List<ReleaseCandidate> Items { get; set; }
        public int Page { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
    }
}