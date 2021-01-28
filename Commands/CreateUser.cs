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
    public class CreateUser
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public CreateUser(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("CreateUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] User user, HttpRequest httpRequest,
            ILogger log)
        {
            PersistedEvent pe = new PersistedEvent(AggregateCommand.Create, $"{User.aggregateType}||{user.id.ToString()}", user);
            await _nostify.PersistAsync(pe);

            return new OkObjectResult(new{ message = user.userName + " was created"});
        }
    }
}
