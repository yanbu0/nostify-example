using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountStatusUpdated
{
    private readonly INostify _nostify;
    private readonly ILogger<OnAccountStatusUpdated> _logger;

    public OnAccountStatusUpdated(INostify nostify, ILogger<OnAccountStatusUpdated> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(OnAccountStatusUpdated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Update_AccountStatus",
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

