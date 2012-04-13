namespace ReleaseCandidateTracker.Services
{
    public static class IdExtensions
    {
        public static string MakeCandidateId(this string version)
        {
            return "candidates/" + version;
        }

        public static string MakeEnvironmentId(this string name)
        {
            return "environments/" + name;
        }
    }
}