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
            databaseName: "BankAccount_DB",
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

                        //We only run this code for BankAccountCommand.UpdateManagerName
                        if (pe.command == BankAccountCommand.UpdateManagerName)
                        {
                            Container baDetailsContainer = await _nostify.GetProjectionContainerAsync(BankAccountDetails.containerName);

                            string mgrid = ((dynamic)pe.payload).accountManagerId;
                            Guid accountManagerId = Guid.Parse(mgrid);

                            //Get List of detail records to update
                            List<BankAccountDetails> baDetailsToUpdate = await baDetailsContainer.GetItemLinqQueryable<BankAccountDetails>()
                                .Where(b => b.accountManagerId == accountManagerId)
                                .ReadAllAsync();

                            //Update the name
                            baDetailsToUpdate.ForEach(async b => {
                                b.Apply(pe);
                                await baDetailsContainer.UpsertItemAsync<BankAccountDetails>(b);
                            });
                        }
                        
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
