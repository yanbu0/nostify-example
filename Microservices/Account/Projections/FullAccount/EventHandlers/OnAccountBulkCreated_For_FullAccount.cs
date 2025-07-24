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
    
    public OnAccountBulkCreated_For_FullAccount(INostify nostify, HttpClient httpClient)
    {
        this._nostify = nostify;
        _httpClient = httpClient;
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
                IsBatched = true)] string[] events,
        ILogger log)
    {
        try
        {
            Container currentStateContainer = await _nostify.GetBulkProjectionContainerAsync<FullAccount>();
            await currentStateContainer.BulkCreateFromKafkaTriggerEventsAsync<FullAccount>(events);
            await _nostify.InitAllUninitializedAsync<FullAccount>();
        }
        catch (Exception e)
        {
            events.ToList().ForEach(async eventStr =>
            {
                Event @event = JsonConvert.DeserializeObject<NostifyKafkaTriggerEvent>(eventStr)?.GetEvent() ?? throw new NostifyException("Event is null");
                await _nostify.HandleUndeliverableAsync(nameof(OnAccountBulkCreated_For_FullAccount), e.Message, @event);
            });
        }

        
    }
    
}

