using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountUpdated_For_FullAccount
{
    private readonly INostify _nostify;
    private readonly HttpClient _httpClient;

    public OnAccountUpdated_For_FullAccount(INostify nostify, HttpClient httpClient)
    {
        this._nostify = nostify;
        _httpClient = httpClient;
    }

    [Function(nameof(OnAccountUpdated_For_FullAccount))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Update_Account",
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
                var updatedProj = await projectionContainer.ApplyAndPersistAsync<FullAccount>(newEvent);
                await updatedProj.InitAsync(_nostify, _httpClient);
            }                       

        }
        catch (Exception e)
        {
            await _nostify.HandleUndeliverableAsync(nameof(OnAccountUpdated_For_FullAccount), e.Message, newEvent);
        }

        
    }
    
}

