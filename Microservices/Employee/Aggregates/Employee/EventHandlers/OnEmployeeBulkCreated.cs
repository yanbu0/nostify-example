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
    
    public OnEmployeeBulkCreated(INostify nostify)
    {
        this._nostify = nostify;
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
                IsBatched = true)] string[] events,
        ILogger log)
    {
        try
        {
            Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Employee>();    
            await currentStateContainer.BulkCreateFromKafkaTriggerEventsAsync<Employee>(events);                         
        }
        catch (Exception e)
        {
            events.ToList().ForEach(async eventStr =>
            {
                Event @event = JsonConvert.DeserializeObject<NostifyKafkaTriggerEvent>(eventStr)?.GetEvent() ?? throw new NostifyException("Event is null");
                await _nostify.HandleUndeliverableAsync(nameof(OnEmployeeBulkCreated), e.Message, @event);
            });            
        }        
    }    
}

