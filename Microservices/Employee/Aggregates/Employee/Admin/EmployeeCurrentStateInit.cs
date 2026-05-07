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
    private readonly ILogger<EmployeeCurrentStateInit> _logger;
    public EmployeeCurrentStateInit(HttpClient httpClient, INostify nostify, ILogger<EmployeeCurrentStateInit> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(EmployeeCurrentStateInit))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "EmployeeCurrentStateInit")] HttpRequestData req)
    {
        await _nostify.RebuildCurrentStateContainerAsync<Employee>();
        return new OkResult();
    }
}