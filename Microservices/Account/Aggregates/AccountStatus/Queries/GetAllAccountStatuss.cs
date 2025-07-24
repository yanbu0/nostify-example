using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service_Service;

public class GetAllAccountStatuss
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    public GetAllAccountStatuss(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(GetAllAccountStatuss))]
    public async Task<List<AccountStatus>> Run(
        [HttpTrigger("get", Route = "AccountStatus")] HttpRequestData req,
        FunctionContext context,
        ILogger log)
    {
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<AccountStatus>();
        List<AccountStatus> allList = await currentStateContainer
                            .GetItemLinqQueryable<AccountStatus>()
                            .ReadAllAsync();


        return allList;
    }
}

