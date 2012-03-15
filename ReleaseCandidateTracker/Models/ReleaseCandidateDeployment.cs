using System;

namespace ReleaseCandidateTracker.Models
{
    public class ReleaseCandidateDeployment
    {
        public DateTime Date { get; set; }
        public string Environment { get; set; }
    }
}