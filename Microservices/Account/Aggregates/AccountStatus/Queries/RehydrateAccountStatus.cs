using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service_Service;

public class RehydrateAccountStatus
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    public RehydrateAccountStatus(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(RehydrateAccountStatus))]
    public async Task<AccountStatus> Run(
        [HttpTrigger("get", Route = "RehydrateAccountStatus/{aggregateId:guid}/{datetime:datetime?}")] HttpRequestData req,
        Guid aggregateId,
        DateTime? dateTime,
        ILogger log)
    {
        AccountStatus retObj = await _nostify.RehydrateAsync<AccountStatus>(aggregateId, dateTime);
                            
        return retObj;
    }
}

