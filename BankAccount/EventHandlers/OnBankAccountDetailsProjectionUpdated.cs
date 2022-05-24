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
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace nostify_example
{
    public class OnBankAccountDetailsProjectionUpdated
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public OnBankAccountDetailsProjectionUpdated(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("OnBankAccountDetailsProjectionUpdated")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "BankAccount_DB",
            collectionName: "BankAccountDetails",
            ConnectionStringSetting = "CosmosConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionPrefix = "OnBankAccountDetailsProjectionUpdated_",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
            [SignalR(HubName = "bankAccountDetailsHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            if (input != null)
            {
                foreach (Document doc in input)
                {
                    BankAccountDetails deets = JsonConvert.DeserializeObject<BankAccountDetails>(doc.ToString());
                        
                    await signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "bankAccountDetailUpdated",
                            Arguments = new[] { deets }
                        });

                }
            }
        }
    }
}
