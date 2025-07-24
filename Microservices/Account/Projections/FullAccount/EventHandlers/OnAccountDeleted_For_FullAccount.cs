using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountDeleted_For_FullAccount
{
    private readonly INostify _nostify;
    
    
    public OnAccountDeleted_For_FullAccount(INostify nostify)
    {
        this._nostify = nostify;
    }

    [Function(nameof(OnAccountDeleted_For_FullAccount))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Delete_Account",                
                #if DEBUG
                Protocol = BrokerProtocol.NotSet,
                AuthenticationMode = BrokerAuthenticationMode.NotSet,
                #else
                Username = "KafkaApiKey",
                Password = "KafkaApiSecret",
                Protocol =  BrokerProtocol.SaslSsl,
                AuthenticationMode = BrokerAuthenticationMode.Plain,
                #endif
                ConsumerGroup = "FullAccount")] NostifyKafkaTriggerEvent triggerEvent,
        ILogger log)
    {
        Event? newEvent = triggerEvent.GetEvent();
        try
        {
            if (newEvent != null)
            {
                //Update projection container
                Container projectionContainer = await _nostify.GetProjectionContainerAsync<FullAccount>();
                //Remove from the container.  If you wish to set isDeleted instead, remove the code below and ApplyAndPersist the Event
                await projectionContainer.DeleteItemAsync<FullAccount>(newEvent.id);
            }
        }
        catch (Exception e)
        {
            await _nostify.HandleUndeliverableAsync(nameof(OnAccountDeleted_For_FullAccount), e.Message, newEvent);
        }

        
        
    }
}

