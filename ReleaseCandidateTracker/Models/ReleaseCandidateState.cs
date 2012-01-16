namespace ReleaseCandidateTracker.Models
{
    public enum ReleaseCandidateState
    {
        UnitTestsPassed,
        IntegrationTestsFailed,
        IntegrationTestsPassed,
        AcceptanceTestsFailed,
        AcceptanceTestsPassed,
        LoadTestsFailed,
        LoadTestsPassed,
        ExploratoryTestsFailed,
        ExploratoryTestsPassed
    }
}