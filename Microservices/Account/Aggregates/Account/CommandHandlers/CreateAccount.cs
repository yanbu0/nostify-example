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
    private readonly ILogger<CreateAccount> _logger;
    public CreateAccount(HttpClient httpClient, INostify nostify, ILogger<CreateAccount> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(CreateAccount))]
    public async Task<Guid> Run(
        [HttpTrigger("post", Route = "Account")] HttpRequestData req,
        FunctionContext context)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic
        return await DefaultCommandHandler.HandlePostAsync<Account>(_nostify, AccountCommand.Create, req, userId, tenantId);
    }
}

