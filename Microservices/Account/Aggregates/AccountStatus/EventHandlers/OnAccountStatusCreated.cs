using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountStatusCreated
{
    private readonly INostify _nostify;
    
    public OnAccountStatusCreated(INostify nostify)
    {
        this._nostify = nostify;
    }

    [Function(nameof(OnAccountStatusCreated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Create_AccountStatus",
                #if DEBUG
                Protocol = BrokerProtocol.NotSet,
                AuthenticationMode = BrokerAuthenticationMode.NotSet,
                #else
                Username = "KafkaApiKey",
                Password = "KafkaApiSecret",
                Protocol =  BrokerProtocol.SaslSsl,
                AuthenticationMode = BrokerAuthenticationMode.Plain,
                #endif
                ConsumerGroup = "AccountStatus")] NostifyKafkaTriggerEvent triggerEvent,
        ILogger log)
    {
        Event? newEvent = triggerEvent.GetEvent();
        try
        {
            if (newEvent != null)
            {
                //Update aggregate current state projection
                Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<AccountStatus>();
                await currentStateContainer.ApplyAndPersistAsync<AccountStatus>(newEvent);
            }                           
        }
        catch (Exception e)
        {
            await _nostify.HandleUndeliverableAsync(nameof(OnAccountStatusCreated), e.Message, newEvent);
        }

        
    }
    
}

