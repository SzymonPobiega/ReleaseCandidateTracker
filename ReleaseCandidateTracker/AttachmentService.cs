using System.IO;
using System.Web.Mvc;
using Raven.Client;
using Raven.Json.Linq;

namespace ReleaseCandidateTracker
{
    public static class AttachmentExtensions
    {
        public static void PutAttachment(this IDocumentSession documentSession, string key, Stream fileContents)
        {
            var metadata = new RavenJObject();
            documentSession.Advanced.DatabaseCommands.PutAttachment(key, null, fileContents, metadata);
        }

        public static ActionResult GetAttachmentResult(this IDocumentSession documentSession, string key, string contentType)
        {
            var attachment = documentSession.Advanced.DatabaseCommands.GetAttachment(key);
            if (attachment != null)
            {
                var result = new FileStreamResult(attachment.Data(), contentType)
                                 {
                                     FileDownloadName = key
                                 };
                return result;
            }
            return null;
        }
    }
}