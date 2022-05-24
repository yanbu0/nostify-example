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

namespace nostify_example
{
    public class ProcessTransaction
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public ProcessTransaction(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("ProcessTransaction")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Guid accountId = Guid.Parse(req.Query["id"]);
            decimal amt = decimal.Parse(req.Query["amount"]);
            var trans = new Transaction()
            {
                amount = amt
            };

            NostifyCommand command = BankAccountCommand.ProcessTransaction;

            PersistedEvent pe = new PersistedEvent(command, accountId, trans);
            await _nostify.PersistAsync(pe);
        }
    }
}
