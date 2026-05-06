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
    private readonly ILogger<OnAccountUpdated_For_FullAccount> _logger;

    public OnAccountUpdated_For_FullAccount(INostify nostify, HttpClient httpClient, ILogger<OnAccountUpdated_For_FullAccount> logger)
    {
        this._nostify = nostify;
        _httpClient = httpClient;
        this._logger = logger;
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
                ConsumerGroup = "FullAccount")] NostifyKafkaTriggerEvent triggerEvent)
    {
        await DefaultEventHandlers.HandleProjectionEventAsync<FullAccount>(_nostify, triggerEvent, _httpClient);
    }
    
}

