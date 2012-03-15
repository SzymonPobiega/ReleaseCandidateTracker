using System;

namespace ReleaseCandidateTracker.Models
{
    public class EnvironmentHistoryItem
    {
        public DateTime Date { get; set; }
        public bool Success { get; set; }
        public string Version { get; set; }
    }
}