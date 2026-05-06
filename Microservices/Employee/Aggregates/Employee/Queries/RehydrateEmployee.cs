using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Employee_Service;

public class RehydrateEmployee
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    private readonly ILogger<RehydrateEmployee> _logger;
    public RehydrateEmployee(HttpClient httpClient, INostify nostify, ILogger<RehydrateEmployee> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(RehydrateEmployee))]
    public async Task<Employee> Run(
        [HttpTrigger("get", Route = "RehydrateEmployee/{aggregateId:guid}/{datetime:datetime?}")] HttpRequestData req,
        Guid aggregateId,
        DateTime? dateTime)
    {
        Employee retObj = await _nostify.RehydrateAsync<Employee>(aggregateId, dateTime);
                            
        return retObj;
    }
}

