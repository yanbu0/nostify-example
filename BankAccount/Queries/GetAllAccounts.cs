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
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using nostify;

namespace nostify_example
{
    public class GetAllAccounts
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public GetAllAccounts(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("GetAllAccounts")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync();
            List<BankAccount> accountList = await currentStateContainer
                                .GetItemLinqQueryable<BankAccount>()
                                .ReadAllAsync<BankAccount>();


            return new OkObjectResult(accountList);
        }
    }
}
