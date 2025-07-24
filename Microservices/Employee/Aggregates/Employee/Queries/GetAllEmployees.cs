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
    public GetAllEmployees(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(GetAllEmployees))]
    public async Task<List<Employee>> Run(
        [HttpTrigger("get", Route = "Employee")] HttpRequestData req,
        FunctionContext context,
        ILogger log)
    {
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Employee>();
        List<Employee> allList = await currentStateContainer
                            .GetItemLinqQueryable<Employee>()
                            .ReadAllAsync();


        return allList;
    }
}

