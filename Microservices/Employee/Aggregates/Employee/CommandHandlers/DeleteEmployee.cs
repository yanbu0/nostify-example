
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Employee_Service;

public class DeleteEmployee
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    private readonly ILogger<DeleteEmployee> _logger;
    public DeleteEmployee(HttpClient httpClient, INostify nostify, ILogger<DeleteEmployee> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(DeleteEmployee))]
    public async Task<Guid> Run(
        [HttpTrigger("delete", Route = "Employee/{aggregateId:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid aggregateId)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        return await DefaultCommandHandler.HandleDeleteAsync<Employee>(_nostify, EmployeeCommand.Delete, aggregateId, userId, tenantId);
    }
}

