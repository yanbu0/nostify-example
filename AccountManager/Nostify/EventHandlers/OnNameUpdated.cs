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
using Newtonsoft.Json.Linq;

namespace AccountManager_Service
{
    public class OnNameUpdated
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public OnNameUpdated(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;;
        }

        [FunctionName("OnNameUpdated")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "AccountManager_DB",
            collectionName: "persistedEvents",
            ConnectionStringSetting = "CosmosConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix = "OnNameUpdated_",
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

                        if (pe.command == AccountManagerCommand.UpdateName)
                        {
                            dynamic payload = (dynamic)pe.payload;
                            //Post to BankAccount service to update projections
                            //This uses defaults for the url, if you have modified the port you may need to update this
                            HttpClient client = new HttpClient();
                            var response = await client.PostAsync($"http://localhost:7071/api/UpdateManagerName?id={payload.id}&name={payload.name}",null);
                        }

                    }
                    catch (Exception e)
                    {
                        await _nostify.HandleUndeliverableAsync("OnAggregateRootUpdated", e.Message, pe);
                    }

                }
            }
        }
    }
}
