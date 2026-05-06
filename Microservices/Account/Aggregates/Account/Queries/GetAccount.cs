using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class GetAccount
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    private readonly ILogger<GetAccount> _logger;
    public GetAccount(HttpClient httpClient, INostify nostify, ILogger<GetAccount> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(GetAccount))]
    public async Task<Account> Run(
        [HttpTrigger("get", Route = "Account/{aggregateId:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid aggregateId)
    {
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Account>();
        Account retObj = await currentStateContainer
                            .ReadItemAsync<Account>(aggregateId.ToString(), new PartitionKey(tenantId.ToString()));
                            
        return retObj;
    }
}

