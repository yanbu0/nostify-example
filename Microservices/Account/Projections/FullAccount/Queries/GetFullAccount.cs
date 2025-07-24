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
    public GetFullAccount(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(GetFullAccount))]
    public async Task<FullAccount> Run(
        [HttpTrigger("get", Route = "FullAccount/{aggregateId:guid}")] HttpRequestData req,
        Guid aggregateId,
        FunctionContext context,
        ILogger log)
    {
        Container projectionContainer = await _nostify.GetProjectionContainerAsync<FullAccount>();
        FullAccount retObj = await projectionContainer
                            .GetItemLinqQueryable<FullAccount>()
                            .Where(x => x.id == aggregateId)
                            .FirstOrDefaultAsync();
                            
        return retObj;
    }
}

