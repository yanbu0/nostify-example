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

namespace nostify_example
{
    public class OnPersistedEvent
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public OnPersistedEvent(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("OnPersistedEvent")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "AimsUsers",
            collectionName: "persistedEvents",
            ConnectionStringSetting = "AzureWebJobsStorage",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                PersistedEvent pe = JsonConvert.DeserializeObject<PersistedEvent>(input[0].ToString());
                var db = await _nostify.Repository.GetDatabaseAsync();
                Container currentStateContainer = await db.CreateContainerIfNotExistsAsync("currentState","/partitionKey");

                User user = await _nostify.BuildProjectionAsync<User>(pe.partitionKey);
                await currentStateContainer.UpsertItemAsync<User>(user, pe.partitionKey.ToPartitionKey());
            }
        }
    }
}
