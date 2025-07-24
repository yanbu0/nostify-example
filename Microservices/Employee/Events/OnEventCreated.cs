using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;

namespace Employee_Service;

public class OnEventCreated
{
    private readonly INostify _nostify;

    public OnEventCreated(INostify nostify)
    {
        this._nostify = nostify;
    }

    [Function(nameof(OnEventCreated))]
    public async Task Run([CosmosDBTrigger(
            databaseName: "Employee_DB",
            containerName: "eventStore",
            Connection = "CosmosConnectionString",
            CreateLeaseContainerIfNotExists = true,
            LeaseContainerPrefix = "OnEventCreated_",
            LeaseContainerName = "leases")] string peListString,
        ILogger log)
    {
        await _nostify.PublishEventAsync(peListString);
    }
}

