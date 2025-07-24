using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using nostify;

namespace Account_Service_Service;

public class AccountStatusCurrentStateInit
{

    private readonly HttpClient _httpClient;
    private readonly INostify _nostify;
    public AccountStatusCurrentStateInit(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(AccountStatusCurrentStateInit))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "AccountStatusCurrentStateInit")] HttpRequestData req,
        ILogger log)
    {
        await _nostify.RebuildCurrentStateContainerAsync<AccountStatus>();
        return new OkResult();
    }
}