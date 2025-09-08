
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class DeleteAccount
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public DeleteAccount(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(DeleteAccount))]
    public async Task<Guid> Run(
        [HttpTrigger("delete", Route = "Account/{aggregateId:guid}")] HttpRequestData req,
        Guid aggregateId,
        ILogger log)
    {
        IEvent pe = new EventFactory().CreateNullPayloadEvent(AccountCommand.Delete, aggregateId);
        await _nostify.PersistEventAsync(pe);

        return aggregateId;
    }
}

