using System;
using Raven.Client;
using Raven.Client.Embedded;

namespace ReleaseCandidateTracker.Infrastructure
{
    public static class Database
    {
        private static IDocumentStore storeInstance;

        public static IDocumentStore Instance
        {
            get
            {
                if (storeInstance == null)
                {
                    throw new InvalidOperationException("Document store has not been initialized.");
                }
                return storeInstance;
            }
        }

        public static void Initialize()
        {
            var embeddableDocumentStore = new EmbeddableDocumentStore {DataDirectory = @"~\App_Data\Database"};
            embeddableDocumentStore.Initialize();
            storeInstance = embeddableDocumentStore;
        }
    }
}