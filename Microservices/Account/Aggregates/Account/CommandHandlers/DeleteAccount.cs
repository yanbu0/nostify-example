
using Microsoft.Extensions.Logging;
using nostify;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Account_Service;

public class DeleteAccount
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    private readonly ILogger<DeleteAccount> _logger;
    public DeleteAccount(HttpClient httpClient, INostify nostify, ILogger<DeleteAccount> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(DeleteAccount))]
    public async Task<Guid> Run(
        [HttpTrigger("delete", Route = "Account/{aggregateId:guid}")] HttpRequestData req,
        FunctionContext context,
        Guid aggregateId)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        return await DefaultCommandHandler.HandleDeleteAsync<Account>(_nostify, AccountCommand.Delete, aggregateId, userId, tenantId);
    }
}

