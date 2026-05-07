
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using System.Threading.Tasks;

namespace Employee_Service;

/// <summary>
/// Kafka trigger that handles incoming <see cref="AsyncEventRequest"/> messages.
/// Queries the event store for the requested aggregate root IDs and publishes
/// <see cref="AsyncEventRequestResponse"/> chunks to the response topic specified in the request.
/// </summary>
public class AsyncEventRequestHandler
{
    private readonly INostify _nostify;
    private readonly ILogger<AsyncEventRequestHandler> _logger;

    public AsyncEventRequestHandler(INostify nostify, ILogger<AsyncEventRequestHandler> logger)
    {
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(AsyncEventRequestHandler))]
    public async Task Run([KafkaTrigger("BrokerList",
                "Employee_EventRequest",
                #if DEBUG
                Protocol = BrokerProtocol.NotSet,
                AuthenticationMode = BrokerAuthenticationMode.NotSet,
                #else
                Username = "KafkaApiKey",
                Password = "KafkaApiSecret",
                Protocol =  BrokerProtocol.SaslSsl,
                AuthenticationMode = BrokerAuthenticationMode.Plain,
                #endif
                ConsumerGroup = "Employee_AsyncEventRequestHandler"
                )] NostifyKafkaTriggerEvent triggerEvent)
    {
        await DefaultEventRequestHandlers.HandleAsyncEventRequestAsync(_nostify, triggerEvent, _logger);
    }
}
