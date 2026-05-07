using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Employee_Service;

public class OnEmployeeDeleted
{
    private readonly INostify _nostify;
    private readonly ILogger<OnEmployeeDeleted> _logger;
    
    public OnEmployeeDeleted(INostify nostify, ILogger<OnEmployeeDeleted> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(OnEmployeeDeleted))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Delete_Employee",
                #if DEBUG
                Protocol = BrokerProtocol.NotSet,
                AuthenticationMode = BrokerAuthenticationMode.NotSet,
                #else
                Username = "KafkaApiKey",
                Password = "KafkaApiSecret",
                Protocol =  BrokerProtocol.SaslSsl,
                AuthenticationMode = BrokerAuthenticationMode.Plain,
                #endif
                ConsumerGroup = "Employee")] NostifyKafkaTriggerEvent triggerEvent)
    {
        await DefaultEventHandlers.HandleAggregateEventAsync<Employee>(_nostify, triggerEvent);
    }
}

