namespace ReleaseCandidateTracker.Models
{
    public class ReleaseCandidateCreateModel
    {
        public string ProductName { get; set; }
        public ReleaseCandidateState State { get; set; }
        public string VersionNumber { get; set; }
    }
}