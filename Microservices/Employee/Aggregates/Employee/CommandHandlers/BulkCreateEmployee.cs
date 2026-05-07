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

public class BulkCreateEmployee
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    private readonly ILogger<BulkCreateEmployee> _logger;
    public BulkCreateEmployee(HttpClient httpClient, INostify nostify, ILogger<BulkCreateEmployee> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(BulkCreateEmployee))]
    public async Task<int> Run(
        [HttpTrigger("post", Route = "Employee/BulkCreate")] HttpRequestData req,
        FunctionContext context)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic

        return await DefaultCommandHandler.HandleBulkCreateAsync<Employee>(_nostify, EmployeeCommand.BulkCreate, req, userId, tenantId);
    }
}

