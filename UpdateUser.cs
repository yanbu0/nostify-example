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
    public class UpdateUser
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public UpdateUser(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("UpdateUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            for(int i = 1; i < 5; i++)
            {
                var update = new {
                    userName = "yanbu0_" + i.ToString()
                };

                PersistedEvent pe = new PersistedEvent(UserCommand.UpdateUser, $"{UserCommand.UpdateUser.AggregateType}||{1.ToString()}", update);
                await _nostify.PersistAsync(pe);
            }

            User user = await _nostify.BuildProjectionAsync<User>("User||1");

            return new OkObjectResult(JsonConvert.SerializeObject(user));
        }
    }
}
