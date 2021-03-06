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

namespace AccountManager_Service
{
    public class CreateAccountManager
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public CreateAccountManager(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("CreateAccountManager")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] AccountManager aggregate, HttpRequest httpRequest,
            ILogger log)
        {
            PersistedEvent pe = new PersistedEvent(AccountManagerCommand.Create, aggregate.id, aggregate);
            await _nostify.PersistAsync(pe);

            return new OkObjectResult(new{ message = $"Aggregate {aggregate.id} was created"});
        }
    }
}
