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
    
    public OnAccountBulkDeletedFor_FullAccount(INostify nostify, HttpClient httpClient)
    {
        this._nostify = nostify;
        _httpClient = httpClient;
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
                IsBatched = true)] string[] events,
        ILogger log)
    {
        try
        {
            Container bulkDeleteContainer = await _nostify.GetBulkProjectionContainerAsync<FullAccount>();
            await bulkDeleteContainer.BulkDeleteFromEventsAsync<FullAccount>(events);
        }
        catch (Exception e)
        {
            events.ToList().ForEach(async eventStr =>
            {
                Event @event = JsonConvert.DeserializeObject<NostifyKafkaTriggerEvent>(eventStr)?.GetEvent() ?? throw new NostifyException("Event is null");
                await _nostify.HandleUndeliverableAsync(nameof(OnAccountBulkDeletedFor_FullAccount), e.Message, @event);
            });
        }

        
    }
    
}

