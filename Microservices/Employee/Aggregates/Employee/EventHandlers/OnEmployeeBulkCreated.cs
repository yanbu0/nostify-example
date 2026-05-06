using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Employee_Service;

public class OnEmployeeBulkCreated
{
    private readonly INostify _nostify;
    private readonly ILogger<OnEmployeeBulkCreated> _logger;
    
    public OnEmployeeBulkCreated(INostify nostify, ILogger<OnEmployeeBulkCreated> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(OnEmployeeBulkCreated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "BulkCreate_Employee",
                ConsumerGroup = "Employee",
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
        int createdCount = await DefaultEventHandlers.HandleAggregateBulkCreateEventAsync<Employee>(_nostify, events);
        _logger.LogInformation("{Handler} processed {Count} records", nameof(OnEmployeeBulkCreated), createdCount);
    }    
}

