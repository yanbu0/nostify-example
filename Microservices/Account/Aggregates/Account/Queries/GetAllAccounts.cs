using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class GetAllAccounts
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    public GetAllAccounts(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(GetAllAccounts))]
    public async Task<List<Account>> Run(
        [HttpTrigger("get", Route = "Account")] HttpRequestData req,
        FunctionContext context,
        ILogger log)
    {
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Account>();
        List<Account> allList = await currentStateContainer
                            .GetItemLinqQueryable<Account>()
                            .ReadAllAsync();


        return allList;
    }
}

