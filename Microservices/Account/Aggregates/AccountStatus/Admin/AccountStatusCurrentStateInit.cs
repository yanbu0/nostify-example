using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using nostify;

namespace Account_Service;

public class AccountStatusCurrentStateInit
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    private readonly ILogger<AccountStatusCurrentStateInit> _logger;
    public AccountStatusCurrentStateInit(HttpClient httpClient, INostify nostify, ILogger<AccountStatusCurrentStateInit> logger)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
        this._logger = logger;
    }

    [Function(nameof(AccountStatusCurrentStateInit))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "AccountStatusCurrentStateInit")] HttpRequestData req)
    {
        await _nostify.RebuildCurrentStateContainerAsync<AccountStatus>();
        return new OkResult();
    }
}