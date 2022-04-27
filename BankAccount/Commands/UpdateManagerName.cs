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
using System.Linq;
using nostify;

namespace nostify_example
{
    public class UpdateManagerName
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public UpdateManagerName(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("UpdateManagerName")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Guid managerId = Guid.Parse(req.Query["id"]);
            string newManagerName = req.Query["name"].ToString();
            var updatedManager = new {
                accountManagerId = managerId,
                accountManagerName = newManagerName
            };

            //Query currentState container to get the aggregate root id of all accounts with this manager
            var currentStateContainer = await _nostify.GetCurrentStateContainerAsync();
            List<Guid> accountIdsWithManager = (await currentStateContainer
                .GetItemLinqQueryable<BankAccount>()
                .Where(a => a.accountManagerId == updatedManager.accountManagerId)
                .ReadAllAsync<BankAccount>())
                .Select(a => a.id)
                .ToList();


            accountIdsWithManager.ForEach(async guid => {
                PersistedEvent pe = _nostify.CreateNewPersistedEvent(BankAccountCommand.UpdateManagerName, guid, updatedManager);
                await _nostify.PersistAsync(pe);
            });
        }
    }
}
