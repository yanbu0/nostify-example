using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountBulkCreated
{
    private readonly INostify _nostify;
    
    public OnAccountBulkCreated(INostify nostify)
    {
        this._nostify = nostify;
    }

    [Function(nameof(OnAccountBulkCreated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "BulkCreate_Account",
                ConsumerGroup = "Account",
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
            Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Account>();    
            await currentStateContainer.BulkCreateFromKafkaTriggerEventsAsync<Account>(events);                         
        }
        catch (Exception e)
        {
            events.ToList().ForEach(async eventStr =>
            {
                Event @event = JsonConvert.DeserializeObject<NostifyKafkaTriggerEvent>(eventStr)?.GetEvent() ?? throw new NostifyException("Event is null");
                await _nostify.HandleUndeliverableAsync(nameof(OnAccountBulkCreated), e.Message, @event);
            });            
        }        
    }    
}

