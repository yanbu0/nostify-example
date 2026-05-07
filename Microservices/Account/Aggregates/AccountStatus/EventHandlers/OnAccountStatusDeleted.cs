using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountStatusDeleted
{
    private readonly INostify _nostify;
    private readonly ILogger<OnAccountStatusDeleted> _logger;
    
    public OnAccountStatusDeleted(INostify nostify, ILogger<OnAccountStatusDeleted> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(OnAccountStatusDeleted))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Delete_AccountStatus",
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

