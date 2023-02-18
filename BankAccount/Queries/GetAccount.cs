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

namespace BankAccount_Service
{
    public class GetAccount
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public GetAccount(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("GetAccount")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            string accountId = req.Query["id"];

            BankAccount currentState = await _nostify.RehydrateAsync<BankAccount>(Guid.Parse(accountId));


            return new OkObjectResult(JsonConvert.SerializeObject(currentState));
        }
    }
}
