using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using nostify;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json.Linq;

namespace Account_Service;

public class OnEmployeeUpdated_For_FullAccount
{
    private readonly INostify _nostify;
    private readonly HttpClient _httpClient;

    public OnEmployeeUpdated_For_FullAccount(INostify nostify, HttpClient httpClient)
    {
        this._nostify = nostify;
        _httpClient = httpClient;
    }

    [Function(nameof(OnEmployeeUpdated_For_FullAccount))]
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
                ConsumerGroup = "FullAccount")] NostifyKafkaTriggerEvent triggerEvent,
        ILogger log)
    {
        Event? newEvent = triggerEvent.GetEvent();
        try
        {
            if (newEvent != null)
            {
                // Use bulk projection container for updating multiple records
                Container bulkProjectionContainer = await _nostify.GetBulkProjectionContainerAsync<FullAccount>();
                
                // Query all records that have accountManagerId equal to the event aggregateRootId
                var projectionsToUpdate = await bulkProjectionContainer
                    .GetItemLinqQueryable<FullAccount>()
                    .Where(p => p.accountManagerId == newEvent.aggregateRootId)
                    .Select(p => p.id)
                    .ReadAllAsync();

                // Use MultiApplyAndPersistAsync to update all projections
                await _nostify.MultiApplyAndPersistAsync<FullAccount>(bulkProjectionContainer, newEvent, projectionsToUpdate);
            }                       
        }
        catch (Exception e)
        {
            await _nostify.HandleUndeliverableAsync(nameof(OnEmployeeUpdated_For_FullAccount), e.Message, newEvent);
        }
    }
}
