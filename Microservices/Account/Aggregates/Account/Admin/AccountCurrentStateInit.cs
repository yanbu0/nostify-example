using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using nostify;

namespace Account_Service;

public class AccountCurrentStateInit
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    private readonly ILogger<AccountCurrentStateInit> _logger;
    public AccountCurrentStateInit(HttpClient httpClient, INostify nostify, ILogger<AccountCurrentStateInit> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(AccountCurrentStateInit))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "AccountCurrentStateInit")] HttpRequestData req)
    {
        await _nostify.RebuildCurrentStateContainerAsync<Account>();
        return new OkResult();
    }
}