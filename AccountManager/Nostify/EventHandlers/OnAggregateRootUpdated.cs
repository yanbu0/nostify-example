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
using System.Linq;

namespace AccountManager_Service
{
    public class OnAggregateRootUpdated
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public OnAggregateRootUpdated(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;;
        }

        [FunctionName("OnAggregateRootUpdated")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "AccountManager_DB",
            collectionName: "persistedEvents",
            ConnectionStringSetting = "CosmosConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix = "OnAggregateRootUpdated_",
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

                        if (pe.command == NostifyCommand.Create ||
                            pe.command == NostifyCommand.Update ||
                            pe.command == NostifyCommand.Delete ||
                            pe.command == AccountManagerCommand.UpdateName)
                        {
                             Guid accountManagerId = pe.id;
                        
                            //Update aggregate current state projection
                            Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync();
                            AccountManager accountMgr = (await currentStateContainer
                                .GetItemLinqQueryable<AccountManager>()
                                .Where(acctMgr => acctMgr.id == accountManagerId)
                                .ReadAllAsync<AccountManager>())
                                .FirstOrDefault() ?? new AccountManager();
                            accountMgr.Apply(pe);
                            await currentStateContainer.UpsertItemAsync<AccountManager>(accountMgr);
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
