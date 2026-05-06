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

public class BulkCreateAccount
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    private readonly ILogger<BulkCreateAccount> _logger;
    public BulkCreateAccount(HttpClient httpClient, INostify nostify, ILogger<BulkCreateAccount> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(BulkCreateAccount))]
    public async Task<int> Run(
        [HttpTrigger("post", Route = "Account/BulkCreate")] HttpRequestData req,
        FunctionContext context)
    {
        Guid userId = Guid.Empty; // You can replace this with actual user ID retrieval logic
        Guid tenantId = Guid.Empty; // You can replace this with actual partition key retrieval logic

        return await DefaultCommandHandler.HandleBulkCreateAsync<Account>(_nostify, AccountCommand.BulkCreate, req, userId, tenantId);
    }
}

