using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Employee_Service;

public class GetAllEmployees
{

    private readonly HttpClient _client;
    private readonly INostify _nostify;
    private readonly ILogger<GetAllEmployees> _logger;
    public GetAllEmployees(HttpClient httpClient, INostify nostify, ILogger<GetAllEmployees> logger)
    {
        this._client = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(GetAllEmployees))]
    public async Task<List<Employee>> Run(
        [HttpTrigger("get", Route = "Employee")] HttpRequestData req,
        FunctionContext context)
    {
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Employee>();
        List<Employee> allList = await currentStateContainer
                            .FilteredQuery<Employee>(tenantId)
                            .ReadAllAsync();


        return allList;
    }
}

