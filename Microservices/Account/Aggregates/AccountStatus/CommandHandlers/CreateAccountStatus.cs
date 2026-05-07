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
    private readonly ILogger<CreateAccountStatus> _logger;
    public CreateAccountStatus(HttpClient httpClient, INostify nostify, ILogger<CreateAccountStatus> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(CreateAccountStatus))]
    public async Task<Guid> Run(
        [HttpTrigger("post", Route = "AccountStatus")] HttpRequestData req,
        FunctionContext context)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        return await DefaultCommandHandler.HandlePostAsync<AccountStatus>(_nostify, AccountStatusCommand.Create, req, userId, tenantId);
    }
}

