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
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Collections.Generic;
using nostify;

namespace AccountManager_Service
{
    public class GetAllAccountManagers
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public GetAllAccountManagers(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("GetAllAccountManagers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync();
            List<AccountManager> accountMgrList = await currentStateContainer
                                .GetItemLinqQueryable<AccountManager>()
                                .ReadAllAsync<AccountManager>();

            return new OkObjectResult(accountMgrList);
        }
    }
}
