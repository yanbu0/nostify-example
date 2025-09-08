using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class DeleteAccountStatus
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public DeleteAccountStatus(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(DeleteAccountStatus))]
    public async Task<Guid> Run(
        [HttpTrigger("delete", Route = "AccountStatus/{aggregateId:guid}")] HttpRequestData req,
        Guid aggregateId,
        ILogger log)
    {
        IEvent pe = new EventFactory().Create<AccountStatus>(AccountStatusCommand.Delete, aggregateId, new { id = aggregateId });
        await _nostify.PersistEventAsync(pe);

        return aggregateId;
    }
}

