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
    public CreateEmployee(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(CreateEmployee))]
    public async Task<Guid> Run(
        [HttpTrigger("post", Route = "Employee")] HttpRequestData req,
        ILogger log)
    {
        dynamic newEmployee = await req.Body.ReadFromRequestBodyAsync(true);

        //Need new id for aggregate root since its new
        Guid newId = Guid.NewGuid();
        newEmployee.id = newId;
        
        IEvent pe = new EventFactory().Create<Employee>(EmployeeCommand.Create, newId, newEmployee);
        await _nostify.PersistEventAsync(pe);

        return newId;
    }
}

