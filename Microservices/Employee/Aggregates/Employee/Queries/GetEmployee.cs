using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Employee_Service;

public class GetEmployee
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    private readonly ILogger<GetEmployee> _logger;
    public GetEmployee(HttpClient httpClient, INostify nostify, ILogger<GetEmployee> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(GetEmployee))]
    public async Task<Employee> Run(
        [HttpTrigger("get", Route = "Employee/{aggregateId:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid aggregateId)
    {
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Employee>();
        Employee retObj = await currentStateContainer
                            .ReadItemAsync<Employee>(aggregateId.ToString(), new PartitionKey(tenantId.ToString()));
                            
        return retObj;
    }
}

