using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Employee_Service;

public class OnEmployeeCreated
{
    private readonly INostify _nostify;
    private readonly ILogger<OnEmployeeCreated> _logger;
    
    public OnEmployeeCreated(INostify nostify, ILogger<OnEmployeeCreated> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(OnEmployeeCreated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Create_Employee",
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

