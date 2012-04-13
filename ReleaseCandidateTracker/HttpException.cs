using System;

namespace ReleaseCandidateTracker
{
    [Serializable]
    public class HttpException : Exception
    {
        private readonly int reponseCode;

        public HttpException(int reponseCode, string message)
            : base(message)
        {
            this.reponseCode = reponseCode;
        }

        public int ReponseCode
        {
            get { return reponseCode; }
        }
    }
}