using System.IO;
using System.Web.Mvc;
using Raven.Client;
using Raven.Json.Linq;
using ReleaseCandidateTracker.Models;

namespace ReleaseCandidateTracker
{
    public static class DocumentSessionExtensions
    {
        private const string SettingsKey = "settings";

        public static Settings GetSettings(this IDocumentSession documentSession)
        {
            var existingSettings = documentSession.Load<Settings>(SettingsKey);
            if (existingSettings == null)
            {
                existingSettings = new Settings();
                documentSession.Store(existingSettings, SettingsKey);
            }
            return existingSettings;
        }

        public static void PutAttachment(this IDocumentSession documentSession, string key, string contentType, Stream fileContents)
        {
            var metadata = new RavenJObject();
            metadata["content-type"] = contentType;
            documentSession.Advanced.DatabaseCommands.PutAttachment(key, null, fileContents, metadata);
        }

        public static ActionResult GetAttachmentResult(this IDocumentSession documentSession, string key)
        {
            var attachment = documentSession.Advanced.DatabaseCommands.GetAttachment(key);
            if (attachment != null)
            {
                var result = new FileStreamResult(attachment.Data(), attachment.Metadata["content-type"].Value<string>())
                {
                    FileDownloadName = key
                };
                return result;
            }
            return null;
        }

        public static ReleaseCandidate GetCandidate(this IDocumentSession documentSession, string versionNumber)
        {
            var result = documentSession.Load<ReleaseCandidate>(versionNumber.MakeCandidateId());
            if(result == null)
            {
                throw new HttpException(404, string.Format("Release candidate {0} does not exist", versionNumber));
            }
            return result;
        }

        public static DeploymentEnvironment GetEnvironment(this IDocumentSession documentSession, string name)
        {
            var result = documentSession.Load<DeploymentEnvironment>(name.MakeEnvironmentId());
            if (result == null)
            {
                throw new HttpException(404, string.Format("Environment {0} does not exist", name));
            }
            return result;
        }
    }
}