using System;
using System.Collections.Generic;

namespace ReleaseCandidateTracker.Models
{
    public class ReleaseCandidate
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public DateTime CreationDate { get; set; }
        public ReleaseCandidateState State { get; set; }
        public string VersionNumber { get; set; }
        public List<ReleaseCandidateHistoryItem> History { get; set; }
        public List<ReleaseCandidateDeployment> Deployments { get; set; }

        public ReleaseCandidate()
        {
            History = new List<ReleaseCandidateHistoryItem>();
            Deployments = new List<ReleaseCandidateDeployment>();
        }

        public void UpdateState(ReleaseCandidateState newState)
        {
            History.Add(new ReleaseCandidateHistoryItem
                            {
                                Date = DateTime.UtcNow,
                                StateChange = string.Format("Changed state from {0} to {1}.", State, newState)
                            });
            State = newState;
        }

        public void MarkAsDeployed(Environment environment)
        {
            var deploymentDate = DateTime.UtcNow;
            History.Add(new ReleaseCandidateHistoryItem
                            {
                                Date = deploymentDate,
                                StateChange = string.Format("Deployed to {0} environment.", environment)
                            });
            Deployments.Add(new ReleaseCandidateDeployment
                                {
                                    Date = deploymentDate,
                                    Environment = environment
                                });
        }

        public int GetLocalId()
        {
            return int.Parse(Id.Split('/')[1]);
        }

        public static string MakeId(int localId)
        {
            return string.Format("ReleaseCandidates/{0}", localId);
        }
    }
}