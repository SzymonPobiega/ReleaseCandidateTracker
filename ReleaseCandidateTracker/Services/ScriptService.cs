using System.IO;
using Raven.Client;
using Raven.Json.Linq;

namespace ReleaseCandidateTracker.Services
{
    public class ScriptService
    {
        private readonly IDocumentSession documentSession;

        public ScriptService(IDocumentSession documentSession)
        {
            this.documentSession = documentSession;
        }

        public void AttachScript(string versionNumber, Stream fileContents)
        {
            var metadata = new RavenJObject();
            documentSession.Advanced.DatabaseCommands.PutAttachment(versionNumber, null, fileContents, metadata);
        }

        public Stream GetScript(string versionNumber)
        {
            var attachment = documentSession.Advanced.DatabaseCommands.GetAttachment(versionNumber);
            return attachment != null 
                ? attachment.Data() 
                : null;
        }
    }
}