
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace Account_Service;

public class UpdateAccount
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public UpdateAccount(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(UpdateAccount))]
    public async Task<Guid> Run(
        [HttpTrigger("patch", Route = "Account")] HttpRequestData req,
        ILogger log)
    {
        dynamic updateAccount = await req.Body.ReadFromRequestBodyAsync();
        Guid aggRootId = Guid.Parse(updateAccount.id.ToString());
        Event pe = new Event(AccountCommand.Update, aggRootId, updateAccount);
        await _nostify.PersistEventAsync(pe);

        return updateAccount.id;
    }
}

