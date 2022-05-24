
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using nostify;
using nostify_example_contracts;

namespace AccountManager_Service
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
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] ManagerNameUpdateContract upd, HttpRequest httpRequest,
            ILogger log)
        {
            var peContainer = await _nostify.GetPersistedEventsContainerAsync();

            PersistedEvent pe = new PersistedEvent(AccountManagerCommand.UpdateName, upd.id, upd);
            await _nostify.PersistAsync(pe);

            return new OkObjectResult(new{ message = $"Name has been changed to {upd.name}"});
        }
    }
}
