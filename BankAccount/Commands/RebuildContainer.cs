using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using nostify;
using System.Linq;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;
using System.Collections.Generic;

namespace BankAccount_Service
{
    public class RebuildContainer
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public RebuildContainer(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("RebuildContainer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try{
            string containerName = req.Query["containerName"];
            Container containerToRebuild = await _nostify.GetProjectionContainerAsync(containerName);
            await _nostify.RebuildContainerAsync<BankAccountDetails>(containerToRebuild);    

            return new OkObjectResult(new{ message = $"Container {containerName} was rebuilt."});
            }
            catch (Exception e){
                throw;
            }
        }
    }
}
