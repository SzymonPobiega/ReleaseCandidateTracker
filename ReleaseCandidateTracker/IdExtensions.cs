namespace ReleaseCandidateTracker
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

        public static string MakeDeploymentScriptKey(this string version)
        {
            return string.Format("deploy-{0}.ps1", version);
        }

        public static string MakeReleaseNotesKey(this string version)
        {
            return string.Format("release-notes-{0}.txt", version);
        }
    }
}