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
    private readonly ILogger<OnAccountStatusCreated> _logger;
    
    public OnAccountStatusCreated(INostify nostify, ILogger<OnAccountStatusCreated> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
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
                ConsumerGroup = "AccountStatus")] NostifyKafkaTriggerEvent triggerEvent)
    {
        await DefaultEventHandlers.HandleAggregateEventAsync<AccountStatus>(_nostify, triggerEvent);
    }
    
}

