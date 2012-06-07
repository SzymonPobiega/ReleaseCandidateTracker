namespace ReleaseCandidateTracker.Models
{
    public class Settings
    {
        public string DeploymentWorkingDirectory { get; set; }
        public int PageSize { get; set; }

        public Settings()
        {
            PageSize = 20;
        }
    }
}