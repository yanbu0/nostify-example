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

public class CreateAccount
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public CreateAccount(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(CreateAccount))]
    public async Task<Guid> Run(
        [HttpTrigger("post", Route = "Account")] HttpRequestData req,
        ILogger log)
    {
        dynamic newAccount = await req.Body.ReadFromRequestBodyAsync(true);

        //Need new id for aggregate root since its new
        Guid newId = Guid.NewGuid();
        newAccount.id = newId;
        
        IEvent pe = new EventFactory().Create<Account>(AccountCommand.Create, newId, newAccount);
        await _nostify.PersistEventAsync(pe);

        return newId;
    }
}

