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
using nostify;

namespace AccountManager_Service
{
    public class GetAccountManager
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public GetAccountManager(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("GetAccountManager")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {

            Guid accountMgrId = Guid.Parse(req.Query["id"]);

            Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync();
            AccountManager accountMgr = (await currentStateContainer
                                .GetItemLinqQueryable<AccountManager>()
                                .Where(acctMgr => acctMgr.id == accountMgrId)
                                .ReadAllAsync<AccountManager>())
                                .FirstOrDefault();

            return new OkObjectResult(accountMgr);
        }
    }
}
