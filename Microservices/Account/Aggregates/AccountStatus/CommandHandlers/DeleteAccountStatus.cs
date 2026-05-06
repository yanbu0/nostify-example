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
    private readonly ILogger<DeleteAccountStatus> _logger;
    public DeleteAccountStatus(HttpClient httpClient, INostify nostify, ILogger<DeleteAccountStatus> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(DeleteAccountStatus))]
    public async Task<Guid> Run(
        [HttpTrigger("delete", Route = "AccountStatus/{aggregateId:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid aggregateId)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        return await DefaultCommandHandler.HandleDeleteAsync<AccountStatus>(_nostify, AccountStatusCommand.Delete, aggregateId, userId, tenantId);
    }
}

