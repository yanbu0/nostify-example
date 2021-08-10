using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using nostify;
using System.Linq;
using Microsoft.Azure.Cosmos.Linq;

namespace nostify_example
{
    public class CreateAccount
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public CreateAccount(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("CreateAccount")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] BankAccount account, HttpRequest httpRequest,
            ILogger log)
        {
            var peContainer = await _nostify.GetPersistedEventsContainerAsync();
            // peContainer.GetItemLinqQueryable<BankAccount>()
                

            PersistedEvent pe = new PersistedEvent(AggregateCommand.Create, $"{BankAccount.aggregateType}||{account.id.ToString()}", account);
            await _nostify.PersistAsync(pe);

            return new OkObjectResult(new{ message = $"Account for {account.customerName} was created"});
        }
    }
}
