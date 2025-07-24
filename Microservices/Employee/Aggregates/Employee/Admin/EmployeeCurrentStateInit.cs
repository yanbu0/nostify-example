using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using nostify;

namespace Employee_Service;

public class EmployeeCurrentStateInit
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public EmployeeCurrentStateInit(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(EmployeeCurrentStateInit))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "EmployeeCurrentStateInit")] HttpRequestData req,
        ILogger log)
    {
        await _nostify.RebuildCurrentStateContainerAsync<Employee>();
        return new OkResult();
    }
}