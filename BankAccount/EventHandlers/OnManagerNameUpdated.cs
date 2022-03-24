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
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Linq;
using System.Linq.Expressions;

namespace nostify_example
{
    public class OnManagerNameUpdated
    {
        private const string _functionName = "OnManagerNameUpdated";
        private const string prefix = _functionName+"_";

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public OnManagerNameUpdated(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName(_functionName)]
        public async Task Run([CosmosDBTrigger(
            databaseName: "AccountManager_DB",
            collectionName: "persistedEvents",
            ConnectionStringSetting = "CosmosConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix = prefix,
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
                        Container bawmContainer = await _nostify.GetProjectionContainerAsync(BankAccountDetails.containerName);

                        JObject payload = (JObject)pe.payload;

                        List<BankAccountDetails> bawmToUpdate = (await bawmContainer.GetItemLinqQueryable<BankAccountDetails>()
                            .Where(b => b.accountManagerId == payload["id"].Value<Guid>())
                            .ReadAllAsync())
                            .ToList();

                        bawmToUpdate.ForEach(async b => {
                            //b.accountManagerName
                            await bawmContainer.UpsertItemAsync<BankAccountDetails>(b);
                        });
                    }
                    catch (Exception e)
                    {
                        await _nostify.HandleUndeliverableAsync(_functionName, e.Message, pe);
                    }

                }
            }
        }
    }
}
