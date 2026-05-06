using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class GetAllAccountStatuss
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    private readonly ILogger<GetAllAccountStatuss> _logger;
    public GetAllAccountStatuss(HttpClient httpClient, INostify nostify, ILogger<GetAllAccountStatuss> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(GetAllAccountStatuss))]
    public async Task<List<AccountStatus>> Run(
        [HttpTrigger("get", Route = "AccountStatus")] HttpRequestData req,
        FunctionContext context)
    {
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<AccountStatus>();
        List<AccountStatus> allList = await currentStateContainer
                            .FilteredQuery<AccountStatus>(tenantId)
                            .ReadAllAsync();


        return allList;
    }
}

