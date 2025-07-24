using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Employee_Service;

public class OnEmployeeUpdated
{
    private readonly INostify _nostify;

    public OnEmployeeUpdated(INostify nostify)
    {
        this._nostify = nostify;
    }

    [Function(nameof(OnEmployeeUpdated))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Update_Employee",
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
            await _nostify.HandleUndeliverableAsync(nameof(OnEmployeeUpdated), e.Message, newEvent);
        }

        
    }
    
}

