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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace nostify_example
{
    public class OnAggregateRootUpdatedpdate
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public OnAggregateRootUpdatedpdate(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("OnAggregateRootUpdated")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "BankAccount_DB",
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
                        JObject payload = (JObject)pe.payload;
                        Guid bankAccountId = payload.GetValue<Guid>("id");
                        
                        //Update aggregate current state
                        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync();
                        BankAccount account = (await currentStateContainer
                            .GetItemLinqQueryable<BankAccount>()
                            .Where(ba => ba.id == bankAccountId)
                            .ReadAllAsync<BankAccount>())
                            .FirstOrDefault() ?? new BankAccount();
                        account.Apply(pe);
                        await currentStateContainer.UpsertItemAsync<BankAccount>(account);

                        //Update BankAccountDetails projection
                        Container baDetailsContainer = await _nostify.GetProjectionContainerAsync(BankAccountDetails.containerName); 
                        BankAccountDetails deets = (await baDetailsContainer.GetItemLinqQueryable<BankAccountDetails>()
                            .Where(b => b.id == bankAccountId)
                            .ReadAllAsync())
                            .FirstOrDefault() ?? new BankAccountDetails();
                        deets.Apply(pe);
                        await baDetailsContainer.UpsertItemAsync<BankAccountDetails>(deets);
                        

                        //TODO: this needs to go into projection event handler
                        // if (pe.command == BankAccountCommand.ProcessTransaction)
                        // {
                        //     await signalRMessages.AddAsync(
                        //         new SignalRMessage
                        //         {
                        //             Target = "transactionAdded",
                        //             Arguments = new[] { pe.payload }
                        //         });
                        // }

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
