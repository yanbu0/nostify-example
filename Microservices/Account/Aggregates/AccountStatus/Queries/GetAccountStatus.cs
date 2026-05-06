using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class GetAccountStatus
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    private readonly ILogger<GetAccountStatus> _logger;
    public GetAccountStatus(HttpClient httpClient, INostify nostify, ILogger<GetAccountStatus> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(GetAccountStatus))]
    public async Task<AccountStatus> Run(
        [HttpTrigger("get", Route = "AccountStatus/{aggregateId:guid}")] HttpRequestData req,
        Guid aggregateId,
        FunctionContext context)
    {
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<AccountStatus>();
        AccountStatus retObj = await currentStateContainer
                            .ReadItemAsync<AccountStatus>(aggregateId.ToString(), new PartitionKey(tenantId.ToString()));
                            
        return retObj;
    }
}

