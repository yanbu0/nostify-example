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

namespace nostify_example
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
            databaseName: "BankAccount_DB",
            collectionName: "persistedEvents",
            ConnectionStringSetting = "CosmosConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix = "OnPersistedEvent_",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
            [SignalR(HubName = "transactionshub")] IAsyncCollector<SignalRMessage> signalRMessages,
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

                        BankAccount account = await _nostify.RehydrateAggregateAsync<BankAccount>(pe.id);
                        await currentStateContainer.UpsertItemAsync<BankAccount>(account);

                        if (pe.command == BankAccountCommand.ProcessTransaction)
                        {
                            await signalRMessages.AddAsync(
                                new SignalRMessage
                                {
                                    Target = "transactionAdded",
                                    Arguments = new[] { pe.payload }
                                });
                        }

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
