using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountDeleted
{
    private readonly INostify _nostify;
    private readonly ILogger<OnAccountDeleted> _logger;
    
    public OnAccountDeleted(INostify nostify, ILogger<OnAccountDeleted> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(OnAccountDeleted))]
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
                ConsumerGroup = "Account")] NostifyKafkaTriggerEvent triggerEvent)
    {
        await DefaultEventHandlers.HandleAggregateEventAsync<Account>(_nostify, triggerEvent);
    }
}

