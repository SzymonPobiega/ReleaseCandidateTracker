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

        public static string MakeCustomDocumentKey(this string version, string userSuppliedKey)
        {
            return string.Format("{0}-{1}", version, userSuppliedKey);
        }
    }
}