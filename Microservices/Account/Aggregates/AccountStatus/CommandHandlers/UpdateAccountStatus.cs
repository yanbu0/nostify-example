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
    private readonly ILogger<UpdateAccountStatus> _logger;
    public UpdateAccountStatus(HttpClient httpClient, INostify nostify, ILogger<UpdateAccountStatus> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(UpdateAccountStatus))]
    public async Task<Guid> Run(
        [HttpTrigger("patch", Route = "AccountStatus/{id:guid?}")] HttpRequestData req,
        FunctionContext context,
        Guid? id)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        return await DefaultCommandHandler.HandlePatchAsync<AccountStatus>(_nostify, AccountStatusCommand.Update, req, context, userId, tenantId);
    }
}

