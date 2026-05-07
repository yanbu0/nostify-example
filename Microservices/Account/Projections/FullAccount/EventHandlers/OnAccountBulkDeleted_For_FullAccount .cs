using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountBulkDeletedFor_FullAccount
{
    private readonly INostify _nostify;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OnAccountBulkDeletedFor_FullAccount> _logger;
    
    public OnAccountBulkDeletedFor_FullAccount(INostify nostify, HttpClient httpClient, ILogger<OnAccountBulkDeletedFor_FullAccount> logger)
    {
        this._nostify = nostify;
        _httpClient = httpClient;
        this._logger = logger;
    }

    [Function(nameof(OnAccountBulkDeletedFor_FullAccount))]
    public async Task Run([KafkaTrigger("BrokerList",
                "BulkDelete_Account",
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
        int deletedCount = await DefaultEventHandlers.HandleProjectionBulkDeleteEventAsync<FullAccount>(_nostify, events);
        _logger.LogInformation("{Handler} processed {Count} records", nameof(OnAccountBulkDeletedFor_FullAccount), deletedCount);
    }
    
}

