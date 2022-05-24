using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using nostify;

namespace nostify_example
{

    public class UpdateBankAccount
    {

        private readonly HttpClient _client;
        private readonly Nostify _nostify;
        public UpdateBankAccount(HttpClient httpClient, Nostify nostify)
        {
            this._client = httpClient;
            this._nostify = nostify;
        }

        [FunctionName("UpdateBankAccount")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] dynamic upd, HttpRequest httpRequest,
            ILogger log)
        {
            Guid aggRootId = Guid.Parse(upd.id.ToString());
            PersistedEvent pe = new PersistedEvent(NostifyCommand.Update, aggRootId, upd);
            await _nostify.PersistAsync(pe);

            return new OkObjectResult(new{ message = $"Account {upd.id} was updated"});
        }
    }
}
