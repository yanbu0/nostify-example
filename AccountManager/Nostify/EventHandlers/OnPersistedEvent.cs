using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using nostify;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace AccountManager_Service
{
    public class OnPersistedEvent
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public OnPersistedEvent(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;;
        }

        [FunctionName("OnPersistedEvent")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "AccountManager_DB",
            collectionName: "persistedEvents",
            ConnectionStringSetting = "CosmosEmulatorConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix = "OnPersistedEvent_",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
            ILogger log)
        {
            if (input != null)
            {
                foreach (Document doc in input)
                {
                    PersistedEvent pe = null;
                    try
                    {
                        pe = JsonConvert.DeserializeObject<PersistedEvent>(doc.ToString());
                        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync();

                        AccountManager account = await _nostify.BuildProjectionAsync<AccountManager>(pe.partitionKey);
                        await currentStateContainer.UpsertItemAsync<AccountManager>(account);

                    }
                    catch (Exception e)
                    {
                        await _nostify.HandleUndeliverableAsync("OnPersistedEvent", e.Message, pe);
                    }

                }
            }
        }
    }
}
