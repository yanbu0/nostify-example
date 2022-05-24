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
    public class BankAccountDetailsHub : ServerlessHub
    {

        private readonly Nostify _nostify;

        public BankAccountDetailsHub(Nostify nostify)
        {

            this._nostify = nostify;
        }

        [FunctionName("negotiate")]
        public SignalRConnectionInfo Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req)
        {
            return Negotiate();
        }

        [FunctionName(nameof(AccountSelected))]
        public async Task AccountSelected([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, 
            [SignalR(HubName = "bankAccountDetailsHub")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            Guid id;
            if (Guid.TryParse(req.Query["bankAccountId"], out id)){
                var deetsContainer = await _nostify.GetProjectionContainerAsync(BankAccountDetails.containerName);
                var details = (await deetsContainer.GetItemLinqQueryable<BankAccountDetails>().Where(b => b.id == id)
                    .ReadAllAsync())
                    .SingleOrDefault();

                await signalRMessages.AddAsync(
                            new SignalRMessage
                            {
                                Target = "bankAccountDetailUpdated",
                                Arguments = new object[] { details }
                            });
            }
            
        }
    }
}