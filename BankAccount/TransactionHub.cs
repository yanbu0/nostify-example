using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq;
using System;
using nostify;

namespace nostify_example
{
    public class TransactionsHub : ServerlessHub
    {

        private readonly Nostify _nostify;

        public TransactionsHub(Nostify nostify)
        {

            this._nostify = nostify;
        }

        [FunctionName("negotiate")]
        public SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req)
        {
            var test = this;
            return Negotiate();
        }

        [FunctionName("OnConnected")]
        public async Task OnConnected([SignalRTrigger] InvocationContext invocationContext, ILogger logger)
        {

            var currentStateContainer = await _nostify.GetCurrentStateContainerAsync();
            var account = (await currentStateContainer.GetItemLinqQueryable<BankAccount>().Where(b => b.accountId == int.Parse(invocationContext.Query["bankAccountId"]))
                .ReadAllAsync())
                .SingleOrDefault();

            await Clients.Client(invocationContext.ConnectionId).SendAsync("initialConnect",
                account.transactions
            );
            logger.LogInformation($"{invocationContext.ConnectionId} has connected");
        }
    }
}