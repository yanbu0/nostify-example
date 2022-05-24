using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using nostify;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

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

                        if (pe.command == NostifyCommand.Create ||
                            pe.command == NostifyCommand.Update ||
                            pe.command == NostifyCommand.Delete ||
                            pe.command == BankAccountCommand.ProcessTransaction)
                        {
                             Guid bankAccountId = Guid.Parse(pe.partitionKey);
                        
                            //Update aggregate current state projection
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

                            //Check if accountManagerId updated, then query the name
                            dynamic payload = (dynamic)pe.payload;
                            string acctMgrId = payload.accountManagerId;
                            if (pe.command == NostifyCommand.Update && acctMgrId != null){
                                var resp = await _client.GetAsync($"http://localhost:7072/api/GetAccountManager?id={acctMgrId}");
                                dynamic accountManager = JsonConvert.DeserializeObject(await resp.Content.ReadAsStringAsync());
                                string name = accountManager.name;
                                payload.accountManagerName = name;
                            }

                            deets.Apply(pe);
                            await baDetailsContainer.UpsertItemAsync<BankAccountDetails>(deets);
                        }

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
