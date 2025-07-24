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
    public GetAllFullAccounts(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(GetAllFullAccounts))]
    public async Task<List<FullAccount>> Run(
        [HttpTrigger("get", Route = "FullAccount")] HttpRequestData req,
        FunctionContext context,
        ILogger log)
    {
        Container projectionContainer = await _nostify.GetProjectionContainerAsync<FullAccount>();
        List<FullAccount> allList = await projectionContainer
                            .GetItemLinqQueryable<FullAccount>()
                            .ReadAllAsync();


        return allList;
    }

}