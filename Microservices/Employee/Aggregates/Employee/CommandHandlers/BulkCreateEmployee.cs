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
    public BulkCreateEmployee(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(BulkCreateEmployee))]
    public async Task<int> Run(
        [HttpTrigger("post", Route = "Employee/BulkCreate")] HttpRequestData req,
        ILogger log)
    {
        List<dynamic> newEmployeeList = JsonConvert.DeserializeObject<List<dynamic>>(await new StreamReader(req.Body).ReadToEndAsync()) ?? new List<dynamic>();
        List<IEvent> peList = new List<IEvent>();

        newEmployeeList.ForEach(e =>
        {
            //Need new id for aggregate root since its new
            Guid newId = Guid.NewGuid();
            e.id = newId;
            
            IEvent pe = new EventFactory().Create<Employee>(EmployeeCommand.BulkCreate, newId, e, Guid.Empty, Guid.Empty); //Empty guids should be replaced with user id and tenant id respectively
            peList.Add(pe);
        });

        await _nostify.BulkPersistEventAsync(peList);

        return newEmployeeList.Count;
    }
}

