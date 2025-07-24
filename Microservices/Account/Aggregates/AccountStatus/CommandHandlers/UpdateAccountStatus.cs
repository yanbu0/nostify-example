using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace Account_Service;

public class UpdateAccountStatus
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public UpdateAccountStatus(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(UpdateAccountStatus))]
    public async Task<Guid> Run(
        [HttpTrigger("patch", Route = "AccountStatus")] HttpRequestData req,
        ILogger log)
    {
        dynamic updateAccountStatus = await req.Body.ReadFromRequestBodyAsync();
        Guid aggRootId = Guid.Parse(updateAccountStatus.id.ToString());
        Event pe = new Event(AccountStatusCommand.Update, aggRootId, updateAccountStatus);
        await _nostify.PersistEventAsync(pe);

        return updateAccountStatus.id;
    }
}

