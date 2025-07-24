
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
    public UpdateEmployee(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(UpdateEmployee))]
    public async Task<Guid> Run(
        [HttpTrigger("patch", Route = "Employee")] HttpRequestData req,
        ILogger log)
    {
        dynamic updateEmployee = await req.Body.ReadFromRequestBodyAsync();
        Guid aggRootId = Guid.Parse(updateEmployee.id.ToString());
        Event pe = new Event(EmployeeCommand.Update, aggRootId, updateEmployee);
        await _nostify.PersistEventAsync(pe);

        return updateEmployee.id;
    }
}

