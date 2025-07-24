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
    public GetEmployee(HttpClient httpClient, INostify nostify)
    {
        this._client = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(GetEmployee))]
    public async Task<Employee> Run(
        [HttpTrigger("get", Route = "Employee/{aggregateId:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid aggregateId,
        ILogger log)
    {
        Container currentStateContainer = await _nostify.GetCurrentStateContainerAsync<Employee>();
        Employee retObj = await currentStateContainer
                            .GetItemLinqQueryable<Employee>()
                            .Where(x => x.id == aggregateId)
                            .FirstOrDefaultAsync();
                            
        return retObj;
    }
}

