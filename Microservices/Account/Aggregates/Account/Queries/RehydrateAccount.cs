using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class RehydrateAccount
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    public RehydrateAccount(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(RehydrateAccount))]
    public async Task<Account> Run(
        [HttpTrigger("get", Route = "RehydrateAccount/{aggregateId:guid}/{datetime:datetime?}")] HttpRequestData req,
        Guid aggregateId,
        DateTime? dateTime,
        ILogger log)
    {
        Account retObj = await _nostify.RehydrateAsync<Account>(aggregateId, dateTime);
                            
        return retObj;
    }
}

