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
    public AccountCurrentStateInit(HttpClient httpClient, INostify nostify)
    {
        this._httpClient = httpClient;
        this._nostify = nostify;
    }

    [Function(nameof(AccountCurrentStateInit))]
    public async Task<IActionResult> Run(
        [HttpTrigger("post", Route = "AccountCurrentStateInit")] HttpRequestData req,
        ILogger log)
    {
        await _nostify.RebuildCurrentStateContainerAsync<Account>();
        return new OkResult();
    }
}