
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace Employee_Service;

public class UpdateEmployee
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    private readonly ILogger<UpdateEmployee> _logger;
    public UpdateEmployee(HttpClient httpClient, INostify nostify, ILogger<UpdateEmployee> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(UpdateEmployee))]
    public async Task<Guid> Run(
        [HttpTrigger("patch", Route = "Employee/{id:guid?}")] HttpRequestData req,
        FunctionContext context,
        Guid? id)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        return await DefaultCommandHandler.HandlePatchAsync<Employee>(_nostify, EmployeeCommand.Update, req, context, userId, tenantId);
    }
}

