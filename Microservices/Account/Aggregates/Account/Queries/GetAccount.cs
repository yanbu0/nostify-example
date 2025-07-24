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
    public GetAccount(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(GetAccount))]
    public async Task<Account> Run(
        [HttpTrigger("get", Route = "Account/{aggregateId:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid aggregateId,
        ILogger log)
    {
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Account>();
        Account retObj = await currentStateContainer
                            .GetItemLinqQueryable<Account>()
                            .Where(x => x.id == aggregateId)
                            .FirstOrDefaultAsync();
                            
        return retObj;
    }
}

