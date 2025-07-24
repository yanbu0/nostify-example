using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service_Service;

public class GetAccountStatus
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    public GetAccountStatus(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(GetAccountStatus))]
    public async Task<AccountStatus> Run(
        [HttpTrigger("get", Route = "AccountStatus/{aggregateId:guid}")] HttpRequestData req,
        Guid aggregateId,
        FunctionContext context,
        ILogger log)
    {
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<AccountStatus>();
        AccountStatus retObj = await currentStateContainer
                            .GetItemLinqQueryable<AccountStatus>()
                            .Where(x => x.id == aggregateId)
                            .FirstOrDefaultAsync();
                            
        return retObj;
    }
}

