
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Employee_Service;

public class DeleteEmployee
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public DeleteEmployee(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(DeleteEmployee))]
    public async Task<Guid> Run(
        [HttpTrigger("delete", Route = "Employee/{aggregateId:guid}")] HttpRequestData req,
        Guid aggregateId,
        ILogger log)
    {
        IEvent pe = new EventFactory().CreateNullPayloadEvent(EmployeeCommand.Delete, aggregateId);
        await _nostify.PersistEventAsync(pe);

        return aggregateId;
    }
}

