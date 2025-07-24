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
    
    
    public OnEmployeeDeleted(INostify nostify)
    {
        this._nostify = nostify;
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
                ConsumerGroup = "Employee")] NostifyKafkaTriggerEvent triggerEvent,
        ILogger log)
    {
        Event? newEvent = triggerEvent.GetEvent();
        try
        {
            if (newEvent != null)
            {
                //Update aggregate current state projection
                Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Employee>();
                await currentStateContainer.ApplyAndPersistAsync<Employee>(newEvent);
            }
        }
        catch (Exception e)
        {
            await _nostify.HandleUndeliverableAsync(nameof(OnEmployeeDeleted), e.Message, newEvent);
        }

        
        
    }
}

