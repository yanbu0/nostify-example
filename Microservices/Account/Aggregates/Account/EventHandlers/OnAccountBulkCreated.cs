using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnAccountBulkCreated
{
    private readonly INostify _nostify;
    private readonly ILogger<OnAccountBulkCreated> _logger;
    
    public OnAccountBulkCreated(INostify nostify, ILogger<OnAccountBulkCreated> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(OnAccountBulkCreated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "BulkCreate_Account",
                ConsumerGroup = "Account",
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
        int createdCount = await DefaultEventHandlers.HandleAggregateBulkCreateEventAsync<Account>(_nostify, events);
        _logger.LogInformation("{Handler} processed {Count} records", nameof(OnAccountBulkCreated), createdCount);
    }    
}

