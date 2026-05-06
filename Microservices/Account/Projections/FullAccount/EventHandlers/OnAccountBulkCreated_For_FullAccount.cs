using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountBulkCreated_For_FullAccount
{
    private readonly INostify _nostify;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OnAccountBulkCreated_For_FullAccount> _logger;
    
    public OnAccountBulkCreated_For_FullAccount(INostify nostify, HttpClient httpClient, ILogger<OnAccountBulkCreated_For_FullAccount> logger)
    {
        this._nostify = nostify;
        _httpClient = httpClient;
        this._logger = logger;
    }

    [Function(nameof(OnAccountBulkCreated_For_FullAccount))]
    public async Task Run([KafkaTrigger("BrokerList",
                "BulkCreate_Account",
                ConsumerGroup = "FullAccount",
                #if DEBUG
                Protocol = BrokerProtocol.NotSet,
                AuthenticationMode = BrokerAuthenticationMode.NotSet,
                #else
                Username = "KafkaApiKey",
                Password = "KafkaApiSecret",
                Protocol =  BrokerProtocol.SaslSsl,
                AuthenticationMode = BrokerAuthenticationMode.Plain,
                #endif
                IsBatched = true)] string[] events)
    {
        int createdCount = await DefaultEventHandlers.HandleProjectionBulkCreateEventAsync<FullAccount>(_nostify, events);
        _logger.LogInformation("{Handler} processed {Count} records", nameof(OnAccountBulkCreated_For_FullAccount), createdCount);
    }
    
}

