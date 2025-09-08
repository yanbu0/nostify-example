using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using nostify;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace Account_Service;

public class CreateAccountStatus
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public CreateAccountStatus(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(CreateAccountStatus))]
    public async Task<Guid> Run(
        [HttpTrigger("post", Route = "AccountStatus")] HttpRequestData req,
        ILogger log)
    {
        dynamic newAccountStatus = await req.Body.ReadFromRequestBodyAsync(true);

        //Need new id for aggregate root since its new
        Guid newId = Guid.NewGuid();
        newAccountStatus.id = newId;
        
        IEvent pe = new EventFactory().Create<AccountStatus>(AccountStatusCommand.Create, newId, newAccountStatus);
        await _nostify.PersistEventAsync(pe);

        return newId;
    }
}

