using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class GetFullAccount
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    private readonly ILogger<GetFullAccount> _logger;
    public GetFullAccount(HttpClient httpClient, INostify nostify, ILogger<GetFullAccount> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(GetFullAccount))]
    public async Task<FullAccount> Run(
        [HttpTrigger("get", Route = "FullAccount/{aggregateId:guid}")] HttpRequestData req,
        Guid aggregateId,
        FunctionContext context)
    {
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        Container projectionContainer = await _nostify.GetProjectionContainerAsync<FullAccount>();
        FullAccount retObj = await projectionContainer
                            .ReadItemAsync<FullAccount>(aggregateId.ToString(), new PartitionKey(tenantId.ToString()));
                            
        return retObj;
    }
}

