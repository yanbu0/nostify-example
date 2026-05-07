using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class GetAllFullAccounts
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    private readonly ILogger<GetAllFullAccounts> _logger;
    public GetAllFullAccounts(HttpClient httpClient, INostify nostify, ILogger<GetAllFullAccounts> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(GetAllFullAccounts))]
    public async Task<List<FullAccount>> Run(
        [HttpTrigger("get", Route = "FullAccount")] HttpRequestData req,
        FunctionContext context)
    {
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        Container projectionContainer = await _nostify.GetProjectionContainerAsync<FullAccount>();
        List<FullAccount> allList = await projectionContainer
                            .FilteredQuery<FullAccount>(tenantId)
                            .ReadAllAsync();


        return allList;
    }

}