using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountStatusBulkCreated
{
    private readonly INostify _nostify;
    private readonly ILogger<OnAccountStatusBulkCreated> _logger;
    
    public OnAccountStatusBulkCreated(INostify nostify, ILogger<OnAccountStatusBulkCreated> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(OnAccountStatusBulkCreated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "BulkCreate_AccountStatus",
                ConsumerGroup = "AccountStatus",
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
        int createdCount = await DefaultEventHandlers.HandleAggregateBulkCreateEventAsync<AccountStatus>(_nostify, events);
        _logger.LogInformation("{Handler} processed {Count} records", nameof(OnAccountStatusBulkCreated), createdCount);
    }    
}

