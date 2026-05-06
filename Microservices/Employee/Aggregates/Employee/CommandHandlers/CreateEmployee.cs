using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using nostify;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace Employee_Service;

public class CreateEmployee
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    private readonly ILogger<CreateEmployee> _logger;
    public CreateEmployee(HttpClient httpClient, INostify nostify, ILogger<CreateEmployee> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(CreateEmployee))]
    public async Task<Guid> Run(
        [HttpTrigger("post", Route = "Employee")] HttpRequestData req,
        FunctionContext context)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        return await DefaultCommandHandler.HandlePostAsync<Employee>(_nostify, EmployeeCommand.Create, req, userId, tenantId);
    }
}

