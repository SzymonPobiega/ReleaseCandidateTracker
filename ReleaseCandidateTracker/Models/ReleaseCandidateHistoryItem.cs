using System;

namespace ReleaseCandidateTracker.Models
{
    public class ReleaseCandidateHistoryItem
    {
        public DateTime Date { get; set; }
        public string StateChange { get; set; }
    }
}