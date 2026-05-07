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
    private readonly ILogger<OnAccountDeleted_For_FullAccount> _logger;
    
    public OnAccountDeleted_For_FullAccount(INostify nostify, ILogger<OnAccountDeleted_For_FullAccount> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
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
                ConsumerGroup = "FullAccount")] NostifyKafkaTriggerEvent triggerEvent)
    {
        await DefaultEventHandlers.HandleProjectionEventAsync<FullAccount>(_nostify, triggerEvent, null);
    }
}

