using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service_Service;

public class OnAccountStatusBulkCreated
{
    private readonly INostify _nostify;
    
    public OnAccountStatusBulkCreated(INostify nostify)
    {
        this._nostify = nostify;
    }

    [Function(nameof(OnAccountStatusBulkCreated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "BulkCreate_AccountStatus",
                ConsumerGroup = "AccountStatus",
                #if DEBUG
                Protocol = BrokerProtocol.NotSet,
                AuthenticationMode = BrokerAuthenticationMode.NotSet,
                #else
                Username = "KafkaApiKey",
                Password = "KafkaApiSecret",
                Protocol =  BrokerProtocol.SaslSsl,
                AuthenticationMode = BrokerAuthenticationMode.Plain,
                #endif
                IsBatched = true)] string[] events,
        ILogger log)
    {
        try
        {
            Container currentStateContainer = await _nostify.GetBulkCurrentStateContainerAsync<AccountStatus>();
            await currentStateContainer.BulkCreateFromKafkaTriggerEventsAsync<AccountStatus>(events);                         
        }
        catch (Exception e)
        {
            events.ToList().ForEach(async eventStr =>
            {
                Event @event = JsonConvert.DeserializeObject<NostifyKafkaTriggerEvent>(eventStr)?.GetEvent() ?? throw new NostifyException("Event is null");
                await _nostify.HandleUndeliverableAsync(nameof(OnAccountStatusBulkCreated), e.Message, @event);
            });            
        }        
    }    
}

