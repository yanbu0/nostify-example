using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using nostify;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace Account_Service_Service;

public class BulkCreateAccountStatus
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public BulkCreateAccountStatus(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(BulkCreateAccountStatus))]
    public async Task<int> Run(
        [HttpTrigger("post", Route = "AccountStatus/BulkCreate")] HttpRequestData req,
        ILogger log)
    {
        List<dynamic> newAccountStatusList = JsonConvert.DeserializeObject<List<dynamic>>(await new StreamReader(req.Body).ReadToEndAsync()) ?? new List<dynamic>();
        List<Event> peList = new List<Event>();

        newAccountStatusList.ForEach(e =>
        {
            //Need new id for aggregate root since its new
            Guid newId = Guid.NewGuid();
            e.id = newId;
            
            Event pe = new Event(AccountStatusCommand.BulkCreate, newId, e, Guid.Empty, Guid.Empty); //Empty guids should be replaced with user id and tenant id respectively
            peList.Add(pe);
        });

        await _nostify.BulkPersistEventAsync(peList);

        return newAccountStatusList.Count;
    }
}

