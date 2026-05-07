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
    private readonly ILogger<GetAllAccounts> _logger;
    public GetAllAccounts(HttpClient httpClient, INostify nostify, ILogger<GetAllAccounts> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(GetAllAccounts))]
    public async Task<List<Account>> Run(
        [HttpTrigger("get", Route = "Account")] HttpRequestData req,
        FunctionContext context)
    {
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Account>();
        List<Account> allList = await currentStateContainer
                            .FilteredQuery<Account>(tenantId)
                            .ReadAllAsync();


        return allList;
    }
}

