using System;
using System.Collections.Generic;

namespace ReleaseCandidateTracker.Models
{
    public class DeploymentEnvironment
    {
        public string Name { get; set; }
        public string CurrentVersion { get; set; }
        public IList<EnvironmentHistoryItem> History { get; set; }

        public DeploymentEnvironment()
        {
            History = new List<EnvironmentHistoryItem>();
        }

        public void RecordDeployment(string version, bool success)
        {
            CurrentVersion = version;
            History.Add(new EnvironmentHistoryItem
                            {
                                Date = DateTime.UtcNow,
                                Success = success,
                                Version = version
                            });
        }
    }
}